\# Wave 003 Change Request - First-Person Controller MVP



Date: 2026-05-15  

Status: Proposed  

Owner: Matt  

Primary builder: Cursor  

Audit: Claude Code  



\## Goal



Create a generic first-person controller MVP that allows the player to move, look, and interact with a debug object in a minimal graybox test scene.



\## Why This Wave Exists



The project needs a body in the room before Pressure, Shadow perception, Hearth, lore inspection, or puzzle logic can be tested. This wave proves basic embodiment only.



\## In Scope



\- Create a generic first-person player controller.

\- Create mouse/controller look behavior if feasible within Unity Input System.

\- Create an interaction raycast from the camera.

\- Create a minimal `IInteractable` contract.

\- Create a debug interactable cube or object.

\- Create or configure a minimal Input Actions asset with:

&#x20; - Move

&#x20; - Look

&#x20; - Interact

\- Create a minimal test scene under `Assets/\_Project/Scenes`.

\- Create player prefab if practical.

\- Update `Docs/BUILD\_LOG.md` at the top.



\## Out of Scope



\- Pressure.

\- Shadow perception.

\- Shadow Charge.

\- Hearth.

\- Inventory.

\- Save/load.

\- Focus Memory.

\- Room-specific puzzle logic.

\- Enemy AI.

\- Combat.

\- Procedural systems.

\- Cinematics.

\- Real lore content.

\- Horror-specific coupling in player scripts.



\## Architecture Constraints



\- Namespace root: `ADoorInsideTheDark`.

\- Player code belongs under `Assets/\_Project/Code/Player`.

\- Interaction contracts belong under `Assets/\_Project/Code/Interaction`.

\- Input assets belong under `Assets/\_Project/Settings/InputActions`.

\- Scenes belong under `Assets/\_Project/Scenes`.

\- Keep player scripts generic.

\- No singletons.

\- No global service locator.

\- No puzzle logic in player scripts.

\- Interaction should pass a `PlayerContext` or equivalent simple context object if needed.



\## Proposed Files



Likely create or modify:



\- `Assets/\_Project/Code/Player/PlayerController.cs`

\- `Assets/\_Project/Code/Player/PlayerLook.cs`

\- `Assets/\_Project/Code/Player/PlayerInteractor.cs`

\- `Assets/\_Project/Code/Player/PlayerContext.cs`

\- `Assets/\_Project/Code/Interaction/IInteractable.cs`

\- `Assets/\_Project/Code/Interaction/DebugInteractable.cs`

\- `Assets/\_Project/Settings/InputActions/ADITDControls.inputactions`

\- `Assets/\_Project/Prefabs/Player/Player.prefab`

\- `Assets/\_Project/Scenes/Test\_FirstPersonController.unity`

\- `Docs/BUILD\_LOG.md`



Cursor may adjust exact file names if Unity conventions require it, but must keep the scope equivalent.



\## Input Actions Spec



Create an Input Actions asset named `ADITDControls`.



Suggested action map: `Player`



Actions:

\- `Move`

&#x20; - Type: Value

&#x20; - Control Type: Vector2

&#x20; - WASD or left stick.

\- `Look`

&#x20; - Type: Value

&#x20; - Control Type: Vector2

&#x20; - Mouse delta or right stick.

\- `Interact`

&#x20; - Type: Button

&#x20; - Keyboard E and/or gamepad south button.



Implementation may use generated C# wrappers or `InputActionReference`, whichever is simplest and clearest for this MVP.



\## Acceptance Criteria



\- Unity opens without compile errors.

\- Player can walk around a graybox test scene.

\- Player can look around smoothly with mouse.

\- Player cannot move through the floor.

\- Interaction raycast detects a debug interactable object.

\- Pressing Interact triggers visible debug feedback, such as a console log or simple object color/state change.

\- Player scripts contain no Pressure, Shadow, Hearth, lore, save/load, or puzzle-specific logic.

\- `Docs/BUILD\_LOG.md` is updated at the top with manual test steps, limitations, and rollback notes.



\## Manual Test Steps



1\. Open `Assets/\_Project/Scenes/Test\_FirstPersonController.unity`.

2\. Press Play.

3\. Use WASD to move.

4\. Use mouse to look.

5\. Aim at the debug interactable cube.

6\. Press E.

7\. Confirm interaction feedback occurs.

8\. Confirm the Console has no errors.



\## Known Risks



\- Unity Input System setup may require editor-side binding work that Cursor cannot fully validate.

\- Prefab/scene serialization can be fragile. Claude must audit scene references after implementation.

\- If the controller uses `CharacterController`, movement tuning may need later polish.



\## Rollback Plan



Remove the files created for Wave 003 and revert the `Docs/BUILD\_LOG.md` entry. If scene or prefab references break, delete the test scene and player prefab and recreate them in a later wave.

