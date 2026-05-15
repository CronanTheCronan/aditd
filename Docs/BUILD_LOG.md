# Build Log

## Build Log Rules
1. Newest entries at the top.
2. Editor tooling waves must include:
- menu path
- tool purpose
- created/updated assets
- manual Unity validation steps
- rollback steps



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
