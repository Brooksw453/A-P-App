# A&P Lab — Project Context (read me first)

This file is the canonical context for Claude Code sessions on this project. It captures
**what we're building, where things are, how to reconnect the Unity bridge, and what's next.**
Keep it updated as the project evolves.

---

## 1. What this is

A **3D Anatomy & Physiology lab companion app** for **Meta Quest 3**, built in **Unity 6000.3.8f1**,
running in **full passthrough / mixed reality**. It pairs with a **fully-online college A&P 1 & 2
course** mapped to the **OpenStax Anatomy & Physiology 2e** textbook.

- **Two experiences:** (a) an **Exploration / Atlas** mode with labeled anatomy (brain, skeleton, etc.),
  and (b) a series of **lab simulations**, each with a *Learn* (reviewable) component and a *hands-on
  Practical* component. Time + scores persist to Supabase and show up as learning stats.
- **Goal:** a student takes the online class and, at key points, does the matching 3D lab in the
  headset; results flow back into the same course dashboards.
- **Owner:** Brooks Winchell. The app was recently migrated **OpenXR → Meta XR Interaction SDK**
  (that migration lives on the `meta-xr-migration` branch).
- **Build approach (important):** **data-driven** — a simulation = a content definition (ScriptableObject/JSON)
  + an anatomy model, assembled into a scene by reusable framework code. We do **not** hand-build each scene.

## 2. Repo / folder layout

- **This repo = the Unity project** (`A-P-App`). Remote: `github.com/Brooksw453/A-P-App`. Active branch: **`meta-xr-migration`**.
- **Parent folder** `C:\Users\brook\GitHub\A&P Lab\` (NOT a git repo) holds:
  - `anatomy-and-physiology-2e_-_WEB.pdf` — the OpenStax textbook (**keep OUT of git**; it lives outside the repo so it's already untracked).
  - `Skull Labels.docx` — author's per-bone skull descriptions (source for skeletal labels).
  - `visible-interactive-human-exploding-skull/` — exploding-skull source assets.
- **Companion web course** (separate repo): `C:\Users\brook\GitHub\Anatomy-and-Physiology\adaptive-learning`
  — Next.js 16 + Vercel + the **same Supabase**. Course slug / `course_id` = **`anatomy-and-physiology`**,
  28 chapters, sections like `7.2`. Content under `content/chapters/chNN/`.

## 3. The app framework — `Assets/APLab/`

- `Runtime/Backend/` — `SupabaseClient` (auth + PostgREST + edge calls), `Dtos`.
- `Runtime/Core/` — `APLabManager`, `ModuleRunner` (Learn→Practice→Assess→Results state machine),
  `SessionTracker` (time tracking), `ScoreModel`, `LocalCache` (offline cache), `ModuleModels` (schemas).
- `Runtime/Config/` — `APLabConfig` (Supabase URL/key, etc.).
- `Content/` — per-module content JSON. `skeletal-system.json` = the pilot's labels/quiz/practical,
  authored from OpenStax 7.2 + `Skull Labels.docx`.
- `Scenes/` — `Module_Skeletal.unity` (the pilot scene).

## 4. Backend — Supabase project "Course Dashboard"

- **Project ref:** `awcmkderlpyxpkmrnvwi` (Postgres 17). It's a **live multi-tenant LMS** shared with the
  web course (29 real users). Don't break existing tables; only add.
- **URL:** `https://awcmkderlpyxpkmrnvwi.supabase.co` · **publishable key** (public, safe in client):
  `sb_publishable_KPsJZn3qXMIm_GpFbXl6iA_lBvbUPpj`
- **Existing key tables we integrate with:** `profiles`, `enrollments`, `section_progress`,
  `quiz_attempts`, `activity_log` — all keyed by `(user_id, course_id text, chapter_id int, section_id text)`.
  RLS helpers: `is_instructor()`, `is_admin()`; students restricted to `auth.uid() = user_id`.
- **VR tables we added:** `vr_modules` (catalog), `vr_sessions` (time), `vr_lab_attempts` (scores,
  mirrors `quiz_attempts`), `vr_pairing_codes` (device-code login; RLS-locked, edge-function-only).
- **Edge functions:** `vr-pair-start` / `vr-pair-approve` / `vr-pair-poll` (device-code "pair + jump"
  login via magic-link OTP), and **`vr-sync-result`** (inserts `vr_lab_attempts`, upserts
  `section_progress.mastery_score`, logs `activity_log` type `vr_lab` → shows up in course dashboards/grades).
- `vr_modules` seeded with 12 modules mapped to real sections (`is_published=false` drafts). Pilot
  `skeletal-system` → chapter 7, section `7.2`.
- Use the Supabase MCP tools (`list_tables`, `apply_migration`, `execute_sql`, `deploy_edge_function`,
  `get_advisors`) for backend work.

## 5. Course ↔ VR integration (in the WEB repo, not deployed yet)

In `Anatomy-and-Physiology/adaptive-learning` I added (uncommitted, for review):
`src/components/VRLabCallout.tsx`, a `courseConfig.vr` flag, the `qrcode.react` dep, and mounted the
callout on the section page. It shows a "Launch on Quest" card on sections that have a published
`vr_module`. To ship: `npm i qrcode.react`, set `courseConfig.vr.appLinkBase`, deploy via Vercel.

---

## 6. ⚙️ Unity MCP bridge — HOW TO CONNECT (this was painful; here's the recipe)

We build in Unity through the **MCP for Unity** bridge (CoplayDev `com.coplaydev.unity-mcp`, server
package `mcpforunityserver`). **Architecture: ONE HTTP server on `127.0.0.1:8080` that BOTH Claude
(via `/mcp`) and the Unity Editor (via WebSocket) connect to. stdio does NOT bridge Unity — don't use it.**

**Prereqs (already installed on this machine):**
- Python 3.14 at `C:\Users\brook\AppData\Local\Python\pythoncore-3.14-64\python.exe` (not on PATH; fine).
- `uv` / `uvx` at `C:\Users\brook\.local\bin\`.
- **`uv tool install mcpforunityserver==9.6.2`** was run → gives `C:\Users\brook\.local\bin\mcp-for-unity.exe`.
  THIS WAS THE KEY FIX: plain `uvx --from mcpforunityserver` re-downloaded ~70 packages on *every* launch
  (~20s), which beat Unity's connect timeout and made "Start Server" appear to fail. The installed exe
  launches instantly.

**Reconnect recipe for a fresh session:**
1. **Server (durable, survives Claude restarts):** launch it detached from Claude via WMI (a Claude-launched
   process dies when Claude restarts; a WMI/scheduled-task one survives). PowerShell:
   `Invoke-CimMethod -ClassName Win32_Process -MethodName Create -Arguments @{ CommandLine = 'cmd.exe /c "C:\Users\brook\mcp-bridge.cmd > C:\Users\brook\mcp-bridge.log 2>&1"' }`
   where `C:\Users\brook\mcp-bridge.cmd` runs:
   `"C:\Users\brook\.local\bin\mcp-for-unity.exe" --transport http --http-url http://127.0.0.1:8080 --project-scoped-tools`
   (Alternatively: in the Unity MCP window click **Start Server** — it's fast now that the tool is installed.)
2. **Claude config (must be HTTP, not stdio):**
   `& "C:\Users\brook\.local\bin\claude.exe" mcp add --transport http mcp-for-unity "http://127.0.0.1:8080/mcp" -s user`
   ⚠️ The MCP For Unity window's "Configure" auto-writes a **stdio** entry for Claude Code that conflicts
   with the HTTP server (Claude ends up on a stdio server with no Unity). Keep the user-scope config HTTP.
3. **Unity side:** Window → **MCP For Unity** → Transport = **HTTP Local**, URL `http://127.0.0.1:8080`.
   Ensure the server is running, then click **Start Session** (a *separate* button from Start Server!) →
   it should show green **"Session Active (A-P-App)"**. Don't re-click Start Server while one is running (port conflict).
4. **Restart Claude Code** so the new session reads the HTTP config and connects to 8080.
5. **Verify:** `ReadMcpResourceTool mcp-for-unity mcpforunity://instances` → should show `transport: http`,
   `instance_count: 1`, `A-P-App@...`. Then `read_console` / `manage_scene get_active` should work.

**Gotchas (learned the hard way):**
- Don't `Stop-Process` things under `C:\Users\brook\.local\bin\` broadly — that kills Unity's server too.
- `claude mcp list` does a *fresh* check (will say ✓ Connected) even when the *running session* is on a stale
  stdio server. Trust `mcpforunity://instances` (transport + instance_count), not just `claude mcp list`.
- After you restart the **Unity Editor**, its instance token changes — just re-click **Start Session**.
- The MCP window has **Start Server** (launches the http server) vs **Start Session** (connects Unity to it) —
  both are needed; "No Session" with a red dot = server up but session not started.

**How we drive Unity:** `manage_scene`, `manage_gameobject`, `manage_components`, `manage_asset`,
`read_console`, `manage_camera` (screenshots), `execute_menu_item`. Build **sequentially** (one live bridge —
don't fan out parallel agents onto it). Claude can't see headset output — the user builds to Quest to verify.

---

## 7. Current status (update me!)

- ✅ Supabase VR backend live (tables + edge functions + seeded `vr_modules`).
- ✅ Course-side `VRLabCallout` written (web repo, not deployed).
- ✅ `APLab` framework C# in project, compiles clean.
- ✅ Unity MCP bridge working.
- ✅ **Pilot scene `Module_Skeletal.unity`** built: clean **`Skull_full`** model (Anatomical_Human_Skeleton,
  ~19 cm, rotated to face the user), directional light, and the **Meta MR passthrough rig** (`OVRCameraRig`
  + `OVRManager` insight passthrough + `OVRPassthroughLayer` underlay + transparent center-eye clear).
  *(Swapped off the exploding-skull prefab — its pivot is offset from the mesh; revisit that one for the
  per-bone explode/place practical, ideally positioning it live in the Editor.)*
- ✅ Imported anatomy **materials converted to URP** (Render Pipeline Converter run; the earlier "green" was
  just a selection gizmo, not the model).
- ✅ **Labeled skull** via a **data-driven generator** — `Assets/Editor/SkeletalLabelGenerator.cs`, menu
  **A&P Lab/Generate Skeletal Labels** — stamps cranial/facial labels (TMP text built in code so it renders,
  + leader lines + markers) onto the skull. Currently 7 visible bones; re-runnable, extendable to all 14 from
  `skeletal-system.json`. *(Lesson: ad-hoc TMP/`TextMesh` created via the bridge won't render in Unity 6 —
  build the text from a compiled script and call `ForceMeshUpdate`.)*
- ⚠️ **Build platform is still `StandaloneWindows64`.** A Quest build needs switching to **Android** (a long
  full asset reimport — do it as a focused pre-build step), then set `Module_Skeletal` as the startup build
  scene, build the APK (IL2CPP/ARM64), deploy via Meta Quest Developer Hub. *(Brooks handles the Android side.)*
- ✅ **`Main` branch synced** (2026-06-12): all of today's work fast-forwarded onto `origin/Main` via a
  `-X ours` merge of `origin/Main` into our branch (kept Main's 2024 AP-logo/Menu-Table content, dropped the
  unused 42 MB mp3). PR #35 resolved. Working branch is still `meta-xr-migration`; `Main` == its tip.
- **Pilot = Skeletal System**, maps to course **§7.2**.
- ⚠️ **Bridge tip:** a domain reload (after a big reimport/merge/recompile) drops the Unity MCP session —
  the HTTP server stays up; just re-click **Start Session** in the MCP For Unity window to reconnect.

## 8. Known issues / cleanup

- **Duplicate `OVRPlugin.dll`** (the only recurring console error — benign warning): the legacy `Assets/Oculus/`
  was *partially* deleted; `OVRPlugin.dll` + the spatializer DLL are locked while Unity runs, and
  `OculusProjectConfig.asset` was kept (needed by `OVRManager`). To finish: **close Unity → delete
  `Assets/Oculus/VR` and `Assets/Oculus/Spatializer` → reopen** (keep `OculusProjectConfig.asset`).
- ✅ Materials converted to URP. ✅ Skull is the clean `Skull_full` (the exploding-skull's offset pivot is
  parked for the per-bone explode/place lab later).
- Repo has historical line-ending churn; a proper `.gitattributes` pass is still pending.

## 9. Next steps (in order)

1. **Extend the labels** — all 14 bones + pull definitions from `skeletal-system.json`; add a small billboard
   so labels face the user; nudge positions. (Generator: `Assets/Editor/SkeletalLabelGenerator.cs`, re-run via
   menu *A&P Lab/Generate Skeletal Labels*.)
2. **Bone-ID / "tap the bone" Practice** — Meta hand grab/poke on the skull, scored (the hands-on phase).
3. **Wire results** — `ModuleRunner` + `SessionTracker` + `SupabaseClient` → post a completed lab to
   **`vr-sync-result`** (writes `vr_lab_attempts`, upserts `section_progress`, logs `activity_log`). Add the
   device-code login UI.
4. **Hub / Atlas** — a home scene with mode select; then replicate the framework for the rest of Season 1
   (heart, respiratory, digestive, urinary, brain, muscular — see the `vr_modules` seed for section mapping).
5. **Quest build** (Brooks drives the Android side): switch platform to Android, set `Module_Skeletal` as the
   startup build scene, build the APK, deploy via Meta Quest Developer Hub, verify passthrough in-headset.

## Session log

**2026-06-12 (marathon)** — Took the project from a stalled, *uncommitted* OpenXR→Meta XR migration to: migration
protected and **`Main` fully synced** (PR #35 resolved, kept Main's 2024 content, dropped a 42 MB unused mp3);
a live **Supabase VR backend** (4 tables + 4 edge functions + seeded module catalog); course-side `VRLabCallout`;
authored Skeletal content; the **Unity MCP bridge made to work *and* documented** (the big time-sink — see §6);
a **Meta MR passthrough scene** with a clean skull; **URP material conversion**; and a **data-driven label
generator → labeled skull**. All committed/pushed to `Main`. Stopped before bone-ID interaction, result-sync,
and the Quest build (see §9). To resume: connect the repo, open `A-P-App` in Unity, follow §6 to reconnect the
bridge, then say "go."

## 10. The full design/plan

The original architecture + simulation catalog is in the plan file:
`C:\Users\brook\.claude\plans\hi-here-is-my-nested-wave.md`.
