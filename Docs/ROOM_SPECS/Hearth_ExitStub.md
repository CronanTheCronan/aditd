# Room Spec: Hearth Exit Stub

Stable ID:
room.hearth.exit_stub

Status:
Ready for implementation

## Purpose
Define the smallest post-placement Hearth exit affordance so the player can recognize there is a forward path after the Green Thermos return beat.
Keep the moment local, readable, and explicitly non-transitioning.

## Prerequisite
- The Green Thermos has already been placed on the Hearth shelf in the minimal return flow.
- The existing placement confirmation beat is allowed to complete or fade before the exit prompt becomes active.

## Observable Player Sequence
1. The Green Thermos has already been placed on the shelf and the existing confirmation beat completes or fades.
2. After that beat settles, one single exit or door affordance becomes available in the Hearth space.
3. The player approaches that door area and sees the prompt `E - Next Door`.
4. While near the door, the player receives only a minimal visual and/or audio cue that another chamber exists beyond it.
5. Pressing `E` acknowledges the stub locally and gives only a small forward-facing or "not yet" response.
6. The room does not transition, load, save, or generate any new destination, and the player remains in the current Hearth space.

## Prompt and Feedback
- Prompt text: `E - Next Door`.
- The prompt appears only after the Green Thermos placement beat has completed or faded.
- Approach feedback may be a subtle light change, quiet door response, or similarly minimal cue that implies continuation.
- Interaction feedback must stay local and restrained, confirming forward possibility without opening, transporting, or loading anything.

## Tone & Constraints
- Keep the moment quiet, grounded, and forward-looking.
- Do not frame it as triumph, reward, chapter select, or mission complete.
- No lore exposition.
- No new emotional monologue.
- The door should feel like permission to continue, not pressure to perform.

## Non-Goals
- No scene transition or level loading.
- No save/load or persistence.
- No full Hearth progression or multiple exits.
- No anchor inventory or chapter select.
- No new controllers, prompts, or `PlayerInteractor` changes.
- No lore prose or emotional exposition beyond the already-approved Green Thermos confirmation beat.
- No assumptions about the next chamber's identity.

## Acceptance Criteria
- The spec file exists at `Docs/ROOM_SPECS/Hearth_ExitStub.md`.
- It contains a numbered Observable Player Sequence with at least 4 steps.
- It explicitly states the prompt text, when it appears, and what happens on approach.
- It explicitly states there is no scene transition or level loading.
- It includes a Tone & Constraints section.
- Total file length stays under 1.5 pages.
- No implementation details, code snippets, or future-wave assumptions.

## Manual Test Intention
1. Open `Docs/ROOM_SPECS/Hearth_ExitStub.md`.
2. Read the Observable Player Sequence section.
3. Confirm every step is concrete and testable.
4. Verify the spec contains the required Tone & Constraints section.
5. Confirm there are no prohibited elements: scene transition, save/load, inventory, chapter select, multiple exits, new controllers, or `PlayerInteractor` changes.
6. Confirm the file can be read in under two minutes and the next-door behavior is unambiguous.

## Parked Ideas
- A later wave may decide what exact local acknowledgment text or cue best communicates "forward, but not yet."
- A later implementation wave may decide whether the cue is primarily visual, primarily audio, or a restrained combination of both.
