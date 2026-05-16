Emotional question: Can the player stop treating resistance as something to overpower long enough to notice what the door is actually carrying?

Core loop: Approach the stuck front door safely, try ordinary force, receive readable wrong-form pushback, use Shadow perception to reveal the hidden bindings, then contribute a small steadying Ego action so the door can be repaired instead of conquered.

# Room Spec: Weight of the Door

- Stable ID: `room.main_floor.weight_of_door`
- Status: Draft spec packet for a contained follow-up implementation wave

## One-Sentence Emotional Question

Can the player stop treating resistance as something to overpower long enough to notice what the door is actually carrying?

## One-Sentence Core Loop

Approach the stuck front door safely, try ordinary force, receive readable wrong-form pushback, use Shadow perception to reveal the hidden bindings, then contribute a small steadying Ego action so the door can be repaired instead of conquered.

## First-Time Player Onboarding Sequence

1. The player enters a short main-floor hall and sees one obvious exit door that looks like the next sensible place to go.
2. The hall feels quiet rather than hostile, with one grounded corner that reads as a place to stop and reorient.
3. The player tries `E` on the door and gets ordinary resistance first, not immediate spectacle.
4. A second ordinary attempt produces a slightly sharper room-local pressure cue and debug text that suggests forcing is making the hall less stable.
5. Repeated direct forcing teaches that the room is reacting to the player’s approach, not merely blocking progress arbitrarily.
6. The room’s overlay or local feedback points the player toward Shadow perception as observation, not aggression.
7. Holding `Q` near the relevant section of wall and door frame reveals hidden knots, bindings, or seams that explain why the door feels wrong.
8. The player maintains position, steadies, and performs one simple ordinary action while the truth is visible.
9. The bindings settle, the door becomes emotionally lighter, and the hall shifts into a repaired state.

## Exact Graybox Room Layout Blockout

- Approximate dimensions: one rectangular main-floor hall segment, about `8m` long, `3m` wide, and `3m` tall.
- Key objects: front door, narrow runner rug, small side table or shelf, one wall light or ceiling light, one scuffed wall section near the door frame, one anchor object, and one subtle Witness cue location.
- Player start: at the far end of the hall, about `5m` to `6m` back from the door, facing it on entry.
- Door location: centered on the short wall opposite the player start, with enough space on both sides for side-step observation.
- Safe pocket: back-left corner near the player start, readable as a calmer place to pause without leaving the room.
- Shadow reveal area: the strip around the door frame and adjacent wall seam, especially latch-side trim and the floor-wall join within about `1.5m` of the door.
- Wrong-form feedback area: the immediate door interaction zone within arm’s reach, where repeated `E` use should escalate local feedback but never lock the player out.

## Existing Component Reuse / Integration Checklist

- Reuse `ShadowPerceptionController` exactly as the current Wave 009-era player-held perception source.
- Reuse `ShadowRevealable` for the hidden knots, bindings, seam traces, or related reveal geometry.
- Reuse `PlayerInteractor` for raycast interaction and prompt routing.
- Reuse the current `IInteractable` pattern with `Interact(PlayerContext context)` for the door-facing ordinary interaction.
- Reuse the room-local controller pattern proven by `HouseWithTheSwitchesController`: one scene-owned controller with explicit serialized references and local state only.
- Reuse the existing debug overlay pattern if the follow-up wave needs temporary player clarity text during graybox validation.
- Reuse generated fallback audio from `ShadowAudioClipFactory` only if the follow-up wave needs prototype cues and no authored clips exist yet.
- Keep current input assumptions: `E` remains ordinary interact through the existing `Player` action map flow, and `Q` remains Shadow perception through `Player/ShadowPerception`; this spec does not call for any input action changes.
- Prefer a thin interactable bridge pattern similar to `HouseSwitchInteractable` if the door collider should delegate to a room-local controller method.

## Full Puzzle Flow

1. Safe first approach
   The player enters a contained hall, sees one obvious door, and reads the space as quiet but slightly resistant rather than overtly dangerous.
2. Ordinary interaction with the door
   The first `E` interaction gives a believable “stuck” response, suggesting weight, drag, or emotional reluctance instead of a puzzle-game hard no.
3. Pressured response
   A follow-up ordinary attempt makes the hall answer back with small instability: sound strain, light discomfort, door-frame shiver, or overlay wording that implies the player is pushing the wrong way.
4. Tempting wrong-form repeated forcing
   The door remains the most obvious affordance, so a first-time player is naturally tempted to keep forcing it, which is useful because the room needs to teach that insistence increases pressure.
5. Wrong-form feedback that teaches without hard failure
   Repeated forcing never kills, resets, or traps the player; it only makes the hall more strained and more clearly communicates that the current self-state is escalating the problem.
6. Shadow perception reveals knots/bindings/seams
   Holding `Q` in the reveal area exposes the hidden truth: bindings, seams, or knot-like pressure traces wrapped through the frame and latch side, showing that the door is emotionally held shut rather than physically jammed.
7. Ego contribution: steady, observe, hold position, or stabilize
   Ego is still needed, but not as force. The player’s contribution is to hold position, stop yanking, and perform one calm ordinary action while the revealed truth is visible.
8. Integrated solution
   The solution combines Shadow seeing with Ego steadiness: the player keeps the hidden bindings readable and then performs the simple repair or release interaction instead of another forceful shove.
9. Repair path or repair stub
   The door does not need a full systemic repair feature; the room only needs a one-room repair beat or stub that reads as “ease, release, settle” and returns the frame to coherence.
10. Completion state
   The hall calms, the door opens or becomes openable, local pressure drops, and the room reads as survived rather than defeated.

## Minimal Pressure Stub Behavior

- Only define a room-local pressure presentation stub for this hall.
- The stub should respond to repeated wrong-form forcing with small escalating discomfort cues such as light strain, brief audio stress, subtle visual pulse, or overlay warning text.
- The stub should decay or stop once the player disengages and shifts into the correct observation path.
- No global Pressure meter, shared data model, save state, or cross-room propagation is needed for this room spec.

## Shadow Reveal Requirements

- Shadow perception must reveal only the hidden bindings, knots, seam traces, or related support geometry around the door zone.
- The reveal must read as protective perception of hidden truth, not enemy tracking or supernatural dominance.
- The reveal area should be tight enough that the player understands where to look without needing a large scan of the whole hallway.
- The reveal should remain local and legible during hold, then hide cleanly when Shadow perception ends unless the room has completed.

## Ordinary Interaction Requirements

- The door must be interactable through the existing player interaction path.
- The first ordinary interaction should feel plausible and restrained.
- Repeated ordinary forcing should drive wrong-form feedback instead of silent failure.
- The integrated completion interaction should still be an ordinary player action, but only meaningful when the hidden truth is revealed and the room is ready.

## Debug Overlay Text Requirements

- Unresolved: `The front door feels wrong. Press E and notice what pushes back.`
- First door attempt: `It is not just stuck. Forcing it makes the hall tighten.`
- Wrong-form warning: `More force is making this heavier. Stop trying to win the door.`
- Shadow relevant: `Hold Q near the frame and look for what the strain is hiding.`
- Stabilized: `Keep steady. While the bindings are revealed, press E to stop forcing and let the door settle.`
- Completed: `The weight lifts. The door can be opened without a fight.`

## One Anchor / Survived Object

- Candidate: `Green Thermos`
- Placement: small side table or floor edge inside the safe pocket.
- Function: a quiet domestic proof that someone once prepared to endure and continue, not a monologue delivery device.
- Constraint: keep the object symbolic, inspectable later if needed, but not responsible for carrying final lore prose in this room spec.

## One Witness Cue

- Candidate: `Snow Owl`
- Form: subtle optional cue only, such as a feather-like pale mark, soft reflected shape, or brief stillness cue visible from the safe pocket.
- Purpose: reinforce orientation and survival without becoming a collectible, dialogue beat, or mandatory mechanic.

## Player Clarity Risks

- Players may read the door as a simple “spam interact until it opens” gate unless the second response clearly changes tone.
- Players may assume Shadow should be used immediately everywhere if the first ordinary attempt is too punitive.
- Players may miss the reveal zone if the hidden bindings are spread too broadly across the hall.
- Players may misread the room as “Shadow solves everything” unless the final action still asks for a calm ordinary contribution.
- Players may interpret the feedback as arbitrary puzzle denial if the door’s resistance does not visually or verbally evolve between attempts.

## Source-Safety Notes

- Keep the door symbolic of burden, resistance, and inherited weight rather than a literal reenactment of a real event.
- Avoid explicit domestic violence framing, accusation framing, or real-person stand-ins.
- Keep Shadow protective and truth-seeing, never sinister or predatory.
- Keep the room’s discomfort in pressure, weight, and constriction rather than gore, screaming spectacle, or trauma theater.

## Parked Ideas

- A later pass could let the door emit subtle settling sounds after stabilization instead of needing heavier visual effects.
- The Green Thermos could become a later inspectable anchor reward, but that is outside this room-spec wave.
- A later house-wide pass could decide whether repaired doors visually persist across returns, but this spec does not require persistence design.

## Follow-Up Implementation Wave Boundaries

- Limit the follow-up to this one room only.
- Prefer fewer than `8-10` changed files total.
- Keep logic scene-local with one room controller and one door-facing interactable bridge if needed.
- Reuse existing Shadow, interaction, overlay, and fallback-audio patterns rather than introducing new systems.
- Do not modify the player controller, input actions, save/load flow, Hearth onboarding, or the existing Wave 007B room.
- Do not expand this room into a multi-door chain, inventory puzzle, or broader house framework.

## 10-Step Manual Tester Checklist

1. Enter the graybox hall and confirm the front door reads as the obvious next action.
2. Pause in the safe pocket and confirm the room feels tense but not immediately hostile or confusing.
3. Press `E` on the door once and confirm the first response reads as meaningful resistance rather than a generic locked-door failure.
4. Press `E` again and confirm the room gives clearer wrong-form feedback without hard failure, death, or reset.
5. Stay near the door and confirm repeated forcing increases clarity that more pressure is the wrong answer.
6. Hold `Q` in the reveal area and confirm hidden bindings, knots, or seam traces become legible around the frame.
7. Release `Q` and confirm the reveal hides cleanly and the room still invites another attempt.
8. Hold `Q` again and perform the integrated ordinary action; confirm the room now reads as calming or settling instead of resisting.
9. Confirm the completion state leaves the hall coherent and the door openable or clearly released.
10. State the emotional truth of the room in one sentence and confirm it matches the intended movement from force toward steady perception and repair.

## Rollback / Iteration Notes

- If the follow-up graybox becomes too large, cut Witness presentation first, then anchor presentation depth, before touching the core integrated door loop.
- If player clarity slips, strengthen the second ordinary-attempt feedback before adding any new mechanic.
- If the room starts implying a full Pressure or Focus Memory system, pull those references back to room-local stubs only.
