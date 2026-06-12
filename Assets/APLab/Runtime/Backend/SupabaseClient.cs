using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace APLab.Backend
{
    /// <summary>
    /// Minimal Supabase client for Unity built on UnityWebRequest + Newtonsoft.
    /// Handles device-code pairing sign-in, token refresh/persistence, PostgREST
    /// reads/writes (as the signed-in user, respecting RLS), and edge-function
    /// invocation. Lives on the persistent Bootstrap object.
    /// </summary>
    public class SupabaseClient : MonoBehaviour
    {
        public static SupabaseClient Instance { get; private set; }

        private APLabConfig _cfg;
        private AuthSession _session;
        private const string RefreshKey = "aplab_refresh_token";

        public bool IsSignedIn => _session != null && !string.IsNullOrEmpty(_session.AccessToken);
        public string UserId => _session?.User?.Id;
        public string UserEmail => _session?.User?.Email;
        public event Action OnAuthChanged;

        public void Init(APLabConfig cfg)
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _cfg = cfg;
            DontDestroyOnLoad(gameObject);
        }

        // ---- Sign-in: device-code pairing ------------------------------------

        /// <summary>
        /// Full pairing flow. Reports the short code to display, then signs in
        /// once the student approves it on the web /link page.
        /// </summary>
        public IEnumerator DeviceLoginFlow(string deviceId, Action<PairStartResponse> onCode,
            Action onSignedIn, Action<string> onError)
        {
            PairStartResponse start = null; string err = null;
            yield return Post(_cfg.FunctionsUrl + "/" + _cfg.fnPairStart, AnonHeaders(),
                JsonConvert.SerializeObject(new { device_id = deviceId }),
                body => start = JsonConvert.DeserializeObject<PairStartResponse>(body),
                e => err = e);
            if (err != null || start == null || string.IsNullOrEmpty(start.Code)) { onError?.Invoke(err ?? "pair start failed"); yield break; }
            onCode?.Invoke(start);

            float interval = Mathf.Max(2, start.PollIntervalSeconds);
            DateTime expires = ParseTime(start.ExpiresAt, DateTime.UtcNow.AddMinutes(10));
            while (DateTime.UtcNow < expires)
            {
                yield return new WaitForSeconds(interval);
                PairPollResponse poll = null; string perr = null;
                yield return Post(_cfg.FunctionsUrl + "/" + _cfg.fnPairPoll, AnonHeaders(),
                    JsonConvert.SerializeObject(new { code = start.Code, device_id = deviceId }),
                    body => poll = JsonConvert.DeserializeObject<PairPollResponse>(body),
                    e => perr = e);
                if (perr != null || poll == null) continue;
                if (poll.Status == "pending") continue;
                if (poll.Status == "approved" && !string.IsNullOrEmpty(poll.TokenHash))
                {
                    yield return VerifyTokenHash(poll.TokenHash, onSignedIn, onError);
                    yield break;
                }
                onError?.Invoke("pairing " + poll.Status);
                yield break;
            }
            onError?.Invoke("pairing code expired");
        }

        private IEnumerator VerifyTokenHash(string tokenHash, Action onSignedIn, Action<string> onError)
        {
            string err = null; AuthSession session = null;
            yield return Post(_cfg.AuthUrl + "/verify", AnonHeaders(),
                JsonConvert.SerializeObject(new { type = "magiclink", token_hash = tokenHash }),
                body => session = JsonConvert.DeserializeObject<AuthSession>(body),
                e => err = e);
            if (err != null || session == null || string.IsNullOrEmpty(session.AccessToken))
            { onError?.Invoke(err ?? "verify failed"); yield break; }
            ApplySession(session);
            onSignedIn?.Invoke();
        }

        // ---- Session restore / refresh ---------------------------------------

        public IEnumerator TryRestoreSession(Action<bool> done)
        {
            string refresh = PlayerPrefs.GetString(RefreshKey, "");
            if (string.IsNullOrEmpty(refresh)) { done?.Invoke(false); yield break; }
            yield return RefreshWith(refresh, ok => done?.Invoke(ok));
        }

        private IEnumerator RefreshWith(string refreshToken, Action<bool> done)
        {
            string err = null; AuthSession session = null;
            yield return Post(_cfg.AuthUrl + "/token?grant_type=refresh_token", AnonHeaders(),
                JsonConvert.SerializeObject(new { refresh_token = refreshToken }),
                body => session = JsonConvert.DeserializeObject<AuthSession>(body),
                e => err = e);
            if (err != null || session == null || string.IsNullOrEmpty(session.AccessToken)) { done?.Invoke(false); yield break; }
            ApplySession(session);
            done?.Invoke(true);
        }

        private IEnumerator EnsureFreshToken(Action done)
        {
            if (_session == null) { done?.Invoke(); yield break; }
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long exp = _session.ExpiresAt > 0 ? _session.ExpiresAt : now;
            if (exp - now > 60) { done?.Invoke(); yield break; }
            yield return RefreshWith(_session.RefreshToken, _ => done?.Invoke());
        }

        private void ApplySession(AuthSession session)
        {
            if (session.ExpiresAt <= 0 && session.ExpiresIn > 0)
                session.ExpiresAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + session.ExpiresIn;
            _session = session;
            if (!string.IsNullOrEmpty(session.RefreshToken))
            {
                PlayerPrefs.SetString(RefreshKey, session.RefreshToken);
                PlayerPrefs.Save();
            }
            OnAuthChanged?.Invoke();
        }

        public void SignOut()
        {
            _session = null;
            PlayerPrefs.DeleteKey(RefreshKey);
            PlayerPrefs.Save();
            OnAuthChanged?.Invoke();
        }

        // ---- PostgREST (as the signed-in user) -------------------------------

        public void RestInsert(string table, object row, bool returnRepresentation,
            Action<string> onOk, Action<string> onError)
        {
            StartCoroutine(AuthedRest("POST", "/" + table, JsonConvert.SerializeObject(row),
                returnRepresentation ? "return=representation" : "return=minimal", onOk, onError));
        }

        public void RestPatch(string table, string filterQuery, object patch,
            Action<string> onOk, Action<string> onError)
        {
            StartCoroutine(AuthedRest("PATCH", "/" + table + "?" + filterQuery,
                JsonConvert.SerializeObject(patch), "return=minimal", onOk, onError));
        }

        public void RestSelect(string table, string query, Action<string> onOk, Action<string> onError)
        {
            StartCoroutine(AuthedRest("GET", "/" + table + (string.IsNullOrEmpty(query) ? "" : "?" + query),
                null, null, onOk, onError));
        }

        private IEnumerator AuthedRest(string method, string path, string json, string prefer,
            Action<string> onOk, Action<string> onError)
        {
            bool ready = false;
            yield return EnsureFreshToken(() => ready = true);
            if (!ready) { onError?.Invoke("not signed in"); yield break; }

            var headers = UserHeaders();
            if (!string.IsNullOrEmpty(prefer)) headers["Prefer"] = prefer;
            yield return Send(method, _cfg.RestUrl + path, headers, json, onOk, onError);
        }

        // ---- Edge functions --------------------------------------------------

        public void InvokeFunction(string fn, object body, bool useUserAuth,
            Action<string> onOk, Action<string> onError)
        {
            StartCoroutine(InvokeFunctionRoutine(fn, body, useUserAuth, onOk, onError));
        }

        private IEnumerator InvokeFunctionRoutine(string fn, object body, bool useUserAuth,
            Action<string> onOk, Action<string> onError)
        {
            var headers = AnonHeaders();
            if (useUserAuth)
            {
                bool ready = false;
                yield return EnsureFreshToken(() => ready = true);
                if (!ready || !IsSignedIn) { onError?.Invoke("not signed in"); yield break; }
                headers = UserHeaders();
            }
            yield return Post(_cfg.FunctionsUrl + "/" + fn, headers, JsonConvert.SerializeObject(body), onOk, onError);
        }

        // ---- HTTP plumbing ---------------------------------------------------

        private System.Collections.Generic.Dictionary<string, string> AnonHeaders()
        {
            return new System.Collections.Generic.Dictionary<string, string>
            {
                { "apikey", _cfg.publishableKey },
                { "Authorization", "Bearer " + _cfg.publishableKey },
                { "Content-Type", "application/json" },
            };
        }

        private System.Collections.Generic.Dictionary<string, string> UserHeaders()
        {
            return new System.Collections.Generic.Dictionary<string, string>
            {
                { "apikey", _cfg.publishableKey },
                { "Authorization", "Bearer " + _session.AccessToken },
                { "Content-Type", "application/json" },
            };
        }

        private IEnumerator Post(string url, System.Collections.Generic.Dictionary<string, string> headers,
            string json, Action<string> onOk, Action<string> onError)
        {
            yield return Send("POST", url, headers, json, onOk, onError);
        }

        private IEnumerator Send(string method, string url,
            System.Collections.Generic.Dictionary<string, string> headers, string json,
            Action<string> onOk, Action<string> onError)
        {
            using (var req = new UnityWebRequest(url, method))
            {
                if (!string.IsNullOrEmpty(json))
                    req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
                req.downloadHandler = new DownloadHandlerBuffer();
                if (headers != null)
                    foreach (var kv in headers) req.SetRequestHeader(kv.Key, kv.Value);

                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    onOk?.Invoke(req.downloadHandler.text);
                }
                else
                {
                    string detail = req.downloadHandler != null ? req.downloadHandler.text : "";
                    onError?.Invoke($"{(int)req.responseCode} {req.error} {detail}");
                }
            }
        }

        private static DateTime ParseTime(string iso, DateTime fallback)
        {
            return DateTime.TryParse(iso, null,
                System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal,
                out var t) ? t : fallback;
        }
    }
}
