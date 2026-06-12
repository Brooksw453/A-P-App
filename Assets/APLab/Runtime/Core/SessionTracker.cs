using System;
using Newtonsoft.Json;
using UnityEngine;
using APLab.Backend;

namespace APLab.Core
{
    /// <summary>
    /// Opens a vr_sessions row when a module (or the hub) is entered, accumulates
    /// active time, and closes it on exit / app-pause / quit. Writes go through
    /// the signed-in user's JWT (RLS-compliant).
    /// </summary>
    public class SessionTracker : MonoBehaviour
    {
        private APLabConfig _cfg;
        private string _sessionId;
        private float _accumSeconds;
        private bool _active;

        public void Init(APLabConfig cfg) => _cfg = cfg;

        public void StartSession(string moduleSlug, string courseId)
        {
            var sb = SupabaseClient.Instance;
            if (sb == null || !sb.IsSignedIn) return;

            _accumSeconds = 0f;
            _active = true;
            _sessionId = null;

            var row = new VrSessionInsert
            {
                UserId = sb.UserId,
                ModuleSlug = moduleSlug,
                CourseId = courseId,
                Device = _cfg != null ? _cfg.device : "Quest3",
                AppVersion = _cfg != null ? _cfg.appVersion : "0.1.0",
            };

            // PostgREST insert returns an array when return=representation.
            sb.RestInsert("vr_sessions", new[] { row }, true, body =>
            {
                try
                {
                    var rows = JsonConvert.DeserializeObject<VrSessionRow[]>(body);
                    if (rows != null && rows.Length > 0) _sessionId = rows[0].Id;
                }
                catch (Exception e) { Debug.LogWarning("[APLab] session parse: " + e.Message); }
            }, err => Debug.LogWarning("[APLab] session insert failed: " + err));
        }

        private void Update()
        {
            if (_active) _accumSeconds += Time.unscaledDeltaTime;
        }

        public void EndSession()
        {
            if (!_active) return;
            _active = false;

            var sb = SupabaseClient.Instance;
            if (sb == null || string.IsNullOrEmpty(_sessionId)) return;

            var patch = new
            {
                ended_at = DateTime.UtcNow.ToString("o"),
                duration_seconds = Mathf.RoundToInt(_accumSeconds),
            };
            sb.RestPatch("vr_sessions", "id=eq." + _sessionId, patch,
                _ => { }, err => Debug.LogWarning("[APLab] session end failed: " + err));
        }

        private void OnApplicationPause(bool paused) { if (paused) EndSession(); }
        private void OnApplicationQuit() => EndSession();
    }
}
