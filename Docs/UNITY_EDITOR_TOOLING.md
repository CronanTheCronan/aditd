\# UNITY\_EDITOR\_TOOLING.md - A Door Inside the Dark



\## Purpose



This document defines how Cursor, Codex, or Claude may create Unity assets programmatically without risky hand-edits to serialized Unity files.



The goal is to let the IDE generate common Unity objects safely through Unity Editor APIs:

\- folders

\- scenes

\- prefabs

\- Canvas/UI objects

\- ScriptableObject assets

\- test props

\- debug objects

\- scene wiring

\- validation helpers



This reduces manual clicking while avoiding blind edits to `.unity`, `.prefab`, `.asset`, and `.meta` files.



\## Core rule



Do not hand-edit serialized Unity YAML when a Unity Editor setup script can do the job safely.



Prefer:



`Assets/\_Project/Code/Editor/\[ToolName].cs`



with a menu command:



`ADITD/Setup/\[Tool Name]`



\## Where editor tools live



All editor-only scripts must live under:



`Assets/\_Project/Code/Editor`



Do not place editor scripts in runtime folders.



Runtime code must not depend on editor-only code.



\## Recommended menu structure



Use the `ADITD` top-level menu.



Examples:



\- `ADITD/Setup/Wave 004 Test UI`

\- `ADITD/Create/Player Prefab`

\- `ADITD/Create/Lore Item Definition`

\- `ADITD/Create/Pressure Profile`

\- `ADITD/Validate/Scene References`

\- `ADITD/Validate/Project Folders`

\- `ADITD/Tools/Clean Missing References`



\## Good uses for Editor tooling



Use Editor tooling for:

\- Creating a Canvas and UI panel hierarchy.

\- Creating test scenes.

\- Creating debug cubes or test interactables.

\- Assigning scene-local references.

\- Creating prefab assets from generated GameObjects.

\- Creating ScriptableObject assets.

\- Creating standard project folders.

\- Validating required project structure.

\- Validating missing references.

\- Creating temporary wave setup helpers.



\## Bad uses for Editor tooling



Do not use Editor tooling to:

\- Hide runtime dependencies.

\- Build global gameplay frameworks.

\- Auto-generate final creative content.

\- Rewrite unrelated scenes.

\- Overwrite hand-authored prefabs without confirmation.

\- Replace manual creative judgment.

\- Create large systems outside the approved wave.



\## Idempotency rule



Setup commands should be safe to run more than once where practical.



Prefer:

\- Find existing object by name.

\- Reuse if compatible.

\- Create only if missing.

\- Log if something exists but cannot be safely modified.



Avoid:

\- Creating duplicate Canvas objects every run.

\- Creating endless duplicate UI panels.

\- Overwriting inspector values silently.

\- Renaming or deleting user-authored objects unexpectedly.



\## Asset and scene safety



When creating scene objects:

\- Use clear names.

\- Register Undo where appropriate.

\- Keep changes scoped to the current open scene unless the command clearly states otherwise.

\- Mark the scene dirty through proper Unity Editor APIs when needed.

\- Log the affected scene.



When creating assets:

\- Create folders first if missing.

\- Use stable asset paths under `Assets/\_Project`.

\- Do not overwrite existing assets unless the command explicitly says it is safe.

\- Let Unity generate `.meta` files.

\- Refresh the AssetDatabase after asset creation.



When creating prefabs:

\- Prefer `PrefabUtility.SaveAsPrefabAsset` or `SaveAsPrefabAssetAndConnect`.

\- Do not hand-edit prefab YAML.

\- Do not modify unrelated prefab instances.

\- Log the prefab path.



\## ScriptableObject asset pattern



ScriptableObject assets should live under:



\- `Assets/\_Project/Data/LoreItems`

\- `Assets/\_Project/Data/Anchors`

\- `Assets/\_Project/Data/PressureProfiles`

\- `Assets/\_Project/Data/Rooms`

\- `Assets/\_Project/Data/PuzzleSpecs`



Use stable IDs, for example:



\- `lore.main\_floor.green\_thermos`

\- `anchor.main\_floor.green\_thermos`

\- `pressure.static.father\_office`

\- `room.main\_floor.weight\_of\_door`

\- `puzzle.main\_floor.weight\_of\_door`



\## Validation helpers



Prefer small validation menu commands for project health.



Examples:

\- Validate required folders exist.

\- Validate player prefab has required components.

\- Validate scene has exactly one Player.

\- Validate UI references are assigned.

\- Validate no missing scripts in the active scene.

\- Validate ScriptableObject IDs are non-empty.

\- Validate duplicate IDs are not present.



\## Output requirement for any Editor tooling wave



After creating or editing an Editor tool, return:



1\. Tool name

2\. Menu path

3\. Files changed

4\. What the tool creates or updates

5\. What the tool refuses to overwrite

6\. How to run it in Unity

7\. Manual test steps

8\. Known limitations

9\. Rollback notes

10\. Whether `Docs/BUILD\_LOG.md` was updated at the top



\## PowerShell file opening preference



When instructing Matt to open documentation files on Windows, prefer PowerShell with `notepad`.



Examples:



`notepad Docs\\UNITY\_EDITOR\_TOOLING.md`



`notepad Docs\\CURSOR\_UNITY\_WORKFLOW.md`



`notepad .cursor\\rules\\adid-project.mdc`

