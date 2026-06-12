using System;
using System.Collections;
using UnityEngine;
using APLab.Backend;

namespace APLab.Core
{
    /// <summary>
    /// Persistent app bootstrap. Lives in the Bootstrap scene, owns the Supabase
    /// client + session tracker, restores a saved session on launch, and flushes
    /// any queued offline results once signed in.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class APLabManager : MonoBehaviour
    {
        public static APLabManager Instance { get; private set; }

        [Header("Assign in the Bootstrap scene")]
        public APLabConfig config;

        public SupabaseClient Supabase { get; private set; }
        public SessionTracker Session { get; private set; }

        public bool IsReady { get; private set; }
        public event Action OnReady;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (config == null)
            {
                Debug.LogError("[APLab] APLabConfig is not assigned on APLabManager.");
                return;
            }

            Supabase = GetComponent<SupabaseClient>() ?? gameObject.AddComponent<SupabaseClient>();
            Supabase.Init(config);

            Session = GetComponent<SessionTracker>() ?? gameObject.AddComponent<SessionTracker>();
            Session.Init(config);

            Supabase.OnAuthChanged += HandleAuthChanged;
            StartCoroutine(Boot());
        }

        private IEnumerator Boot()
        {
            yield return Supabase.TryRestoreSession(_ => { });
            IsReady = true;
            OnReady?.Invoke();
        }

        private void HandleAuthChanged()
        {
            if (Supabase != null && Supabase.IsSignedIn)
                LocalCache.FlushPending(Supabase);
        }

        /// <summary>Stable per-install id used to bind a pairing code to this headset.</summary>
        public string DeviceId
        {
            get
            {
                const string key = "aplab_device_id";
                var id = PlayerPrefs.GetString(key, "");
                if (string.IsNullOrEmpty(id))
                {
                    id = Guid.NewGuid().ToString("N");
                    PlayerPrefs.SetString(key, id);
                    PlayerPrefs.Save();
                }
                return id;
            }
        }
    }
}
