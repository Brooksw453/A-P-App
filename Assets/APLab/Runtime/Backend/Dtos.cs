using System.Collections.Generic;
using Newtonsoft.Json;

namespace APLab.Backend
{
    // ---- Device-code pairing -------------------------------------------------

    public class PairStartResponse
    {
        [JsonProperty("code")] public string Code;
        [JsonProperty("expires_at")] public string ExpiresAt;
        [JsonProperty("poll_interval_seconds")] public int PollIntervalSeconds = 3;
        [JsonProperty("error")] public string Error;
    }

    public class PairPollResponse
    {
        // status: pending | approved | consumed | expired | invalid | device_mismatch
        [JsonProperty("status")] public string Status;
        [JsonProperty("token_hash")] public string TokenHash;
        [JsonProperty("email")] public string Email;
        [JsonProperty("error")] public string Error;
    }

    // ---- GoTrue auth ---------------------------------------------------------

    public class AuthSession
    {
        [JsonProperty("access_token")] public string AccessToken;
        [JsonProperty("refresh_token")] public string RefreshToken;
        [JsonProperty("token_type")] public string TokenType;
        [JsonProperty("expires_in")] public long ExpiresIn;
        [JsonProperty("expires_at")] public long ExpiresAt; // unix seconds
        [JsonProperty("user")] public AuthUser User;
        [JsonProperty("error")] public string Error;
        [JsonProperty("error_description")] public string ErrorDescription;
        [JsonProperty("msg")] public string Msg;
    }

    public class AuthUser
    {
        [JsonProperty("id")] public string Id;
        [JsonProperty("email")] public string Email;
        [JsonProperty("user_metadata")] public Dictionary<string, object> UserMetadata;
    }

    // ---- Lab result sync (payload to vr-sync-result) -------------------------

    public class LabResultPayload
    {
        [JsonProperty("module_slug")] public string ModuleSlug;
        [JsonProperty("course_id")] public string CourseId;
        [JsonProperty("chapter_id", NullValueHandling = NullValueHandling.Ignore)] public int? ChapterId;
        [JsonProperty("section_id")] public string SectionId;
        [JsonProperty("attempt_number", NullValueHandling = NullValueHandling.Ignore)] public int? AttemptNumber;
        [JsonProperty("learn_completed")] public bool LearnCompleted;
        [JsonProperty("practical_score")] public float PracticalScore;   // 0..100
        [JsonProperty("quiz_score")] public float QuizScore;             // 0..100
        [JsonProperty("mastery_score")] public float MasteryScore;       // 0..1
        [JsonProperty("passed")] public bool Passed;
        [JsonProperty("duration_seconds")] public int DurationSeconds;
        [JsonProperty("rubric")] public object Rubric;                   // serialized RubricResult
    }

    public class SyncResultResponse
    {
        [JsonProperty("ok")] public bool Ok;
        [JsonProperty("attempt_id")] public string AttemptId;
        [JsonProperty("attempt_number")] public int AttemptNumber;
        [JsonProperty("progress_updated")] public bool ProgressUpdated;
        [JsonProperty("error")] public string Error;
    }

    // ---- Session time tracking (rows in vr_sessions) -------------------------

    public class VrSessionInsert
    {
        [JsonProperty("user_id")] public string UserId;
        [JsonProperty("module_slug", NullValueHandling = NullValueHandling.Ignore)] public string ModuleSlug;
        [JsonProperty("course_id", NullValueHandling = NullValueHandling.Ignore)] public string CourseId;
        [JsonProperty("device")] public string Device;
        [JsonProperty("app_version")] public string AppVersion;
    }

    public class VrSessionRow
    {
        [JsonProperty("id")] public string Id;
        [JsonProperty("duration_seconds")] public int DurationSeconds;
    }

    // ---- Catalog / progress reads -------------------------------------------

    public class VrModuleRow
    {
        [JsonProperty("slug")] public string Slug;
        [JsonProperty("title")] public string Title;
        [JsonProperty("unit")] public string Unit;
        [JsonProperty("course_id")] public string CourseId;
        [JsonProperty("chapter_id")] public int? ChapterId;
        [JsonProperty("section_id")] public string SectionId;
        [JsonProperty("est_minutes")] public int? EstMinutes;
        [JsonProperty("sort_order")] public int SortOrder;
        [JsonProperty("is_published")] public bool IsPublished;
    }

    public class VrLabAttemptRow
    {
        [JsonProperty("module_slug")] public string ModuleSlug;
        [JsonProperty("attempt_number")] public int AttemptNumber;
        [JsonProperty("practical_score")] public float? PracticalScore;
        [JsonProperty("quiz_score")] public float? QuizScore;
        [JsonProperty("mastery_score")] public float? MasteryScore;
        [JsonProperty("passed")] public bool Passed;
        [JsonProperty("submitted_at")] public string SubmittedAt;
    }
}
