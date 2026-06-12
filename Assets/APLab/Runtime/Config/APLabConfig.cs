using UnityEngine;

namespace APLab
{
    /// <summary>
    /// Project-wide configuration for the A&P Lab companion app.
    /// Holds the (public) Supabase client values and edge-function names.
    /// Create one asset via Create > A&P Lab > Config and reference it from the
    /// APLabManager in the Bootstrap scene.
    /// </summary>
    [CreateAssetMenu(fileName = "APLabConfig", menuName = "A&P Lab/Config")]
    public class APLabConfig : ScriptableObject
    {
        [Header("Supabase project (Course Dashboard)")]
        public string supabaseUrl = "https://awcmkderlpyxpkmrnvwi.supabase.co";

        [Tooltip("Publishable (public) client key. Safe to ship in the app build.")]
        public string publishableKey = "sb_publishable_KPsJZn3qXMIm_GpFbXl6iA_lBvbUPpj";

        [Header("Edge function names")]
        public string fnPairStart = "vr-pair-start";
        public string fnPairPoll = "vr-pair-poll";
        public string fnSyncResult = "vr-sync-result";

        [Header("Web approval page shown to the student during pairing")]
        public string linkPageUrl = "https://your-course-site.example/link";

        [Header("App / device metadata")]
        public string appVersion = "0.1.0";
        public string device = "Quest3";

        public string RestUrl => $"{supabaseUrl}/rest/v1";
        public string AuthUrl => $"{supabaseUrl}/auth/v1";
        public string FunctionsUrl => $"{supabaseUrl}/functions/v1";
    }
}
