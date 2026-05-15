# CURSOR_UNITY_WORKFLOW.md - A Door Inside the Dark

## Purpose

This document tells Cursor how to work safely inside the Unity project without damaging scenes, prefabs, serialized assets, or project references.

Cursor is the primary implementation assistant for C# and small Unity waves, but Unity scene, prefab, UI, and asset work must be handled carefully.

## Project baseline

- Engine: Unity 6000.x stable/LTS line
- Rendering: URP
- Language: C#
- Namespace: ADoorInsideTheDark
- Project-owned files live under Assets/_Project
- Docs live under Docs
- Cursor implements small approved waves
- Claude Code audits implementation after the wave when the value justifies the token/cost spend
- Docs/BUILD_LOG.md must be updated at the top after every wave

## Golden rule

Prefer safe C#, documentation, and Unity Editor tooling changes over risky serialized Unity asset edits.

When Unity Editor wiring is required, Cursor should either:

1. Provide exact manual Unity Editor steps, or
2. Create an Editor-only setup tool that performs the wiring through Unity APIs.

Cursor should not guess through serialized YAML edits.

Criteria for choosing manual steps versus an Editor setup command are in **Manual steps vs Editor tooling** (below).

## Files Cursor may usually edit

Cursor may usually edit:

- Assets/_Project/Code/**/*.cs
- Assets/_Project/Code/Editor/**/*.cs
- Assets/_Project/Data script definitions when explicitly requested
- Docs/**/*.md
- .cursor/rules/*.mdc
- README.md if explicitly requested
- Tests under Assets/_Project/Tests or Assets/Tests when the wave asks for tests

## Files Cursor must treat as risky

Do not edit these unless explicitly requested and the change is narrow:

- *.unity
- *.prefab
- *.asset
- *.meta
- *.mat
- *.controller
- *.anim
- *.inputactions
- ProjectSettings/**
- Packages/manifest.json
- Packages/packages-lock.json

If a wave requires one of these files, Cursor must:

1. Explain why the file must change.
2. Keep the change minimal.
3. Preserve existing references.
4. List rollback steps.
5. Call out that Unity should be opened afterward to validate serialization.

## Unity .meta file rules

- Do not delete `.meta` files.
- Do not regenerate `.meta` files casually.
- Do not move or rename Unity assets without preserving their `.meta` files.
- If creating a new C# script, allow Unity to generate the `.meta` file unless the workflow explicitly requires otherwise.
- If rolling back newly created files, delete the paired `.meta` files only for files created in the same wave.

## Scene and prefab workflow

For most waves, Cursor should create scripts and then provide Unity Editor wiring steps.

Example:

1. Create `Assets/_Project/Code/Interaction/IInteractable.cs`.
2. Create `Assets/_Project/Code/Player/PlayerInteractor.cs`.
3. In Unity, attach `PlayerInteractor` to the Player prefab.
4. Assign the camera reference in the inspector.
5. Create a cube in the test scene.
6. Attach `DebugInteractable`.
7. Press Play and test interaction.

Cursor should not assume scene references are wired unless it created or verified them.

## Programmatic Unity asset creation

For Unity scenes, prefabs, UI, ScriptableObjects, and test assets, Cursor should prefer Editor setup scripts over raw serialized file edits.

Good:

- Create `Assets/_Project/Code/Editor/Wave004TestSceneSetup.cs`.
- Add a menu command such as `ADITD/Setup/Wave 004 Test UI`.
- Use Unity Editor APIs to create Canvas, UI Text, panels, test objects, prefab assets, or ScriptableObject assets.
- Let Unity generate `.meta` files.
- Mark scenes/assets dirty through proper Unity Editor APIs when needed.
- Provide manual validation steps after the setup command runs.

Avoid:

- Hand-editing `.unity` YAML.
- Hand-editing `.prefab` YAML.
- Guessing serialized object references.
- Deleting or regenerating `.meta` files.
- Creating runtime manager systems just to solve editor setup.

Editor setup scripts must:

- Live under `Assets/_Project/Code/Editor`.
- Be safe to run more than once when practical.
- Use clear object names.
- Log what was created, reused, skipped, or could not be wired.
- Avoid changing unrelated scene objects.
- Be removable after the wave if they are temporary scaffolding.
- Never create runtime dependencies on `UnityEditor`.

## Player controller boundary

Player scripts may own:

- Movement
- Look
- Interact raycast
- Input routing
- PlayerContext
- Basic physical state

Player scripts must not own:

- Door puzzle rules
- Shadow knot logic
- Pressure calculations
- Lore state
- Hearth state
- Save/load orchestration
- Room-specific puzzle behavior
- UI panel behavior beyond routing interaction state to assigned scene-local UI components

If a feature seems to require adding puzzle behavior to the player, stop and create a scene-local room, puzzle, interactable, or UI component instead.

## Architecture guardrails

- Build only the approved wave.
- Keep systems small and testable.
- Prefer ScriptableObject definitions for data.
- Prefer scene-local puzzle controllers until reuse is proven by at least two real rooms.
- Avoid singletons unless explicitly approved and documented in Docs/DECISIONS.md.
- Do not introduce a global framework just because it seems cleaner.
- Do not add combat, enemy AI, procedural generation, split-screen, full inventory, or full save/load before the MVP loop proves itself.
- Do not create service locators, registries, global event buses, or framework abstractions unless the wave explicitly requires them.

## Before changing architecture

Before introducing a manager, service, singleton, registry, framework, or reusable abstraction, answer:

1. Does the current wave require this to become playable?
2. Can this be scene-local for now?
3. Has the pattern appeared in at least two real rooms?
4. Is the decision documented in Docs/DECISIONS.md?

If the answer is no, do not add the abstraction.

## Manual steps vs Editor tooling

Use manual Unity Editor steps when:
- The wiring is one-time and simple.
- Only one or two inspector references are needed.
- The setup is unlikely to be repeated.
- The risk of creating tooling is higher than the setup itself.

Use Editor tooling when:
- The setup is repetitive.
- Multiple GameObjects, UI objects, prefabs, or ScriptableObjects must be created.
- The setup is error-prone by hand.
- The same test scene needs to be repaired or regenerated.
- Future waves will benefit from the same creation pattern.

Do not hand-edit serialized Unity YAML as a shortcut.

## Cursor output format after every wave

Cursor must return:

1. Summary
2. Files changed
3. Unity Editor steps
4. Manual test steps
5. Known limitations
6. Rollback notes
7. Build log entry added or updated

If the wave creates Editor tooling, Cursor must also return:

1. Tool name
2. Unity menu path
3. What the tool creates or updates
4. What the tool reuses or skips
5. How to run the tool in Unity

## Required build log format

Update `Docs/BUILD_LOG.md` at the top.

Each entry must include:

- Date
- Wave ID and title
- Summary
- Files changed
- What was implemented
- Manual test steps
- Known limitations
- Rollback notes

Editor tooling waves must also include:

- Menu path
- Tool purpose
- Created or updated assets
- Manual Unity validation steps
- Rollback steps

## Compile and validation expectations

Cursor should attempt to keep C# compile-safe by:

- Using the `ADoorInsideTheDark` namespace
- Keeping serialized fields private with `[SerializeField]`
- Avoiding scene lookups when references can be assigned
- Validating null risks
- Avoiding Unity API calls from constructors
- Avoiding editor-only APIs in runtime scripts
- Keeping MonoBehaviours small
- Avoiding hidden dependencies
- Keeping runtime scripts free of `UnityEditor` references
- Placing Editor-only scripts under an `Editor` folder

## When blocked

If Cursor cannot safely complete a Unity step through runtime code or an Editor setup tool, it should stop and provide:

- The exact reason
- The safest manual Unity Editor steps
- What file or object Matt should inspect
- What to report back after testing

Do not guess through serialized asset edits.

## Between-wave checkpoint

After each wave, before starting the next implementation wave, pause and decide:

1. Is a Claude audit worth doing for this wave?
2. Do we need Gemini or Gemini Notebook for lore drift, puzzle advice, story architecture, mapping, or continuity?
3. Do we need Grok to critique scope, player clarity, production risk, or design drift?
4. Does the value of the agent pass justify the token/cost spend for a solo developer?

Use audits and critique passes when risk is meaningful. Skip them for low-risk infrastructure waves that compile, pass manual tests, and do not touch lore, puzzle meaning, architecture, or broad Unity serialization.