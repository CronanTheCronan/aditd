# Room Spec: Hearth Minimal Return

Stable ID:
room.hearth.minimal_return

Status:
Ready for implementation

## Continuity Reference
- This loop establishes the primitive technical baseline for the Episode 1 Hearth interludes, satisfying the global narrative requirement that the first recovered object must be explicitly held through a localized placement action before subsequent consciousness strata can open.

## Purpose
Provide the smallest Hearth return container needed to validate object return after `Weight of the Door`.
Confirm that one recovered anchor can be carried through a room-local placement step with calm readable feedback.

## Observable Player Sequence
1. The player enters or returns to a minimal Hearth graybox after `Weight of the Door` is complete.
2. The Green Thermos can be claimed with `E`, setting a local carried boolean state and showing a carry-state prompt.
3. While the thermos is carried, one Hearth shelf slot becomes visibly highlighted and presents `E - Place Anchor`.
4. Pressing `E` at the highlighted slot places the Green Thermos on the shelf and clears the local carried state.
5. The Hearth fire brightens one notch and one quiet confirmation line appears, then the room remains stable with no further progression logic.

## Graybox Implementation Notes
- Player starts in or enters a minimal Hearth graybox room.
- One Hearth shelf slot exists and is visually highlighted only while the Green Thermos is carried.
- Prompt near slot: `E - Place Anchor`.
- On placement, thermos appears on shelf.
- Hearth fire brightens one notch.
- One quiet confirmation line appears.
- No inventory UI.
- No multiple anchors.
- No save/load.
- No full Hearth progression.
- No final lore prose.

## Required Objects
- Hearth fire placeholder.
- One highlighted shelf / placement slot.
- Green Thermos placed-state mesh.
- Minimal debug overlay or prompt text.

## Prompts and Feedback
- Prompt list: `E - Claim Thermos`, `Carrying: Green Thermos`, `E - Place Anchor`.
- Feedback list: shelf highlight appears only while carried; thermos becomes visible on shelf on placement; Hearth fire brightens one notch; one confirmation line appears once.

## Confirmation Text Options
Use only one placeholder line from the approved options:
- `An ordinary capacity to endure, held in place.`
- `The weight of preparation settles here.`
- `Proof of what survived, kept safe from the static.`

## Player Clarity Risks
- Players may not understand the thermos has been claimed if the carried-state feedback is too subtle.
- Players may search for multiple valid shelf targets if the single active slot is not clearly highlighted.
- Players may read the placement as inventory storage or score tracking if the prompt language becomes too systemic.
- Players may miss the fire-brightening change if the notch increase is too faint to register.

## Source-Safety Notes
- No real names.
- No direct biographical exposition.
- No blame-focused text.
- No trophy-room framing.

## Non-Goals
- No full Hearth onboarding flow.
- No general anchor inventory.
- No carried-object hand or camera presentation.
- No second shelf slot or multi-anchor routing.
- No save/load or persistence.
- No Road to Cabin, second room, or broader interlude architecture.
- No final narrative monologue or biography-facing lore delivery.

## Acceptance Criteria
- The room spec defines a single recoverable-object return flow that starts after `Weight of the Door` completion.
- The only carry state required is a room-local boolean indicating whether the Green Thermos is currently carried.
- The highlighted placement slot is active only while the thermos is carried.
- The placement step results in visible shelf placement, one fire-brightening notch, and one quiet confirmation line.
- The spec does not require inventory UI, save/load, global Hearth systems, or player-rig carrying visuals.
- The spec is implementation-ready as a single-room graybox follow-up with explicit prompts and feedback.

## Parked Ideas
- A later Hearth wave may decide whether placed anchors remain visible across returns.
- A later polish pass may replace the debug overlay text with quieter in-world prompt treatment.
- A later interlude wave may define how additional anchors alter Hearth layout, but this room does not.
