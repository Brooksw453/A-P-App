using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using APLab.Backend;

namespace APLab.Core
{
    /// <summary>
    /// Offline fallback: queues lab-result payloads in PlayerPrefs when a sync
    /// fails (no network, or before vr-sync-result is live) so they can be
    /// re-sent on the next successful sign-in. Best-effort, not a full sync DB.
    /// </summary>
    public static class LocalCache
    {
        private const string PendingKey = "aplab_pending_results";

        public static void SavePendingResult(LabResultPayload payload)
        {
            var list = LoadPending();
            list.Add(payload);
            PlayerPrefs.SetString(PendingKey, JsonConvert.SerializeObject(list));
            PlayerPrefs.Save();
        }

        public static List<LabResultPayload> LoadPending()
        {
            var raw = PlayerPrefs.GetString(PendingKey, "");
            if (string.IsNullOrEmpty(raw)) return new List<LabResultPayload>();
            try { return JsonConvert.DeserializeObject<List<LabResultPayload>>(raw) ?? new List<LabResultPayload>(); }
            catch { return new List<LabResultPayload>(); }
        }

        public static void ClearPending()
        {
            PlayerPrefs.DeleteKey(PendingKey);
            PlayerPrefs.Save();
        }

        public static bool HasPending => !string.IsNullOrEmpty(PlayerPrefs.GetString(PendingKey, ""));

        /// <summary>Re-send queued results; clears the queue if all succeed.</summary>
        public static void FlushPending(SupabaseClient client)
        {
            if (client == null || !client.IsSignedIn) return;
            var list = LoadPending();
            if (list.Count == 0) return;

            int remaining = list.Count;
            bool anyFailed = false;
            foreach (var payload in list)
            {
                client.InvokeFunction("vr-sync-result", payload, true,
                    _ => { if (--remaining == 0 && !anyFailed) ClearPending(); },
                    _ => { anyFailed = true; if (--remaining == 0) { /* keep queue for next time */ } });
            }
        }
    }
}
