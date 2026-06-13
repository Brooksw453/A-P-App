# A&P Lab — Project Context (read me first)

This file is the canonical context for Claude Code sessions on this project. It captures
**what we're building, where things are, how to reconnect the Unity bridge, and what's next.**
Keep it updated as the project evolves.

---

## 0. ⏭️ RESUME HERE — first move in a fresh chat (saved end of 2026-06-12, marathon #2)

> **▶️ RESUME NEXT (marathon #5, 2026-06-13 — STABLE on Quest; poke mechanic WORKS ✅):** On-device confirmed:
> **skull is stable in passthrough** (no fly-up), sits at a comfortable seated distance, **14 labeled bone dots**,
> and the **bone-ID Practice works** — poked the **Mandible** → green flash → advanced 1/6→2/6 (`[APLab] ✓ Mandible
> award=1.00`). Stable commits on `meta-xr-migration`: **`03477fc6`** (static visible poke-tip sphere) ·
> **`c49838f6`** (skull+labels moved ~0.25m back for reach) · **`151d7b67`** (fresh Meta Building Blocks rig).
>
> 🔑 **THE "SKULL FLIES UP ~100ft" DRIFT — ROOT CAUSE FOUND (cost us ~10 builds):** it was **runtime
> `GameObject.CreatePrimitive(Sphere)` in `PokeTip.Awake()`** — creating a collider-bearing object **at startup on
> an object parented under the Meta XR rig** disrupts the rig's tracking-origin init, so the whole world launches
> upward. **100% correlated across all 10 builds** (marker code present ⟺ drift; absent ⟺ stable). ⚠️ **The scene
> "save reshuffles 8600 lines" and "can't save in Unity" theories were RED HERRINGS** — saving is fine; those
> drift builds just also had the marker code. **RULE: never spawn objects at runtime under the XR rig during
> Awake/Start; add visible helpers as STATIC scene meshes** (that's how the cyan/poke-tip ball was finally added —
> a plain Sphere child of `Poke Tip`, collider deleted, in the scene). **Always Build (NOT Build-And-Run — that
> auto-launches on the desk, not worn, and also corrupts the origin); launch WHILE WORN; sideload via adb.**
>
> **OPEN follow-ups (small):** (1) the visible tip **ball isn't co-located with the `Poke Tip` trigger collider**
> (trigger sits ~at the hand) — align them (move the ball to the trigger, or the trigger out to the ball ~z 0.12).
> (2) **Deep/back bones unreachable** on a solid skull (occipital=back, sphenoid/ethmoid/vomer=internal) — the
> **exploded skull** is the real next feature (also powers the "place mandible in socket" step). (3) Wire
> **result-sync** to `vr-sync-result`. Editing+saving in Unity is back to normal.
>
> **▶️ RESUME NEXT (session-end 2026-06-13 — Android `level0` crash SOLVED ✅):** Root cause was a **stale
> hand-migrated OVR rig** (full saga in the marathon-#4 notes below). The MR scene is rebuilt and **verified on
> the Quest**: fresh **Meta Building Blocks** Camera Rig + Passthrough, **skull resized to ~life-size**, **14
> labels + BoneTargets regenerated**, **Poke Tip** re-added (script renamed `PokeInput.cs`→`PokeTip.cs` so the
> `PokeTip` MonoBehaviour is Add-Component-able — class name must == file name). On-device confirmed: **skull +
> labels render in full passthrough (look great)**; `[APLab] Starting module … 14 bone targets` runs; no crash.
> **2 OPEN ISSUES before the poke test works (Brooks flagged):** (1) **hands keep changing color** — the Building
> Blocks hand-tracking visual; investigate the hand material / tracking-confidence state. (2) **Poke Tip is
> invisible** (it's just a trigger collider) — **add a small visible marker** (a little sphere mesh on the
> `Poke Tip` object, ~collider size, maybe emissive) so you can see where to poke. **NEXT:** add that poke-tip
> visual → rebuild → poke the **Mandible** at Practice 1/6 (expect green flash + advance to 2/6). **Cleanup:**
> delete the throwaway `_Probe.unity`. Sideload + **launch-while-worn** recipe is in the marathon-#4 notes / §6.

> **🔴 UPDATE (marathon #4, 2026-06-13):** The full `Library` wipe + clean rebuild **did NOT fix `level0`** — the
> freshly-built APK crashes identically (`level0 corrupted / Position out of bounds`, `SIGTRAP` in
> `Loading.Preload`). **So a build *cache* is ruled out.** Cheap analysis this session also ruled out: oversized/
> baked scene data (longest scene line = 128 chars; no baked mesh/TMP geometry), custom-script serialization
> (`LabelInfo`/`LabelBillboard`/`BoneTarget`/`ModuleHost` clean — no `[SerializeReference]`/
> `ISerializationCallbackReceiver`), the force-included `Assets/Resources` (only small Meta/OVR `.asset`
> settings), build-time errors (build log clean — the Editor.log errors are just in-Editor Play-mode OpenXR
> "no runtime" noise), and Split-Application-Binary/OBB. **`level0` is physically in the APK but malformed.**
> **RESULT (marathon #4):** Deleted the `Skeletal Labels` root, rebuilt, sideloaded — **STILL crashes with the
> identical `level0 corrupted`** once launched correctly. **So the label rig is NOT the cause.** Two very
> different scene contents corrupt `level0` *identically* → leans toward a **project-wide build-pipeline issue**.
> ⚠️ **LAUNCH-WHILE-WORN GOTCHA (new):** adb-launching this app while the headset is asleep/off hangs Unity at
> early init (no display surface) → 5 s `FocusEvent` **ANR**, and it never reaches scene-load — which masked the
> crash and briefly looked like a fix. **Always launch while the headset is worn + awake** (`am force-stop`
> first, then `monkey`, with Brooks wearing it). **NEXT = step-7 empty-scene isolation test:** build a trivial
> `_Probe` scene as the ONLY enabled build scene, launch-while-worn. Empty ALSO corrupts ⇒ pipeline (attack
> Player/build settings, Unity version, packages, IL2CPP); empty runs ⇒ base-scene content (bisect OVR rig /
> skull / ModuleHost). adb recipe below works (device `230YC01DC1013Q`, pkg `com.ESD.AP.lab`).
>
> **✅ EMPTY-SCENE PROBE RAN CLEAN (marathon #4):** a trivial `_Probe` scene boots on the Quest with **no
> `level0` crash** ⇒ **NOT the build pipeline — the corruption is `Module_Skeletal` CONTENT.** Base scene (after
> label deletion) = **`MR Camera Rig`** (the OVR/passthrough prefab, GUID `126d619c…`) + **`Skull`**
> (`Skull_full.prefab`) + `Module Host` + `Directional Light`. **Prime suspect = `MR Camera Rig`** — it's the
> one serializing **Meta-package enums** (the documented "By Design" cause), likely made **stale by the
> OpenXR→Meta XR migration**. **NEXT cut:** delete `MR Camera Rig`, set `Module_Skeletal` back as the build scene
> (last build was `_Probe`!), rebuild → **boots ⇒ rig confirmed** (drop in a fresh Meta passthrough rig via
> Building Blocks); **still crashes ⇒ it's the `Skull` prefab.** Reminder: **always launch while worn.**
>
> **🎯 SOLVED (marathon #4): the `MR Camera Rig` was the cause.** Minus-rig `Module_Skeletal` boots clean — NO
> `level0` crash; `[APLab] Starting module 'Skeletal System: Skull & Bones'` runs on-device (Learn→Practice,
> "Practice 1/6: Select the ONLY moveable bone… answer Mandible"; 0 bone targets only because labels were also
> deleted). **ROOT CAUSE = a stale Meta/OVR passthrough rig (`MR Camera Rig`, prefab GUID `126d619c…`) left from
> the OpenXR→Meta XR migration, serializing a player-incompatible type (Unity's "By Design" package-enum bug).**
> Without a rig the build is flat/head-locked & no passthrough (expected — the rig holds the tracked camera +
> passthrough layer). **FIX PATH:** (1) add a FRESH passthrough rig via **Meta XR Building Blocks** (Camera Rig +
> Passthrough blocks — a current-package rig has no stale enums); (2) regenerate labels via menu *A&P Lab/
> Generate Skeletal Labels* (restores BoneTargets); (3) re-parent **Poke Tip** under the new rig's
> `RightHandAnchor`. NOTE: orphaned hand-anchor objects (`*HandOnControllerAnchor`, `*HandAnchorDetached`,
> `*ControllerInHandAnchor`) survived the rig deletion — clean those up when adding the new rig.
>
> **✅ FRESH RIG CONFIRMED WORKING ON-DEVICE (marathon #4):** Building Blocks **Camera Rig + Passthrough** →
> NO crash; MR service logs `[Passthrough] -> Started` for our pkg; `[APLab] Starting module` runs; and **Brooks
> confirms the skull floating in his room in full passthrough + proper head-tracked 3D** (stays put when he
> moves; good seated height/placement). **THE ANDROID `level0` WALL IS DOWN.** Remaining tuning + build-back:
> skull renders **~half life-size** → scale `Skull` ~2× toward ~0.21 m tall **before** regenerating labels (so
> targets fit); then menu *A&P Lab/Generate Skeletal Labels* (restores BoneTargets), re-parent **Poke Tip** under
> the new rig's `RightHandAnchor`, delete leftover orphan hand-anchors, rebuild → poke test.
> **LESSON (Brooks's call, now proven): add Meta rigs via Building Blocks — a clean current-package rig — and
> never hand-migrate an OVR rig (the hand-migrated one is what went stale and corrupted `level0`).**

**Where we are:** **Slice 1** of the bone-ID Practice (§9 step 2) is **built and verified in the Editor** —
autoSelfTest ran the full `Learn→Practice→Assess→Results` flow on-device-of-the-editor (practical = 77%). The
whole framework now runs **data-driven off `skeletal-system.json`**. The one remaining wall is **getting the
Android build to actually run on the Quest**. It now boots all the way through IL2CPP, Vulkan, OpenXR, and the
Meta MR Utility Kit, then **crashes while loading the scene**:
> `The file '.../assets/bin/Data/level0' is corrupted! [Position out of bounds!]` → `SIGTRAP` in `Loading.Preload`

`level0` = the `Module_Skeletal` scene baked into the APK. This is **a corrupt build artifact, NOT our code**
(the scene loads perfectly in the Editor; disk has 1.3 TB free). It survived a player-data cache clear
(`Library/PlayerDataCache` + `BuildPlayerData`), so the next step was a **full clean** — **now DONE** (the whole
`Library/` was wiped this session; see the NEXT MOVE step 1). The ball is in Brooks's court to reopen Unity +
rebuild. *(No MCP bridge needed for this — it's all file ops + adb.)*

**THE NEXT MOVE — full clean rebuild, then sideload:**
1. ✅ **DONE (marathon #3, 2026-06-12):** Unity was confirmed closed (no `Temp/UnityLockfile`) and the entire
   `A-P-App/Library/` (24.8 GB) was wiped clean — plus `Temp/` and `obj/` — via robocopy empty-mirror. Source
   (`Assets`, `Packages`, `ProjectSettings`, the scene + generator) untouched. **So the next person starts at
   step 2.** *(Pre-checked the scene for the step-7 suspects: it's structurally clean — 53 GameObjects, 14
   LineRenderer leader lines, 28 inline label materials, 15 SphereCollider bone-targets, **no baked Mesh blobs**,
   only 1 harmless zero-guid ref. Confirms the corruption was a build-cache artifact, not scene content — if the
   rebuild somehow STILL corrupts, the 28 inline label materials are the prime suspect for the step-7 bisect.)*
2. **Reopen Unity** — long reimport (full 24.8 GB regen — expect 30–60 min); let it settle (the Meta XR audio
   updater churns — normal).
3. **Re-enter keystore passwords** (they do **NOT** persist across Unity restarts — this bit us): Project
   Settings → Player → **Publishing Settings** → Keystore password + Key alias **`ap key`** password.
   Keystore: `C:\Users\brook\GitHub\Local Keystores\A-P  Lab Keystore 26.keystore` (note the double space).
4. **Build the APK** (just *Build*; no need to deploy).
5. **Tell Claude "built"** → Claude sideloads + launches + reads logcat (all via adb, zero headset fiddling):
   - adb: `C:\Program Files\Unity\Hub\Editor\6000.3.8f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe`
   - APK: `A-P-App/Library/Bee/Android/Prj/IL2CPP/Gradle/launcher/build/outputs/apk/release/launcher-release.apk`
   - `adb install -r -d "<apk>"` → `adb shell monkey -p com.ESD.AP.lab -c android.intent.category.LAUNCHER 1`
   - capture: `adb logcat -c`; launch; `adb logcat -d > log.txt`; grep `[APLab]` / `level0` / `Stripping`.
6. **Success = logcat shows `[APLab] Starting module…` and NO "level0 corrupted".** Then the real poke test:
   set `Module Host` → **`autoSelfTest` = OFF** (so you poke manually) and use the **right Touch controller**
   (the Poke Tip rides `RightHandAnchor`; hand-tracking poke isn't wired yet). Report how poking *feels*
   (target size ~3.6 cm, tip offset 7 cm, reachability) and the skull's placement/legibility in passthrough.
7. **If `level0` is STILL corrupt after a full `Library` wipe** → it's the scene *content*, not a cache.
   Isolate: temporarily make a trivial empty scene the only enabled build scene, build, sideload. Empty scene
   runs → bisect/regenerate `Module_Skeletal` (suspect generated label objects / runtime-created materials);
   empty scene also corrupt → project-wide build-pipeline issue.

**Android facts you'll need (all hard-won 2026-06-12):**
- **Package name is `com.ESD.AP.lab`** (deliberate). The published store app is `com.ESD.AP` signed with the
  *original* key; our dev keystore differs, so a dev build can't replace the store app (signature mismatch) and
  the store re-syncs it. A separate package lets the dev build coexist. **Three same-named "Anatomy & Physiology
  Lab" apps are on the headset now:** `com.ESD.AP` (store), `com.Education_Simulation_Design.APLab` (older),
  **`com.ESD.AP.lab` (ours — the one to launch/test).**
- Build = **Android / IL2CPP / ARM64 / Vulkan**, Linear color space, GameActivity entry. **`stripEngineCode: 0`**
  and Android **`managedStrippingLevel: 0`** — we turned stripping OFF (it caused an *earlier* preload crash);
  **keep it off.**
- "Active Input Handling = **Both**" warns it's unsupported on Android — **not** a crash cause (our poke uses
  physics triggers + OVR tracking, not Unity input). Leave it, or set to one handler later.
- Test headset is a **Quest Pro (`seacliff`)** — fine, same MR/passthrough stack as Quest 3.
- adb on this device is noisy/large and sometimes stalls: clear (`-c`) before launch + use bounded dumps
  (`-d -t N`, or redirect to a file). A **Quest reboot drops adb** — re-accept the USB-debugging prompt in-headset.
- Slice-1 wiring already in the saved scene: **Module Host** (`ModuleHost`, `contentJson` = skeletal-system.json,
  `autoStartOnPlay` on) + **Poke Tip** under `RightHandAnchor`; 14 markers each carry a `BoneTarget`.

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
- **The bridge does NOT auto-reconnect on this project — a human must click `Start Session` after every drop.**
  Hard-won this session (2026-06-12): every script recompile (domain reload) AND every Play-mode enter drops
  the session to `instance_count: 0`, and it stays down until someone clicks **Window → MCP For Unity →
  Start Session**. Focusing the window / `AppActivate(<UnityPID>)` did **not** bring it back on its own.
  Implication: (1) batch script edits to minimize recompiles; (2) Claude can't press Play and then read the
  console over the bridge — instead read `Debug.Log` output straight from `Editor.log`
  (`C:\Users\brook\AppData\Local\Unity\Editor\Editor.log`), which needs no bridge. The Meta XR audio/spatializer
  updater churning after the Android platform switch made the drops more frequent this session.
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
  **A&P Lab/Generate Skeletal Labels** — stamps labels (TMP text built in code so it renders, + leader lines
  + markers) onto the skull. **Now all 14 bones** (2025-06-12): term + pronunciation pulled from
  `skeletal-system.json` (single source of truth), laid out in two columns with leader lines; TMP pivot on the
  inner edge so text grows outward (no mesh overlap). Each label carries a **`LabelInfo`** (term/pronunciation/
  definition/landmark, for the upcoming tap-the-bone Practice) and a **`LabelBillboard`** (faces the user at
  runtime; editor preview off by default) — both in `Assets/APLab/Runtime/View/`. Re-runnable; rebuilds the
  `Skeletal Labels` root. *(Lesson: ad-hoc TMP/`TextMesh` created via the bridge won't render in Unity 6 —
  build the text from a compiled script and call `ForceMeshUpdate`.)*
  ⚠️ **7 deep/posterior landmark points are approximate** (sphenoid, occipital, ethmoid, lacrimal, inferior
  nasal concha, vomer, palatine) — the generator logs them and sets `LabelInfo.approximate`; nudge the marker
  points live in the Editor (the 7 facial/cranial-surface bones are tuned). Some are really better shown on the
  exploded/cutaway skull later.
- ✅ **Build platform switched to Android** (IL2CPP/ARM64/Vulkan); `Module_Skeletal` is the only enabled build
  scene. ✅ **Slice-1 Practice built & Editor-verified** (see §9 step 2). ⛔ **Quest build does not run yet** —
  blocked on a corrupt `level0` scene artifact in the APK (`SIGTRAP` in `Loading.Preload`). **See §0 for exactly
  where to resume.** *(Brooks drives the Android side; Claude can sideload + read logcat via adb.)*
- ✅ **`Main` branch synced** (2026-06-12): all of today's work fast-forwarded onto `origin/Main` via a
  `-X ours` merge of `origin/Main` into our branch (kept Main's 2024 AP-logo/Menu-Table content, dropped the
  unused 42 MB mp3). PR #35 resolved. Working branch is still `meta-xr-migration`; `Main` == its tip.
- **Pilot = Skeletal System**, maps to course **§7.2**.
- ⚠️ **Bridge tip:** a domain reload (after a big reimport/merge/recompile) drops the Unity MCP session —
  the HTTP server stays up; just re-click **Start Session** in the MCP For Unity window to reconnect.

## 8. Known issues / cleanup

- ✅ **Duplicate `OVRPlugin.dll` — RESOLVED (2026-06-12).** This was NOT just a benign warning: on **Android**
  it became a **fatal build error** (`Error building Player: 7 errors`, failing in Preprocess Player in ~1s on
  the plugin-name collision). Fixed by deleting the leftover duplicate plugin files with Unity closed (DLLs are
  locked while it runs): `Assets/Oculus/VR/Plugins/1.82.0/Win64OpenXR/OVRPlugin.dll` and
  `Assets/Oculus/Spatializer/Plugins/x86_64/AudioPluginOculusSpatializer.dll` (+ their `.meta`).
  `OculusProjectConfig.asset` kept (OVRManager needs it). Empty `Assets/Oculus/VR` + `Spatializer` folders
  remain (harmless; delete in Explorer for tidiness).
- ⚠️ **Both Oculus XR Plugin (`com.unity.xr.oculus`) and OpenXR Plugin (`com.unity.xr.openxr`) are installed** —
  Unity logs "not recommended… OpenXR is the recommended plugin." It's an *advisory*, not one of the hard build
  errors, so left as-is for now. If it ever blocks a build, remove `com.unity.xr.oculus` from
  `Packages/manifest.json` (the project is on the Meta XR / OpenXR backend).
- ⛔ **Android build doesn't run yet — corrupt `level0` (the `Module_Skeletal` scene in the APK).** Full saga +
  fix in **§0**. Boots through all subsystems, then `SIGTRAP` in `Loading.Preload` on `level0 corrupted /
  Position out of bounds`. Not our code (loads in Editor). Fix = full `Library` wipe + clean rebuild.
- ⚠️ **Keystore passwords don't persist across Unity restarts** — re-enter them (Player → Publishing Settings)
  before every build after reopening the project, or the APK signing step fails (manifests as a misleading
  Gradle `PackageAndroidArtifact$IncrementalSplitterRunnable` error).
- ⚠️ **`com.ESD.AP.lab` dev package coexists with the published `com.ESD.AP` store app** (signature mismatch
  prevents replacing it). Three same-named apps are on the test headset — see §0. Sideload **ours** via
  `adb install -r -d` of `launcher-release.apk` and launch `com.ESD.AP.lab`.
- ✅ Materials converted to URP. ✅ Skull is the clean `Skull_full` (the exploding-skull's offset pivot is
  parked for the per-bone explode/place lab later).
- Repo has historical line-ending churn; a proper `.gitattributes` pass is still pending. `.utmp/` (Android
  build scratch) is now in `.gitignore`.
- 🧹 **Asset-Store bloat to delete (planned cleanup — noted 2026-06-13; do AFTER a confirmed-running APK, not
  before — change one variable at a time).** `Assets/Unity Asset Store/` is **6.0 GB of the 6.6 GB total
  `Assets/`** — almost all unrelated to anatomy, and the main reason `Library` reimports / Android platform
  switches are slow (every `Library` wipe re-imports all of it). It does **NOT** bloat the APK (Unity only
  packages assets the build scene references) **except `Overhead Crane`**, which has a `Resources/` subfolder
  that ships in every build regardless. **Brooks owns all these packs on the Unity Asset Store, so any can be
  reimported anytime — deletion is safe and fully reversible.**
  - **DELETE (~4.5 GB, zero anatomy):** SuburbNeighborhoodHousePack (3.1 GB), RealisticKitchenPack (671 MB),
    Overhead Crane (279 MB — also un-ships it from the APK), Butterfly Animated (166 MB), Wood Boat Lowpoly
    (141 MB), Simple Garage (119 MB), Retro Cartoon Car Cicada (92 MB), Enemy_Spikeball, Fire, Sherbbs Particles.
  - **KEEP — in use now:** `Anatomical_Human_Skeleton` (750 MB — the `Skull_full` pilot model).
  - **KEEP — raw material for Season-1 modules:** Heart URP, motion open heart, Human Body Anatomy Male,
    RIG Female Dissection, Operation room - surgery, Scalpels, Skin model.
  - **Method:** ask Claude to generate the exact GUID-referenced keep/delete manifest from `Module_Skeletal`
    first; delete each folder **with its `.meta`** (or via the Unity Project window) with the scene closed/Unity
    closed; commit right after as a revertible checkpoint. Also check the top-level `Assets/Resources` (could be
    ours or more shipped junk).

## 9. Next steps (in order)

1. ✅ **Extend the labels** (2025-06-12) — all 14 bones, content pulled from `skeletal-system.json`,
   `LabelBillboard` faces the user, two-column layout. *Remaining:* nudge the 7 approximate deep/posterior
   marker points in the Editor (generator logs which). (Generator: `Assets/Editor/SkeletalLabelGenerator.cs`,
   re-run via menu *A&P Lab/Generate Skeletal Labels*.)
2. **Bone-ID / "tap the bone" Practice** — ⏳ *in progress.* ✅ **Slice 1 done & verified (2026-06-12):** the
   whole framework now runs data-driven off `skeletal-system.json`. New runtime: `ModuleContentLoader`
   (JSON→`ModuleDefinition` in memory, no SO assets — `Runtime/Content/`), `BoneTarget` (pokeable marker w/
   arm + correct/wrong flash), `PokeInput` (`PokeTip` trigger-poke for hand/controller + `MouseRaySelector`
   fallback), and `ModuleHost` (orchestrator that drives `ModuleRunner` Learn→Practice→Assess→Results and
   scores each IdentifyPart step). The generator now also stamps a trigger-collider `BoneTarget` on every
   marker. Scene `Module_Skeletal` wired: **Module Host** (autoSelfTest verified practical=77% — 5/5 bones +
   deferred PlaceInSocket) + **Poke Tip** under `RightHandAnchor`. *Remaining:* Learn-phase UI (slice 2) and
   the quiz/Assess UI (slice 3); real headset poke-test; `PlaceInSocket` scoring.
3. **Wire results** — `ModuleRunner` + `SessionTracker` + `SupabaseClient` → post a completed lab to
   **`vr-sync-result`** (writes `vr_lab_attempts`, upserts `section_progress`, logs `activity_log`). Add the
   device-code login UI.
4. **Hub / Atlas** — a home scene with mode select; then replicate the framework for the rest of Season 1
   (heart, respiratory, digestive, urinary, brain, muscular — see the `vr_modules` seed for section mapping).
5. **Quest build** (Brooks drives the Android side): ✅ platform switched to Android, `Module_Skeletal` is the
   build scene, APK builds + signs + sideloads. ⛔ **Currently blocked:** the APK boots fully (IL2CPP/Vulkan/
   OpenXR/MR) then crashes on a **corrupt `level0` scene artifact** — fix is a full `Library` wipe + clean
   rebuild. **See §0 for the step-by-step.**

## Session log

**2026-06-12 (marathon #2 — slice 1 + Android bringup)** — Built **slice 1 of the bone-ID Practice**: a
data-driven runtime that loads `skeletal-system.json` into an in-memory `ModuleDefinition`
(`ModuleContentLoader`), pokeable `BoneTarget` markers, a decoupled `PokeInput` layer (`PokeTip` +
`MouseRaySelector`), and a `ModuleHost` orchestrator driving `ModuleRunner` through all four phases. Extended
the label generator to all 14 bones (JSON-sourced text, billboard, two-column layout) and to stamp a
`BoneTarget` on each marker. Wired `Module Host` + `Poke Tip` into the scene. **Verified the whole flow in the
Editor** via an autoSelfTest (practical 77%, scoring flows through `ModuleRunner`). Then spent a long time on
**first-ever Quest/Android bringup**, clearing blocker after blocker: keystore, duplicate-`OVRPlugin` build
collision (§8), engine-stripping preload crash, a store-vs-dev **signature/package conflict** (→ renamed dev
build to `com.ESD.AP.lab`), keystore passwords not persisting across restarts, and finally a **corrupt `level0`
scene artifact** that still blocks the app from running on-device. **Banked here** (committed/pushed) with slice
1 Editor-verified. **Resume = §0** (full `Library` wipe → rebuild → sideload). Slices 2 (Learn UI) & 3 (quiz +
Supabase result-sync) still pending.

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
