# Wave 013B Retry Constraints: Bus of Names Micro-Room

## 1. Purpose

This brief preserves the lesson from the failed Wave 013B runtime attempt and constrains the next implementation retry so the Bus of Names micro-room can be resumed later in smaller, safer slices without repeating the earlier scope expansion.

## 2. Last Verified Good State

- Hearth Green Thermos return loop works.
- Hearth `E - Next Door` stub works locally.
- Bus of Names exists as docs only.
- No Bus of Names runtime implementation should currently exist.

## 3. What Went Wrong

- The attempted Wave 013B runtime implementation was too large for a single pass.
- The build broke.
- The unverified runtime scripts and generated scene were removed.
- Future implementation should be split into smaller, independently verifiable slices.

## 4. Retry Principle

- Do not implement the full micro-room in one wave.
- Do not touch `PlayerInteractor`, Shadow core, Input Actions, Hearth code, `ProjectSettings`, `Packages`, or existing verified scenes.
- Prefer standalone scene generation and scene-local room scripts only when runtime work resumes.
- Each slice must compile and be manually verified before the next slice begins.
- If a slice breaks compile, rollback that slice immediately instead of fixing forward into broader systems.

## 5. Proposed Retry Slices

### 013B-1: Scene Skeleton Only

Goal:
- Create the standalone graybox Bus of Names scene and editor setup only.
- No interactions.
- No Shadow reveal.
- No wrong-form feedback.
- No new gameplay beyond walking through the scene.

Expected contents:
- Small bus/bedroom threshold room.
- Stable entrance frame.
- One blocked forward threshold.
- One visible placeholder false label object.
- One placeholder hidden knot object may exist but should remain inactive/unused.

### 013B-2: False Label Wrong-Form Cue

Goal:
- Add only the false label interaction and restrained local wrong-form feedback.
- No Shadow reveal yet.
- No room resolution.
- No path opening.

Expected contents:
- One interactable false label.
- Prompt such as `E - Pull Label`.
- Local cue when forced: jitter, light flicker, low hum placeholder, label pulling taut, or threshold nudge.
- Local-only counters/timers allowed.
- No shared systems.

### 013B-3: Shadow Knot Reveal

Goal:
- Add only the Shadow-visible knot/bind reveal using the existing Shadow reveal pattern.
- No new Shadow input.
- No Shadow architecture changes.
- No room resolution.

Expected contents:
- One knot/bind/string/tension object visible only during Shadow perception.
- Objective debug/reveal text only if needed.
- No therapeutic explanation.
- Threshold remains blocked.

## 6. Allowed Files by Slice

For `013B-1`, allowed:
- `Assets/_Project/Code/Editor/Wave013BSceneSkeletonSetup.cs`
- `Assets/_Project/Scenes/Wave013B_BusOfNamesMicroRoom.unity`
- `Docs/BUILD_LOG.md`

For `013B-2`, allowed:
- `Assets/_Project/Code/Rooms/BusOfNamesFalseLabelInteractable.cs`
- `Assets/_Project/Code/Rooms/BusOfNamesMicroRoomController.cs`
- `Assets/_Project/Code/Editor/Wave013BSceneSkeletonSetup.cs`
- `Docs/BUILD_LOG.md`

For `013B-3`, allowed:
- `Assets/_Project/Code/Rooms/BusOfNamesMicroRoomController.cs`
- `Assets/_Project/Code/Editor/Wave013BSceneSkeletonSetup.cs`
- Existing scene file: `Assets/_Project/Scenes/Wave013B_BusOfNamesMicroRoom.unity`
- `Docs/BUILD_LOG.md`

Note:
- Any need to touch files outside the allowed list must stop the wave and ask Matt before proceeding.

## 7. Explicit Do-Not-Touch List

- `Assets/_Project/Code/Player/PlayerInteractor.cs`
- `Assets/_Project/Code/Interaction/IInteractionPromptProvider.cs`
- `Assets/_Project/Code/Shadow/ShadowPerceptionController.cs`
- `Assets/_Project/Code/Shadow/ShadowRevealable.cs`
- `Assets/_Project/Settings/InputActions/Resources/ADITDControls.inputactions`
- `Assets/_Project/Code/Hearth/*`
- `ProjectSettings/*`
- `Packages/*`
- Any verified Hearth scene or setup unless Matt explicitly approves

## 8. Acceptance Gates Before Moving Past Each Slice

For each slice:
- Unity opens without compile errors.
- The generated scene opens.
- Play Mode can start.
- Existing Hearth scene can still be recreated and manually verified.
- No protected files were modified.
- `Docs/BUILD_LOG.md` has a top entry with changed files, manual test steps, known limitations, and rollback notes.
- Matt confirms the slice before the next slice begins.

## 9. Manual Verification Checklist

1. Run `git status --short`.
2. Confirm only approved files changed.
3. Open Unity.
4. Confirm no compile errors.
5. Run the relevant setup menu command for the slice.
6. Open the generated Bus of Names scene.
7. Enter Play Mode.
8. Verify only the slice's intended behavior exists.
9. Re-run the Hearth setup command.
10. Verify the Hearth Green Thermos + Next Door loop still works.
11. Confirm no scene transition, save/load, inventory, chapter routing, or global manager was introduced.

## 10. Parked Until Later

- Full Bus of Names puzzle resolution.
- Removing or loosening the false label.
- Opening the blocked threshold.
- Connecting Hearth `E - Next Door` to the Bus scene.
- Save/load.
- Chapter routing.
- Multiple labels.
- Audio polish.
- Art polish.
- Lore prose.
- Full Upstairs chapter content.
