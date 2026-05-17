# Puzzle Spec: Green Thermos Return

- Puzzle title: `Green Thermos Return`
- Stable ID: `puzzle.green_thermos_return`

## Room

`room.hearth.minimal_return`

## Status

Ready for implementation

## Continuity Reference

This puzzle is the smallest approved follow-up to `room.main_floor.weight_of_door`, using the Green Thermos as the first explicit recovered-anchor return action before any larger Hearth interlude logic is introduced.

## Purpose

Validate the smallest possible claim-and-place return loop for the Green Thermos without introducing inventory, save/load, or broader Hearth progression.

## Required Inputs

- `E` to claim the Green Thermos after `Weight of the Door` completion.
- `E` to place the Green Thermos on the one highlighted Hearth shelf slot.

## Local State Only

- `weightOfDoorCompleted` gate or equivalent room-entry prerequisite.
- `isCarryingGreenThermos` boolean.
- `greenThermosPlaced` boolean.
- No inventory list, stack, item database requirement, or persistent save state.

## Puzzle Sequence

1. The loop becomes available only after `Weight of the Door` is already complete.
2. The player approaches the Green Thermos and uses `E` to claim it.
3. Claiming the thermos sets `isCarryingGreenThermos = true` and makes one Hearth shelf slot visibly valid.
4. While the thermos is carried, the player can approach the highlighted slot and see `E - Place Anchor`.
5. Pressing `E` at the slot clears `isCarryingGreenThermos`, sets `greenThermosPlaced = true`, shows the thermos on the shelf, brightens the fire one notch, and plays one quiet confirmation line.

## Interaction Rules

- The Green Thermos is either unclaimed, carried via local boolean state, or placed.
- Carried state is represented through prompts and placement eligibility only.
- Do not parent a thermos mesh to the player camera, hands, or body.
- Do not create carrying animations, rigging, inventory UI, or camera-stack behavior.
- The shelf slot should reject placement unless `isCarryingGreenThermos` is true.
- After placement, the thermos cannot be duplicated or routed into other systems in this wave.

## Prompts and Feedback

- Claim prompt: `E - Claim Thermos`
- Carry-state feedback: `Carrying: Green Thermos`
- Placement prompt: `E - Place Anchor`
- Completion feedback: one quiet confirmation line plus one visible Hearth fire-brightening notch

## Emotional/Tone Constraints

- The Green Thermos should read as quiet, contradictory, and dignified.
- The Hearth return should feel like practiced safety and holding truth, not reward display.
- Placement should read as relief and grounding, not obedience or punishment.
- Confirmation text must remain symbolic and non-biographical.

## Confirmation Text Options

Use exactly one placeholder line:

- `An ordinary capacity to endure, held in place.`
- `The weight of preparation settles here.`
- `Proof of what survived, kept safe from the static.`

## Non-Goals

- No inventory system.
- No save/load or persistence.
- No full Hearth progression tree.
- No Road to Cabin follow-up.
- No second room or second anchor.
- No global architecture or manager layer.
- No player-controller changes.
- No hand-held object visuals or inspect mode.

## Acceptance Criteria

- The puzzle can be explained as one sentence: claim the Green Thermos with `E`, carry it through a local boolean state, and place it on one highlighted Hearth shelf slot with `E`.
- The claim step is unavailable or irrelevant before `Weight of the Door` completion.
- Carrying the thermos requires only local boolean state and prompt/slot feedback.
- Exactly one placement slot is valid in this wave.
- Placement produces the thermos placed-state mesh, one fire-brightening notch, and one quiet confirmation line.
- The spec does not require any inventory UI, persistence layer, carrying rig, or multi-room behavior.

## Player Clarity Risks

- If the claim prompt is too understated, players may not realize the thermos is interactable after the door room resolves.
- If the carry-state feedback is too abstract, players may not understand why the Hearth slot is now active.
- If the slot highlight persists when not carrying the thermos, the loop may read as broken or pre-solved.
- If the confirmation line sounds triumphant, the tone will drift away from grounding and toward trophy-room framing.

## Source-Safety Notes

- Keep the thermos symbolic and domestic, not biographical exposition.
- Do not frame the placement as proof against a named person or accusation.
- Avoid punitive, shame-based, or score-like feedback.
- Keep the return gesture contained, quiet, and reversible in implementation scope.

## Manual Test Intention

- This spec should be implementable as a room-local graybox follow-up with no dependency on global inventory or Hearth systems.
- Review should confirm the loop reads as grounded return and placement, not object collection or achievement display.

## Parked Ideas

- A later wave may decide whether the placed thermos becomes inspectable inside the Hearth.
- A later Hearth pass may define how multiple returned anchors sequence without changing this first boolean-only loop.
- Future polish may replace text prompts with subtler presentation once the baseline interaction is proven.
