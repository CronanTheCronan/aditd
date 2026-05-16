# Build Log

## 2026-05-16 - Wave 008A-FixPass: Shadow Perception MVP Audit Fixes

### Summary

Applied the accepted Claude audit fixes for Wave 008A without expanding system scope: documented authorization in `Docs/DECISIONS.md`, added one-time broken-setup warning coverage for `ShadowRevealable`, removed one unused public API from `ShadowPerceptionController`, corrected inactive tint color caching to use instance material color, and simplified the Wave 007B setup so the seam uses visibility wiring without redundant tint assignment.

### Files Changed

- `Assets/_Project/Code/Shadow/ShadowRevealable.cs`
- `Assets/_Project/Code/Shadow/ShadowPerceptionController.cs`
- `Assets/_Project/Code/Editor/Wave007BHouseWithTheSwitchesSetup.cs`
- `Docs/BUILD_LOG.md`
- `Docs/DECISIONS.md`

### What Was Fixed

- Added a dated authorization entry to `Docs/DECISIONS.md` for the provisional Wave 008A Shadow Perception MVP and its explicit non-goals.
- Added a clear one-time `Debug.LogWarning` path in `ShadowRevealable` when both inspector assignment and `FindAnyObjectByType<ShadowPerceptionController>()` fallback fail, so broken setup does not fail silently.
- Changed `ShadowRevealable.CacheInactiveColors()` to cache `renderer.material.color` instead of `renderer.sharedMaterial.color`, preserving instance-material palette state when present.
- Removed the unused public `TemporaryBindingNote` property from `ShadowPerceptionController` while keeping the serialized temporary-binding note for Inspector documentation.
- Updated the Wave 007B setup tool so the seam revealable keeps `_renderersToToggleVisibility` wired but leaves `_renderersToTint` empty, avoiding redundant seam tint wiring.
- Expanded the Wave 008A build-log coverage with missing manual edge-case checks and explicit Unity validation checks.

### Manual Test Steps

1. Open Unity and allow scripts to import.
2. Confirm no Unity Console compile errors after import.
3. Run `ADITD/Setup/Recreate Wave 007B House With the Switches`.
4. Enter Play Mode.
5. Confirm normal first-person movement, look, and interaction still work.
6. Aim at or approach the hidden seam/test crack.
7. Confirm it is hidden while `Q` is not held.
8. Hold `Q` before pressing `E` while the room is still `Unresolved`.
9. Confirm the seam does not reveal while the room is still `Unresolved`.
10. Release `Q`.
11. Press `E` on the ordinary switch without Shadow perception and confirm wrong-form destabilization still works.
12. Hold `Q` after destabilization and confirm the seam/truth becomes visible.
13. Release `Q` before completing the room and confirm the seam hides again without errors.
14. Hold `Q` again and confirm Shadow perception reactivates cleanly.
15. While holding `Q`, press `E` on the same switch and confirm the room completes.
16. If feasible from the Inspector during Play Mode, disable `ShadowPerceptionController`.
17. Confirm revealables clean up/hide without errors when the controller is disabled.
18. Stop Play Mode.
19. Confirm no console errors, missing scripts, broken serialized references, or null-reference warnings.

### Manual Unity Validation Steps

1. Run `ADITD/Setup/Recreate Wave 007B House With the Switches`.
2. Select the generated `Player` object and confirm `ShadowPerceptionController` is present.
3. Select `HiddenSeamRoot` and confirm `ShadowRevealable` is present.
4. In `HiddenSeamRoot`'s `ShadowRevealable`, confirm the renderer references under visibility toggling are assigned as intended.
5. In `HiddenSeamRoot`'s `ShadowRevealable`, confirm tint renderer references are intentionally empty for the seam setup.
6. Select the root `Wave007B_HouseWithTheSwitches` object and confirm `HouseWithTheSwitchesController` has its `_shadowPerception` reference assigned.
7. Confirm no missing scripts or broken serialized references are present after the setup menu finishes.

### Known Limitations

- Shadow perception remains visual-only.
- `Q` binding is still temporary and not yet routed through the Input Actions asset.
- No Shadow Charge.
- No audio treatment.
- No pressure integration.
- No full form switching.
- No split-screen or third-person Shadow proxy.
- No save/load or Hearth interaction.
- Revealable visuals remain prototype quality.
- This pass does not address one-frame flicker.
- This pass does not change seam root active/inactive behavior.
- Rerunning `ADITD/Setup/Recreate Wave 007B House With the Switches` rebuilds the Wave 007B graybox scene from scratch and does not preserve manual scene tweaks.
- Headless Unity scene generation is still avoided because of the prior local `Unity.Licensing.Client.exe` exception.

### Rollback Notes

- Revert `Assets/_Project/Code/Shadow/ShadowRevealable.cs`.
- Revert `Assets/_Project/Code/Shadow/ShadowPerceptionController.cs`.
- Revert `Assets/_Project/Code/Editor/Wave007BHouseWithTheSwitchesSetup.cs`.
- Revert `Docs/DECISIONS.md`.
- Remove the Wave 008A-FixPass entry from the top of `Docs/BUILD_LOG.md`.

## 2026-05-16 - Wave 008A: Shadow Perception MVP

### Summary

Replaced the Wave 007B room’s temporary local `Q` placeholder with a small reusable Shadow Perception MVP: a player-bound perception controller plus reusable revealable component, then rewired the Wave 007B graybox seam to use that layer without touching player movement, look, or interaction architecture.

### Files Changed

- `Assets/_Project/Code/Shadow/IShadowRevealable.cs`
- `Assets/_Project/Code/Shadow/ShadowPerceptionController.cs`
- `Assets/_Project/Code/Shadow/ShadowRevealable.cs`
- `Assets/_Project/Code/Rooms/HouseWithTheSwitchesController.cs`
- `Assets/_Project/Code/Editor/Wave007BHouseWithTheSwitchesSetup.cs`
- `Docs/BUILD_LOG.md`

### What Was Implemented

- Added `ShadowPerceptionController` under `Assets/_Project/Code/Shadow` as a minimal reusable perception-state component that activates while `Q` is held and deactivates cleanly when released.
- Kept the `Q` binding explicitly temporary for Wave 008A and documented in code that this path should later route through the Input Actions asset.
- Added reusable `IShadowRevealable` and `ShadowRevealable` types so future scenes can mark Shadow-visible objects without duplicating Wave 007B-specific room logic.
- Replaced the local `Keyboard.current.qKey` polling inside `HouseWithTheSwitchesController` with a dependency on `ShadowPerceptionController` state changes.
- Preserved the existing Wave 007B puzzle behavior: Ego-only switch use destabilizes the room, holding Shadow perception after destabilization reveals the seam, interacting with the same switch while perception is active completes the room, and releasing `Q` before completion hides the seam again.
- Updated `ADITD/Setup/Recreate Wave 007B House With the Switches` so the recreated graybox scene adds `ShadowPerceptionController` to the generated player instance and wires the seam as a reusable `ShadowRevealable`.
- The MVP reveal path uses no added raycast or visibility-query loop, so there is no new per-frame raycast cost for this wave.

### Menu Path

- `ADITD/Setup/Recreate Wave 007B House With the Switches`

### Tool Purpose

- Recreates the dedicated Wave 007B graybox scene from scratch, with the reusable Shadow Perception MVP wired into the generated player and seam reveal object.

### Created or Updated Assets

- Recreates `Assets/_Project/Scenes/Wave007B_HouseWithTheSwitches.unity` when the menu command is run in the Unity Editor.

### Manual Test Steps

1. Open Unity and allow scripts to import.
2. Confirm no Unity Console compile errors after import.
3. Run `ADITD/Setup/Recreate Wave 007B House With the Switches`.
4. Enter Play Mode.
5. Confirm normal first-person movement, look, and interaction still work.
6. Aim at or approach the hidden seam/test crack.
7. Confirm it is hidden while `Q` is not held.
8. Hold `Q` before pressing `E` while the room is still `Unresolved`.
9. Confirm the seam does not reveal while the room is still `Unresolved`.
10. Release `Q`.
11. Press `E` on the ordinary switch without Shadow perception and confirm wrong-form destabilization still works.
12. Hold `Q` after destabilization and confirm the seam/truth becomes visible.
13. Release `Q` before completion and confirm the seam hides again.
14. Hold `Q` again.
15. Confirm Shadow perception activates and the hidden seam/test crack becomes visible or highlighted.
16. While holding `Q`, press `E` on the same switch and confirm the room completes.
17. If feasible from the Inspector during Play Mode, disable `ShadowPerceptionController` and confirm revealables clean up/hide without errors.
18. Stop Play Mode.
19. Confirm no console errors, missing scripts, broken serialized references, or null-reference warnings.

### Manual Unity Validation Steps

1. Run `ADITD/Setup/Recreate Wave 007B House With the Switches`.
2. Select the generated `Player` object and confirm `ShadowPerceptionController` is present.
3. Select `HiddenSeamRoot` and confirm `ShadowRevealable` is present.
4. In `HiddenSeamRoot`'s `ShadowRevealable`, confirm the renderer references under visibility toggling are assigned as intended.
5. Select the root `Wave007B_HouseWithTheSwitches` object and confirm `HouseWithTheSwitchesController` has its `_shadowPerception` reference assigned.
6. Confirm no missing scripts or broken serialized references are present after the setup menu finishes.

### Known Limitations

- Shadow perception is visual-only.
- `Q` binding is still temporary and not yet routed through the Input Actions asset.
- No Shadow Charge.
- No audio treatment.
- No pressure integration.
- No full form switching.
- No split-screen or third-person Shadow proxy.
- No save/load or Hearth interaction.
- Revealable visuals are prototype quality.
- Rerunning `ADITD/Setup/Recreate Wave 007B House With the Switches` rebuilds the Wave 007B graybox scene from scratch and does not preserve manual scene tweaks.
- Headless Unity scene generation is still avoided because of the prior local `Unity.Licensing.Client.exe` exception.

### Rollback Notes

- Delete or revert new files under `Assets/_Project/Code/Shadow`.
- Revert `Assets/_Project/Code/Rooms/HouseWithTheSwitchesController.cs`.
- Revert `Assets/_Project/Code/Editor/Wave007BHouseWithTheSwitchesSetup.cs`.
- Delete or recreate the generated Wave 007B scene if needed.
- Remove the Wave 008A entry from the top of `Docs/BUILD_LOG.md`.

## 2026-05-16 - Wave 007B-FixPass: House With the Switches Audit Fixes

### Summary

Applied a small Wave 007B stabilization pass from Claude's non-blocking audit: added earlier scene-wiring warnings, tightened the editor setup tool's safety and naming, corrected the player spawn height, and clarified in the docs that the Wave 007B scene command recreates the graybox scene from scratch.

### Files Changed

- `Assets/_Project/Code/Rooms/HouseWithTheSwitchesController.cs`
- `Assets/_Project/Code/Interaction/HouseSwitchInteractable.cs`
- `Assets/_Project/Code/Editor/Wave007BHouseWithTheSwitchesSetup.cs`
- `Docs/BUILD_LOG.md`

### What Was Fixed

- Added an early startup warning in `HouseSwitchInteractable.Awake` so missing controller wiring is surfaced during initialization instead of only when the player presses `E`.
- Added simple `OnValidate` warnings in `HouseWithTheSwitchesController` for `_switchRenderer`, `_switchHandle`, and `_completionMarker`.
- Renamed the editor helper method `roomRootAddController` to `AddOrGetController` for consistent PascalCase naming.
- Updated the editor menu command to prompt the user to save modified scenes before replacing the active scene.
- Clarified the editor tool language so it explicitly recreates the Wave 007B graybox scene from scratch and does not imply manual scene edits are preserved.
- Corrected the generated player spawn height from `0.05f` to `0.2f` so the player starts safely above the floor surface.
- Expanded manual test coverage in the build log for unresolved `Q` behavior, repeated destabilized switch use, releasing `Q` from the reveal state, and post-import compile checks.

### Menu Path

- `ADITD/Setup/Recreate Wave 007B House With the Switches`

### Tool Purpose

- Recreates the dedicated Wave 007B graybox scene from scratch, with a save prompt before replacing the current active scene.

### Created or Updated Assets

- Recreates `Assets/_Project/Scenes/Wave007B_HouseWithTheSwitches.unity` when the menu command is run in the Unity Editor.

### Manual Test Steps

1. Open Unity and allow the project to finish importing the Wave 007B scripts.
2. Confirm the Unity Console shows no compile errors after import completes.
3. Make a harmless unsaved scene change, run `ADITD/Setup/Recreate Wave 007B House With the Switches`, and confirm Unity prompts you to save modified scenes before replacing the active scene.
4. Run `ADITD/Setup/Recreate Wave 007B House With the Switches`.
5. Open `Assets/_Project/Scenes/Wave007B_HouseWithTheSwitches.unity` if Unity does not leave it active automatically.
6. Enter Play Mode.
7. Confirm the player starts above the floor and is not partially embedded in it.
8. Hold `Q` before pressing `E` and confirm the hidden seam does not reveal while the room is still `Unresolved`.
9. Look at the ordinary switch and press `E`.
10. Confirm the room gives clear wrong-form feedback through unstable light, visual disturbance, and recoverable state messaging.
11. Press `E` multiple times while the room is `Destabilized` and confirm the wrong-form feedback restarts cleanly instead of leaving the room in a broken state.
12. Hold `Q` and confirm one hidden seam/path/truth becomes visible while Shadow perception is active.
13. Release `Q` while the room is `ShadowRevealed` and confirm the room returns to `Destabilized` and the hidden seam hides again.
14. Hold `Q` again, then interact with the ordinary switch using `E`.
15. Confirm the room reaches a simple completed state with stable light and a visible completion marker.
16. Confirm no console errors, missing scripts, or broken serialized references.

### Known Limitations

- Graybox only.
- Shadow perception uses a temporary local `Q` placeholder instead of a shared Shadow system.
- No full form switching.
- No split-screen or third-person Shadow.
- No real Pressure, Hearth, lore, anchor, save/load, or Focus Memory integration.
- Visual/audio feedback is placeholder quality.
- Rerunning `ADITD/Setup/Recreate Wave 007B House With the Switches` rebuilds the Wave 007B graybox scene from scratch and does not preserve manual scene tweaks.
- Headless Unity scene generation is still avoided because of the prior local `Unity.Licensing.Client.exe` exception.

### Rollback Notes

- Revert `Assets/_Project/Code/Rooms/HouseWithTheSwitchesController.cs`.
- Revert `Assets/_Project/Code/Interaction/HouseSwitchInteractable.cs`.
- Revert `Assets/_Project/Code/Editor/Wave007BHouseWithTheSwitchesSetup.cs`.
- Revert the Wave 007B-FixPass entry at the top of `Docs/BUILD_LOG.md`.

## 2026-05-16 - Wave 007B: House With the Switches Graybox Prototype

### Summary

Implemented a contained Wave 007B graybox prototype path for **House With the Switches** using one scene-local room controller, one ordinary switch interactable, one temporary local Shadow perception placeholder, and one dedicated editor setup command that builds the graybox scene without touching player architecture or shared puzzle frameworks.

### Files Changed

- `Assets/_Project/Code/Rooms/HouseWithTheSwitchesController.cs`
- `Assets/_Project/Code/Interaction/HouseSwitchInteractable.cs`
- `Assets/_Project/Code/Editor/Wave007BHouseWithTheSwitchesSetup.cs`
- `Docs/BUILD_LOG.md`

### What Was Implemented

- Added `HouseWithTheSwitchesController` as a scene-local room state controller with four bounded states: `Unresolved`, `Destabilized`, `ShadowRevealed`, and `Completed`.
- Implemented Ego-only wrong-form feedback on the ordinary switch through reversible light instability, room tint shift, switch feedback, and clear local overlay messaging.
- Implemented a temporary local MVP Shadow perception placeholder on `Q` so no shared Shadow system, player controller rewrite, or input-asset change was required for this wave.
- Implemented one integrated completion action: destabilize the room with the ordinary switch, reveal the hidden seam while holding Shadow perception, then interact with the same switch again to settle the room.
- Added `HouseSwitchInteractable` so the ordinary switch stays explicit and reusable only at the scene level, without moving puzzle behavior into player scripts.
- Added editor tooling at `ADITD/Setup/Recreate Wave 007B House With the Switches` to create or recreate a dedicated graybox scene with:
  - one boxed room
  - one ordinary wall switch
  - one hidden seam/path reveal
  - one completion marker
  - one instantiated player prefab
- Verified the new runtime and editor C# compile successfully with isolated local build outputs outside Unity’s locked temp targets.

### Menu Path

- `ADITD/Setup/Recreate Wave 007B House With the Switches`

### Tool Purpose

- Creates or recreates the dedicated Wave 007B graybox scene without hand-editing Unity YAML and limits scene writes to the Wave 007B scene only.

### Created or Updated Assets

- Creates or recreates `Assets/_Project/Scenes/Wave007B_HouseWithTheSwitches.unity` when the menu command is run in the Unity Editor.

### Manual Test Steps

1. Open Unity and allow the project to finish importing the new Wave 007B scripts.
2. Run `ADITD/Setup/Recreate Wave 007B House With the Switches`.
3. Open `Assets/_Project/Scenes/Wave007B_HouseWithTheSwitches.unity` if Unity does not leave it active automatically.
4. Enter Play Mode.
5. Walk into the room.
6. Look at the ordinary switch and press `E`.
7. Confirm the room gives clear wrong-form feedback through unstable light, visual disturbance, and recoverable state messaging.
8. Hold `Q` to trigger the temporary local Shadow perception placeholder.
9. Confirm one hidden seam/path/truth becomes visible only while Shadow perception is active.
10. While holding `Q`, interact with the ordinary switch again using `E`.
11. Confirm the room reaches a simple completed state with stable light and a visible completion marker.
12. Confirm no console errors, missing scripts, or broken serialized references.

### Manual Unity Validation Steps

1. Select the root `Wave007B_HouseWithTheSwitches` object and confirm `HouseWithTheSwitchesController` is present with assigned light, seam, room renderers, and completion marker references.
2. Select `SwitchHandle` under `OrdinarySwitch` and confirm `HouseSwitchInteractable` is present with the controller reference assigned.
3. Confirm the Player prefab instance exists near the room entrance and uses the existing generic controller/interactor scripts unchanged.
4. Save the scene after validation if the generated layout looks correct.

### Known Limitations

- Graybox only.
- Shadow perception uses a temporary local `Q` placeholder instead of a shared Shadow system.
- No full form switching.
- No split-screen or third-person Shadow.
- No real Pressure, Hearth, lore, anchor, save/load, or Focus Memory integration.
- Visual/audio feedback is placeholder quality.
- Rerunning the menu command recreates the scene from scratch and does not preserve manual scene tweaks.
- Headless Unity scene generation was not used after a local `Unity.Licensing.Client.exe` exception, so the scene should be generated from the normal editor menu instead.

### Rollback Notes

- Delete `Assets/_Project/Code/Rooms/HouseWithTheSwitchesController.cs`.
- Delete `Assets/_Project/Code/Interaction/HouseSwitchInteractable.cs`.
- Delete `Assets/_Project/Code/Editor/Wave007BHouseWithTheSwitchesSetup.cs`.
- Delete or revert `Assets/_Project/Scenes/Wave007B_HouseWithTheSwitches.unity` after it is generated.
- Remove this Wave 007B entry from the top of `Docs/BUILD_LOG.md`.

## 2026-05-16 - Wave 007A Fix Pass: House With the Switches Mechanical Realignment

### Summary

Gemini Notebook reviewed Wave 007A and approved the docs with required mechanical realignment. The room and puzzle specs were updated to remove generic rhythm/sequence framing and explicitly realign the puzzle around Ego / normal interaction, Shadow perception reveal, wrong-form feedback, and one integrated action before Wave 007B graybox work.

### Files Changed

- `Docs/ROOM_SPECS/House_With_The_Switches.md`
- `Docs/PUZZLE_SPECS/House_With_The_Switches.md`
- `Docs/AGENT_REPORTS/Wave007A_Handoff.md`
- `Docs/BUILD_LOG.md`

### What Was Implemented

- Rewrote the room spec so the distortion rule is driven by wrong-self-state interaction rather than timing, sequence, or combination logic.
- Reframed the Shadow reveal around first-person Shadow perception exposing a hidden seam, path, or truth concealed by ordinary glare.
- Updated puzzle beats so wrong-form feedback comes from forced Ego control under surveillance and integrated action requires both Ego stabilization and Shadow reveal.
- Updated the puzzle spec to explicitly name Ego / normal interaction state, Shadow perception state, and a split-self presentation placeholder for later only.
- Replaced rhythm/sequence/combo acceptance language with a bounded one-room, one-switch, one-reveal, one-resolution MVP target for Wave 007B.
- Added a handoff report summarizing Gemini's required realignment and the scope clamp for the next wave.

### Manual Test Steps

1. Open `Docs/ROOM_SPECS/House_With_The_Switches.md` and confirm the `Distortion Rule`, `Shadow Reveal`, `Puzzle Beats`, `Safe Pocket`, and `Manual Test Intention` sections now frame the room around Ego control versus Shadow perception instead of timing or sequence play.
2. Open `Docs/PUZZLE_SPECS/House_With_The_Switches.md` and confirm `Required Systems`, `Wrong-Form Feedback`, `Acceptance Criteria`, `Manual Test Intention`, and `Implementation Notes for the Later Graybox Wave` explicitly bound 007B to one room, one ordinary switch, one localized Shadow reveal, and one integrated action.
3. Confirm neither spec describes the puzzle as rhythm-based, sequence-based, timing-based, combination-based, or circuit-based.
4. Open `Docs/AGENT_REPORTS/Wave007A_Handoff.md` and confirm the `Gemini Required Realignment` section summarizes the approved correction and scope clamp for 007B.
5. Confirm this fix-pass entry appears at the top of `Docs/BUILD_LOG.md`.

### Known Limitations

- This remains a docs-only pass; no gameplay code, Unity scenes, prefabs, materials, assets, or input configuration were changed.
- `Shadow Form` and `Shadow perception` are used here as MVP-facing design language, not as approval for a full controller architecture or split-self presentation system.
- Stable IDs were not changed in this pass.

### Rollback Notes

- Revert `Docs/ROOM_SPECS/House_With_The_Switches.md`.
- Revert `Docs/PUZZLE_SPECS/House_With_The_Switches.md`.
- Delete `Docs/AGENT_REPORTS/Wave007A_Handoff.md`.
- Remove this fix-pass entry from the top of `Docs/BUILD_LOG.md`.

## 2026-05-16 - Wave 007: House With the Switches Spec Skeletons

### Summary

Created initial room and puzzle spec documents for `House With the Switches` using stable IDs, review-first status, and source-safe framing so Gemini Notebook can assess emotional truth before any graybox or implementation work begins.

### Files Changed

- `Docs/ROOM_SPECS/House_With_The_Switches.md`
- `Docs/PUZZLE_SPECS/House_With_The_Switches.md`
- `Docs/BUILD_LOG.md`

### What Was Implemented

- Added room spec skeleton for **House With the Switches** with the requested sections:
  - room title and stable ID
  - status
  - emotional thesis
  - player-facing goal
  - ordinary baseline
  - distortion rule
  - pressure source
  - safe pocket
  - shadow reveal
  - puzzle beats
  - anchor/lore reward placeholder
  - manual test intention
  - parked ideas
- Added puzzle spec skeleton for **House With the Switches** with the requested sections:
  - puzzle title and stable ID
  - room link
  - status
  - emotional movement practiced
  - required systems
  - inputs
  - named puzzle states
  - wrong-form feedback
  - repair path
  - acceptance criteria
  - player clarity notes
  - manual test intention
  - implementation notes for a later graybox wave
- Marked both specs as pending **Gemini Notebook** review before any implementation wave begins.

### Manual Test Steps

1. Open `Docs/ROOM_SPECS/House_With_The_Switches.md` and confirm all requested room-spec headings are present.
2. Open `Docs/PUZZLE_SPECS/House_With_The_Switches.md` and confirm all requested puzzle-spec headings are present.
3. Verify stable IDs follow project conventions:
   - `room.main_floor.house_with_the_switches`
   - `puzzle.house_with_the_switches`
4. Confirm both files explicitly require Gemini Notebook review for emotional truth and source-safety before implementation.
5. Confirm this Wave 007 entry appears at the top of `Docs/BUILD_LOG.md`.

### Known Limitations

- These are draft spec skeletons only; no implementation wave, gameplay wiring, or Unity assets were created.
- Emotional wording is intentionally conservative and may still need refinement once Gemini Notebook reviews continuity and truthfulness.
- Room area placement uses `main_floor` as a provisional stable-ID segment pending any later project-level location decision.

### Rollback Notes

- Delete `Docs/ROOM_SPECS/House_With_The_Switches.md`.
- Delete `Docs/PUZZLE_SPECS/House_With_The_Switches.md`.
- Remove this **Wave 007** entry from the top of `Docs/BUILD_LOG.md`.

## Build Log Rules
1. Newest entries at the top.
2. Editor tooling waves must include:
- menu path
- tool purpose
- created/updated assets
- manual Unity validation steps
- rollback steps



---

## 2026-05-16 - Wave 006B: Edit-mode material leak warning fix

### Summary

Contained fix for the `InspectableDebugObject` editor warning caused by `renderer.material` during `OnValidate`. Edit-mode startup tint now applies through `sharedMaterial`, while Play Mode interaction tint still uses an instance material.

### Files Changed

- `Assets/_Project/Code/Interaction/InspectableDebugObject.cs`
- `Docs/BUILD_LOG.md`

### What Was Implemented

- Added a small shared helper for tint application in `InspectableDebugObject`.
- Changed startup tint application to use `Renderer.sharedMaterial` when not playing, preventing Unity from instantiating leaked scene materials during edit-time validation.
- Kept runtime interact feedback on `Renderer.material` so Play Mode color changes remain instance-local.

### Manual Test Steps

1. In Unity, select `InspectableDebugCube` in `Test_FirstPersonController`.
2. Confirm the Console no longer logs `Instantiating material due to calling renderer.material during edit mode` when the object validates or after rerunning `ADITD/Setup/Wave 004 Test UI`.
3. Enter Play Mode, look at `InspectableDebugCube`, and press `E`.
4. Confirm the object still changes to the interacted tint and the inspection UI behavior is unchanged.

### Known Limitations

- Edit-mode tinting now affects the shared material reference on the renderer, which is appropriate for this scene-local debug object but would need a different approach for authored shared materials intended to stay untouched.

### Rollback Notes

- Revert `Assets/_Project/Code/Interaction/InspectableDebugObject.cs`.
- Remove this build log entry.


---

## 2026-05-15 - Wave 006: Editor Setup Helper for Wave 004 Test UI

### Summary

Editor-only setup command that programmatically builds or rewires legacy `UnityEngine.UI` Canvas prompts and inspection panels in the active scene, assigns `PlayerInteractor` references via `SerializedObject`, and optionally spawns `InspectableDebugCube` when no `IInspectable` exists. Idempotent-by-name reuse; no gameplay, runtime `UnityEditor`, TextMeshPro, or additional tooling.

### Wave 006 Fix Pass Note

Accepted Claude audit fixes applied in a contained pass: removed a dead prompt-layout call, aligned UI view wiring with undoable `SerializedObject.ApplyModifiedProperties()`, added `Undo.RecordObject` coverage for actual `RectTransform` / `Text` / `Image` mutations, and added null-safe `RectTransform` guards so malformed existing named UI objects warn and skip instead of throwing.

### Files Changed

- `Assets/_Project/Code/Editor/Wave004TestSceneSetup.cs`
- `Docs/BUILD_LOG.md`

### Tool Menu Path

`ADITD/Setup/Wave 004 Test UI`

### Tool Purpose

Remove manual Canvas/UI wiring friction for Wave 004 manual tests in `Test_FirstPersonController` (still works if run in another scene with a warning).

### What Was Implemented

- Menu command `Wave004TestSceneSetup` under `ADITD/Setup/Wave 004 Test UI`.
- Active-scene workflow: warns when scene name is not `Test_FirstPersonController`; continues anyway.
- **Canvas:** Finds or creates root `ADITD_TestCanvas` with `Canvas` (`Screen Space Overlay`), `CanvasScaler` (defaults applied when scaler is newly added), and `GraphicRaycaster`.
- **Prompt:** Under Canvas, finds or creates `InteractionPrompt` with `InteractionPromptView`; child `PromptText` with legacy `UnityEngine.UI.Text`; serialized `_root` / `_promptText` wired via `SerializedObject`; prompt starts hidden (`PromptText` inactive).
- **Panel:** Finds or creates `InspectionPanel` with `InspectionPanelView`; child `PanelContents` (`Image` backdrop) toggled by `_panelRoot`; `TitleText` and `BodyText` legacy `Text` components; `_titleText` / `_bodyText` wired; panel starts hidden (`PanelContents` inactive).
- **`PlayerInteractor`:** Locates instances in the **active scene only**; sets `_promptView` and `_inspectionPanel` through serialized properties (fields stay private).
- **Inspectable test prop:** If no `IInspectable` exists, reuses root `InspectableDebugCube` or creates a primitive cube `InspectableDebugCube` with `InspectableDebugObject`; refuses to stack `InspectableDebugObject` on a GameObject that already has another `IInteractable`.
- **Undo grouping** named `ADITD Wave 004 Test UI Setup`; **`EditorSceneManager.MarkSceneDirty`** on the active scene; console logs for created, reused, skipped, and warned steps.

### Created or Updated Assets (Editor Session)

Creates **scene-local** GameObjects in the scene you open (not new `.prefab`/`.asset` disk files unless you save). Expected hierarchy includes `PanelContents` (internal visibility root, not Wave name list) plus the suggested Wave names.

### Manual Test Steps

1. Open `Assets/_Project/Scenes/Test_FirstPersonController.unity`.
2. Run **`ADITD/Setup/Wave 004 Test UI`**.
3. In Hierarchy under `ADITD_TestCanvas`, confirm `InteractionPrompt` / `PromptText`, `InspectionPanel` / `PanelContents` / `TitleText` / `BodyText`.
4. Select the player (or prefab instance) carrying `PlayerInteractor`; confirm **`Interaction Prompt View`** and **`Inspection Panel`** reference fields populate.
5. Run the menu command **again**: confirm **no duplicate** sibling UI objects (`ADITD_TestCanvas`, `InteractionPrompt`, `InspectionPanel` stay single-instance where named).
6. Enter **Play Mode** and verify Wave 004 behavior:
   - **WASD** movement, mouse look unchanged.
   - Aim at **`InspectableDebugCube`** (or any inspectable): **E Inspect** prompt shows; look away → hidden.
   - **E** opens inspection panel; **E** or **Escape** closes it.
7. Inspect **Console** for `[ADITD Wave004 UI Setup]` logs (created / reused / skipped / warnings).

### Known Limitations

- **Existing CanvasScaler:** If `ADITD_TestCanvas` already had a scaler, its **reference resolution / match** values are left as-is (only **newly added** scaler gets Wave defaults).
- **Font:** Uses built-in LegacyRuntime/Arial lookup; Unity may omit in some setups—Console warns; assign a Font in Inspector if text is invisible.
- **Duplicates:** Naming discipline (`ADITD_TestCanvas`, etc.) avoids junk; oddly named clones outside this naming are ignored.
- **Assignment scope:** Finds the **first** `InteractionPromptView` / `InspectionPanelView` under the scene roots; ambiguous multi-UI setups are out of Wave scope.
- **`PanelContents`** is an extra hierarchy node required so `InspectionPanelView` can toggle visibility **without disabling** the host `InspectionPanel` GameObject script.
- No EventSystem is created; Unity may log a missing-EventSystem warning during Play Mode. Manually add an EventSystem to the scene if this is disruptive.

### Rollback Notes

- Delete or disable **`ADITD_TestCanvas`** and **`InspectableDebugCube`** in the scene, clear `PlayerInteractor` UI refs if undesired; save scene—or **Git discard** edits to affected scene file.
- Delete `Assets/_Project/Code/Editor/Wave004TestSceneSetup.cs` (and `.meta` if your workflow tracks Unity-generated `.meta`).
- Remove this **Wave 006** section from **`Docs/BUILD_LOG.md`**.


---

## 2026-05-15 - Wave 005A: Claude audit fixes (rules, docs, raycast cache)

### Summary

Low-risk follow-up before Wave 006: Cursor rule frontmatter on `adid-project.mdc`, Markdown escape fixes in `AGENTS.md`, Wave 001 numbering note in the build log, and per-frame caching of `PlayerInteractor` physics raycasts so prompt update and interact input reuse one result. No gameplay systems, scenes, prefabs, or serialized Unity YAML changed.

### Files Changed

- `.cursor/rules/adid-project.mdc`
- `AGENTS.md`
- `Docs/BUILD_LOG.md`
- `Assets/_Project/Code/Player/PlayerInteractor.cs`

### What Was Implemented

- **`adid-project.mdc`:** YAML frontmatter (`description`, `alwaysApply: true`) so Cursor treats the rule as always-applied reliably.
- **`AGENTS.md`:** Removed stray backslash escapes before `_` in paths so Markdown renders cleanly.
- **Build log / Wave 001:** **Wave 001** was never used as a shipped wave ID (numbering jumps **Wave 000** → **Wave 002**); nothing was retracted—it is an intentionally skipped milestone slot.
- **`PlayerInteractor`:** Invalidate a small frame cache at `Update` start; `TryGetRaycastHit` computes at most once per frame and reuses for `UpdateInteractionPrompt` and `TryInteract` when both run.

### Manual Test Steps

1. **Rules:** Open `.cursor/rules/adid-project.mdc` and confirm the file starts with `---`, `description:`, `alwaysApply: true`, `---`.
2. **Markdown:** Open `AGENTS.md` and confirm paths like `Assets/_Project`, `Docs/BUILD_LOG.md`, and `agent_instructions/CODEX_AUTOMATION_DOCS_SYNC.md` render without visible backslashes before underscores.
3. **Play Mode (Wave 004 parity):** In `Test_FirstPersonController` (or your wired scene): aim at inspectable → **E Inspect** prompt; aim away → hidden; press **E** → panel opens; **E** or **Escape** → closes; inspectable path still fires `Interact` when implemented; plain `DebugInteractable` still shows **E Interact** and responds to **E**; movement/look unchanged.
4. Optional: add a temporary counter in editor-only debug only if validating single raycast (not committed)—behavioral parity with step 3 is sufficient.

### Known Limitations

- Cache is intentionally **per-frame fields** only; no threading or persistence.
- Early returns (no camera, inspection panel open for prompt hide) still skip raycasting where they did before; first `TryGetRaycastHit` on a frame fills the cache.

### Rollback Notes

- Revert the four listed files (or Git restore paths above).
- Remove this build log section.


---

## 2026-05-15 - Wave 005: Documentation and rules alignment

### Summary

Aligned Cursor rules, Claude audit reads, DECISIONS wave records, workflow cross-links, and markdown hygiene so authorization truth lives in `Docs/BUILD_LOG.md` / `Docs/DECISIONS.md` rather than stale rule snapshots. No gameplay or Unity serialized assets changed.

### Files Changed

- `.cursor/rules/adid-architecture-and-waves.mdc`
- `Docs/DECISIONS.md`
- `Docs/CURSOR_UNITY_WORKFLOW.md`
- `Docs/UNITY_EDITOR_TOOLING.md`
- `CLAUDE.md`
- `CODEX_INSTRUCTIONS.md`
- `Docs/BUILD_LOG.md`

### What Was Implemented

- **Architecture rule:** wave status pointers + standing deferred domains; rule file no longer implies latest authorization snapshot.
- **DECISIONS:** single consolidated Wave 003 subsection; Wave 004 unchanged in substance.
- **CLAUDE.md:** normalized markdown paths; audit doc list matches Cursor workflow docs; solo-dev cost-aware audit guidance strengthened.
- **CURSOR_UNITY_WORKFLOW:** Golden rule links to Manual steps vs Editor tooling section.
- **UNITY_EDITOR_TOOLING / CODEX_INSTRUCTIONS:** readable markdown and fixed Codex pointer path.

### Manual Test Steps

1. Open `.cursor/rules/adid-architecture-and-waves.mdc` and confirm it points to `Docs/BUILD_LOG.md` / `Docs/DECISIONS.md` for wave truth (no “Wave 003 is latest authority” wording).
2. Open `Docs/DECISIONS.md` and confirm one Wave 003 block and Wave 004 block exist.
3. Open `CLAUDE.md` and confirm audit doc bullets list `adid-project.mdc`, workflow/editor tooling docs, `BUILD_LOG`, `DECISIONS`, and cost-aware rule.
4. Open `Docs/UNITY_EDITOR_TOOLING.md` and confirm headings/lists render without stray `\` prefixes.

### Known Limitations

- Standing “deferred domains” in the architecture rule remain high-level; exact wave scope still depends on dated entries in `Docs/DECISIONS.md`.

### Rollback Notes

- Revert the files listed above (or restore from Git).
- Remove this build log entry.

---

## 2026-05-15 - Wave 004: Interaction Prompt and Inspect Stub

### Summary

Added minimal UI-driven interaction prompts (`E Inspect` / `E Interact`), an `IInspectable` contract with `InspectableDebugObject` (placeholder title/body), and an inspection panel that opens on Interact and closes on Interact again or Escape. `PlayerInteractor` owns raycast targeting for prompts and routes input without putting inspection rules on `PlayerController`. Serialized Unity assets were not edited in-repo; scene/prefab wiring is manual.

### Files Changed

- `Assets/_Project/Code/Interaction/IInspectable.cs`
- `Assets/_Project/Code/Interaction/InspectableDebugObject.cs`
- `Assets/_Project/Code/UI/InteractionPromptView.cs`
- `Assets/_Project/Code/UI/InspectionPanelView.cs`
- `Assets/_Project/Code/Player/PlayerInteractor.cs`
- `Docs/BUILD_LOG.md`
- Unity will create `.meta` files for new scripts on import (commit if your workflow tracks them).

### What Was Implemented

- **`IInspectable`:** Title/body getters for inspection panel content (no puzzle/lore systems).
- **`InspectableDebugObject`:** Implements `IInspectable` + `IInteractable`; placeholder strings; same style of debug log + material tint as `DebugInteractable` when interacted.
- **`InteractionPromptView`:** Shows/hides a prompt root + optional `UnityEngine.UI.Text`.
- **`InspectionPanelView`:** Shows title/body on a panel root; closes on **Escape** (via Input System keyboard); `Hide()` for interactor-driven close when Interact is pressed while open.
- **`PlayerInteractor`:** Per-frame raycast for prompt visibility; Interact opens panel for `IInspectable`, calls `IInteractable` when present (inspect flow calls interact on the same target for debug feedback); Interact/Escape closes an open panel; uses `GetComponentInParent` so colliders on children still resolve.

### Manual Test Steps

1. In Unity, create a **Canvas** (Screen Space — Overlay) if the test scene has none.
2. Under the Canvas, add an empty GameObject **InteractionPrompt** with a child **UI → Text** (legacy uGUI `UnityEngine.UI.Text`; not TextMeshPro).
   - Position near bottom center; set text to anything (the script overwrites content).
   - Add `InteractionPromptView`; assign **Root** to the object to toggle (often the same as the parent holding the text) and **Prompt Text** to the `Text` component.
3. Under the Canvas, add **InspectionPanel** (full stretch semi-transparent Image optional): child **Title** (`Text`) and **Body** (`Text`, larger area).
   - Add `InspectionPanelView`; assign **Panel Root** (the panel object to enable/disable), **Title Text**, **Body Text**. Start with panel inactive or let Awake hide it.
4. Select the **Player** instance or prefab → `PlayerInteractor`: assign **Prompt View** and **Inspection Panel** references.
5. In `Test_FirstPersonController`, on **DebugInteractableCube** (or your test prop): **remove** `DebugInteractable` and add `InspectableDebugObject` *or* use a separate cube so only one `IInteractable` lives on that object (two interactable components on one GameObject are ambiguous for `GetComponentInParent`).
6. Press **Play**; aim at the inspectable — prompt **E Inspect** appears; look away — hidden.
7. Press **E** — panel shows placeholder title/body; press **E** or **Escape** — panel closes.
8. Confirm **WASD** and **mouse look** still work.
9. Optional: on a second object, keep only `DebugInteractable`; confirm prompt **E Interact** and **E** still tints/logs with no panel.

### Known Limitations

- Movement and look stay **active** while the inspection panel is open (MVP; no pause/input stack).
- Panel close uses **keyboard Escape** only inside `InspectionPanelView`; **Interact/E** to close is handled in `PlayerInteractor`. Gamepad users rely on the same Interact binding to close (no separate Cancel binding).
- Requires **manual Canvas/UI wiring** and Player `PlayerInteractor` references until a later wave edits prefabs/scenes in-repo.
- `UnityEngine.UI.Text` only — no TextMeshPro wiring in these scripts.
- `GetComponentInParent<IInteractable>` / `IInspectable` return a single match; do not stack multiple interactable behaviours on the same GameObject.

### Rollback Notes

- Delete `IInspectable.cs`, `InspectableDebugObject.cs`, `InteractionPromptView.cs`, `InspectionPanelView.cs`, and their `.meta` files.
- Revert `PlayerInteractor.cs` to the Wave 003 version (interaction-only, no prompt/panel fields).
- Remove UI components from the scene and clear serialized references on the Player to avoid missing-script warnings.
- Remove this build log entry.

---

## 2026-05-15 - Wave 003: First-Person Controller MVP

### Summary

Added a generic first-person controller (move, mouse look, interact raycast), minimal interaction contracts, debug interactable, Input System actions asset, player prefab, and graybox test scene. No horror, puzzle, Pressure, Shadow, or Hearth coupling in player scripts.

### Files Changed

- `Assets/_Project/Code/Player/PlayerController.cs`
- `Assets/_Project/Code/Player/PlayerLook.cs`
- `Assets/_Project/Code/Player/PlayerInputActionsLoader.cs`
- `Assets/_Project/Code/Player/PlayerInteractor.cs`
- `Assets/_Project/Code/Player/PlayerContext.cs`
- `Assets/_Project/Code/Interaction/IInteractable.cs`
- `Assets/_Project/Code/Interaction/DebugInteractable.cs`
- `Assets/_Project/Settings/InputActions/Resources/ADITDControls.inputactions` (loads via `Resources.Load("ADITDControls")` when prefab assignment is absent)
- `Assets/_Project/Prefabs/Player/Player.prefab`
- `Assets/_Project/Scenes/Test_FirstPersonController.unity`
- Associated `.meta` files for the above
- `Docs/BUILD_LOG.md`

### What Was Implemented

- **Movement:** `CharacterController`-based WASD movement with simple gravity (`PlayerController`).
- **Look:** Mouse/gamepad look with pitch clamp; cursor hidden and locked during play (`PlayerLook`), restored when the component disables.
- **Interaction:** Camera-forward raycast; `E` / gamepad south button via Input System (`PlayerInteractor`).
- **Contracts:** `IInteractable`, `PlayerContext`, `DebugInteractable` (console log + material color change).
- **Input:** `ADITDControls` asset with `Player` map (`Move`, `Look`, `Interact`). Stored under `InputActions/Resources/` so scripts can bind at runtime via `Resources.Load` if the prefab does not serialize the asset reference.
- **Content:** `Player` prefab and `Test_FirstPersonController` graybox scene (floor + debug cube + player instance).

### Manual Test Steps

1. Open `Assets/_Project/Scenes/Test_FirstPersonController.unity` in Unity 6000.x.
2. Press **Play**.
3. Move with **WASD**; confirm the player stays on the floor and does not fall through.
4. Move the **mouse**; confirm smooth yaw (body) and pitch (camera).
5. Aim at **DebugInteractableCube** and press **E**.
6. Confirm the cube tint changes and the Console logs `[DebugInteractable] Interacted with 'DebugInteractableCube'.`
7. Stop Play; confirm **no compile errors** in the Console.

### Known Limitations

- Movement tuning is MVP-only (no sprint, crouch, or head bob).
- Mouse sensitivity is a single serialized scalar; no in-game settings UI.
- `DebugInteractable` mutates `material.color` (instance material) — not a production interaction pattern.
- Player scripts still prefer an assigned `InputActionAsset`; if `_controls` is null, `PlayerInputActionsLoader` loads `ADITDControls` from `Resources` (asset path must remain `Assets/_Project/Settings/InputActions/Resources/ADITDControls.inputactions`).
- No automated play mode tests in this wave.

### Rollback Notes

- Delete the Wave 003 files listed above (scripts, input actions, prefab, scene, and their `.meta` files).
- Remove this build log entry.
- If scenes break after rollback, remove `Test_FirstPersonController.unity` from Build Settings (if added) and revert to `Assets/Scenes/SampleScene.unity`.

---

## 2026-05-14 - Wave 002: Project Coding Conventions and Guardrails

### Summary

Added Cursor project rules for Unity C# conventions, namespaces, serialization and null-safety expectations, architecture guardrails, and stable ID formats. Documented the same decisions in `Docs/DECISIONS.md`. No gameplay scripts, scenes, or packages were added.

### Files Changed

- `.cursor/rules/adid-architecture-and-waves.mdc` (new)
- `.cursor/rules/adid-csharp-unity.mdc` (new)
- `Docs/DECISIONS.md`
- `Docs/BUILD_LOG.md`

### What Was Implemented

- **Always-on rule:** waves, out-of-scope systems, architecture limits, stable ID table, agent roles.
- **C# rule (when editing `Assets/_Project/**/*.cs`):** namespace ↔ folder table, `[SerializeField]` / `OnValidate`, null-safety logging, general Unity C# guardrails.

### Manual Test Steps

1. In Cursor, open **Settings → Rules** (or the rules panel) and confirm the two **A Door Inside the Dark** rules appear with the expected descriptions.
2. Open any existing `.cs` file under `Assets/` (e.g. tutorial scripts): confirm the C# rule applies only when the path matches `Assets/_Project/**/*.cs` (new project code); adjust globs later if you move third-party code.
3. Read `Docs/DECISIONS.md` § Wave 002 and confirm it matches team intent.

### Known Limitations

- Rules do not enforce compile-time checks; they guide agents and humans. Assembly definitions / analyzers can be added in a later wave if desired.
- Tutorial or non-`_Project` scripts are outside the C# rule glob until moved or the glob is extended.

### Rollback Notes

- Delete `.cursor/rules/adid-architecture-and-waves.mdc` and `.cursor/rules/adid-csharp-unity.mdc`, then remove the **Wave 002** subsection from `Docs/DECISIONS.md` and this build log entry from `Docs/BUILD_LOG.md` (or revert the commit).

---

## 2026-05-14 - Wave 000: Clean Project Foundation

### Summary

Established the `_Project` Unity asset layout, repository documentation folders, agent instruction stubs, Claude audit hook docs, spec templates, and `.gitkeep` placeholders so the repo tracks an empty but intentional structure. No gameplay systems, movement, or puzzle code were added.

### Files Changed

- `Assets/_Project/**` (folders + `.gitkeep`)
- `Docs/**` (this file, `DECISIONS.md`, empty spec subfolders with `.gitkeep`)
- `agent_instructions/**`
- `claude_hooks/**`
- `templates/**`

### What Was Implemented

- Full `Assets/_Project/` tree for art, audio, code namespaces, data, prefabs, scenes, settings, and tests (placeholders only).
- `Docs/` with build log, decisions log, and empty `ROOM_SPECS`, `PUZZLE_SPECS`, `AGENT_REPORTS`, `LORE_ITEMS`, `ART_PROMPTS`.
- Agent-facing markdown in `agent_instructions/` and audit-oriented markdown in `claude_hooks/`.
- Reusable templates under `templates/`.

### Manual Test Steps

1. Open the project in Unity 6000.x and confirm the `_Project` folder appears in the Project window.
2. Confirm no compiler errors (no new C# scripts were added for this wave).
3. Optionally verify Git shows the new paths and `.gitkeep` files as expected.

### Known Limitations

- Folders contain placeholders only; no scenes, prefabs, or ScriptableObjects were created in this wave.
- Unity may generate `.meta` files for new assets on first import; commit those in a follow-up if your workflow requires them.

### Rollback Notes

- Remove `Assets/_Project/`, `Docs/` (except any content you want to keep), `agent_instructions/`, `claude_hooks/`, and `templates/` and revert this commit if the structure needs to be redone.

---
