# House With the Switches

- Puzzle title: `House With the Switches`
- Stable ID: `puzzle.house_with_the_switches`

## Room

`room.main_floor.house_with_the_switches`

## Status

Draft - Pending Gemini Notebook review before any implementation wave.

## Emotional Movement Practiced

Move from forceful correction toward patient perception, then from perception toward deliberate repair.

## Required Systems

- Ego / normal interaction state
- Shadow perception state
- Split-self presentation placeholder for later only
- Scene-local switch interaction
- Scene-local room state tracking
- Feedback layer for light/audio/environmental response
- Stable completion gate for the repaired room state

## Inputs

- Player inspection of the room and switch feedback
- Ordinary light-switch interaction in Ego Form
- Entering Shadow perception / Shadow Form
- Environmental observation of the hidden seam, path, or truth revealed by darkness

## States

### Unresolved

The room appears solvable through ordinary switch use, but its emotional law and hidden truth are not yet understood.

### Pressurized

Wrong-form Ego forcing under surveillance increases contradiction and makes the room feel less stable, less readable, or less trustworthy.

### Damaged

The player has pushed the room into a clearly degraded state where forceful control is visibly failing and a reset in approach is required.

### Revealed

Shadow perception exposes the hidden seam, path, or truth that cannot be seen under ordinary glare.

### Stabilized

The player has stopped escalation and restored enough structure for the room to hold together, but the final integrated action may still be incomplete.

### Completed

The player has used Shadow reveal and Ego stabilization together, and the room returns to an ordinary, repaired baseline.

## Wrong-Form Feedback

Incorrect play should communicate “you are using the wrong self-state for this room,” not just “that input is invalid.” Forcing interaction in Ego Form while the room is under surveillance should trigger immediate instability feedback such as a pressure spike, screen flicker, light fracture, prompt tremble, or similar graybox-safe signal. This teaches that order/control cannot maintain the room through denial.

## Repair Path

The puzzle should allow recovery through perception and a calmer interaction pattern rather than punishment-only failure. The player needs a readable route from pressurized or damaged states into Shadow perception, then back toward a solvable integrated action.

## Acceptance Criteria

- The puzzle can be described in one player sentence without hidden external knowledge.
- Player can enter the room.
- Player can interact with one ordinary light switch in Ego Form.
- Ego-only forcing produces clear wrong-form feedback.
- Player can enter Shadow perception / Shadow Form.
- Shadow reveals one hidden seam, path, or truth not visible in normal light.
- Player can complete one integrated action using both Ego stabilization and Shadow reveal.
- No external notes are required to understand the basic behavior.
- The stabilized state feels meaningfully different from the pressurized state.
- Completion returns the room to a coherent ordinary baseline.
- The puzzle remains scene-local and does not require new global managers or shared puzzle infrastructure.

## Player Clarity Notes

- The player should quickly understand that the ordinary switch matters, but not assume ordinary force is enough.
- Feedback must imply wrong-self-state and denial, not arbitrary developer trickery.
- The safe pocket or observation zone should help the player reset attention.
- Shadow perception should read as vulnerable truth-seeing, not as a power fantasy or monster mode.

## Manual Test Intention

- This spec must be reviewable by Gemini Notebook for emotional truth and source-safety before any implementation wave begins.
- Review should confirm the puzzle’s emotional movement is symbolic, grounded, and not dependent on explicit autobiographical recreation.

## Implementation Notes for the Later Graybox Wave

- Wave 007B must be a graybox prototype only.
- Keep the first implementation scene-local to the target room.
- Use one ordinary switch, one localized Shadow reveal, and one integrated resolution only.
- Do not add full global form architecture.
- Do not add multiple switches, sequences, timing chains, or room-wide puzzle chains.
- Do not add third-person Shadow or split-screen presentation unless Matt explicitly creates a later wave for it.
- Graybox should validate player clarity before any visual polish or lore coupling.
- Instrument manual testing around state transitions: unresolved, pressurized, damaged, revealed, stabilized, completed.
