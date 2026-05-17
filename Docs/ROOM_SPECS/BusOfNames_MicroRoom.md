# Room Spec: Bus of Names Micro-Room

Stable ID:
room.upstairs.bus_of_names.micro_room

Status:
Ready for critique

## Purpose
Define the smallest first playable Bus of Names chamber that could eventually sit beyond the Hearth `E - Next Door` stub.
Keep the beat limited to one symbolic room promise: false labels are present, one label distorts the way forward, and Shadow helps the player notice a knot rather than treat the label as truth.

## Decision Rationale
- Bus of Names gives the next chamber an immediate emotional shape without requiring a larger routing structure first.
- It turns shame into a readable spatial beat fast: one room, one false label, one blocked threshold, one reveal.
- It is smaller and more implementation-ready than Road to Cabin or a Main Floor continuation, which both imply broader navigation and environment scope.
- It carries more specific emotional-play clarity than a Threshold connector, which risks feeling like transit instead of the first true encounter with false naming.

## Observable Player Promise
The player steps through the next door into a cramped threshold that feels part bus aisle and part bedroom edge, where childish false labels hang in view like misplaced name tags. One label visibly warps the only way forward until Shadow reveals that the label is tied to a knot in the space rather than being the truth, and the room signals that lashing out at it is not the right answer.

## Smallest Future Playable Beat
1. The player enters one small Bus of Names micro-room directly after the Hearth next-door handoff and can immediately see a single blocked threshold ahead.
2. In the room, one false label is clearly visible as an object or sign, and its presence makes the threshold feel narrowed, bent, or wrongly named.
3. Ordinary observation establishes that the false label is the main point of attention and that forcing past it is not currently the intended read.
4. Shadow reveals one knot-like truth behind or within the false label, making clear that the label is attached to something tangled rather than describing the player.
5. A restrained cue shows that tearing, smashing, or retaliating at the label is not the answer, while still keeping the room tense and uneasy.
6. The beat ends with the room newly legible but not fully resolved, preserving this as one future graybox encounter rather than a full chapter puzzle.

## Tone & Source-Safety Constraints
- False labels should feel childish, absurd, and still powerful.
- The room should practice noticing before retaliation.
- Shadow must not be framed as evil.
- Do not use real names, real school details, or direct biography.
- No revenge framing.
- No therapist-speak during the horror beat.
- Keep the horror environmental and symbolic.

## Non-Goals
- No implementation or scene work.
- No Unity references or code snippets.
- No full Bus of Names chapter content.
- No puzzle spec beyond the minimal playable beat.
- No lore dump.
- No real-person references.
- No changes to the current Hearth graybox or exit stub.
- No inventory, save/load, chapter select, or progression routing.
- No comparison of all four candidate options in the final deliverable.

## Acceptance Criteria
- The spec file exists at `Docs/ROOM_SPECS/BusOfNames_MicroRoom.md`.
- It contains a Decision Rationale section with 3 to 4 bullets.
- It contains a clear Observable Player Promise section.
- It contains a Smallest Future Playable Beat section with 4 to 6 numbered steps.
- It explicitly lists the required non-goals.
- The file is readable in under two minutes.
- The next implementation wave is obvious from the spec.
- No code, Unity references, implementation steps, scene work, or future-wave assumptions are included.

## Manual Test Intention
1. Open `Docs/ROOM_SPECS/BusOfNames_MicroRoom.md`.
2. Read the Decision Rationale section and confirm it explains why Bus of Names was chosen.
3. Read the Observable Player Promise section and confirm it describes a concrete, testable future behavior.
4. Read the Smallest Future Playable Beat section and confirm it stays small: one micro-room, one false label, one Shadow-revealed knot, one restrained cue.
5. Verify the file stays under 1.5 pages and contains none of the prohibited elements.
6. Confirm a future implementer could read this spec and immediately understand the smallest playable version.

## Parked Ideas
- The exact visual form of the false label can stay flexible as long as it reads as childish, misplaced, and symbolically loud.
- A later wave may decide whether the blocked threshold feels more like a bus aisle interruption, a bedroom doorway, or a deliberately unstable blend of both.
- A later wave may decide what restrained cue best communicates that destruction is not the answer without turning the moment into a tutorial.
