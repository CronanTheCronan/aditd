# Project Decisions

Living log of agreed defaults for **A Door Inside the Dark**. Unity-first, clean-room assumptions unless noted.

## Initial decisions (Wave 000)

1. **Unity 6000.x with URP** is the rendering and project target.
2. This is a **clean-room** project; prior engines, prototypes, and deleted asset histories are out of scope for implementation.
3. **Cursor** is the primary implementation environment for day-to-day edits and refactors.
4. **Claude Code** is **audit-first**: use it for review, checklists, and risk passes—not silent bulk implementation unless explicitly requested.
5. **Gemini Notebook** is the continuity owner for **story, lore, and puzzle** specs; implementation should trace back to it.
6. **Grok** is the **grounded critique** assistant: plausibility, tone, friction, and “does this hold up?” passes.
7. Prefer **small waves** of work with clear boundaries (like Wave 000) over large undifferentiated dumps.
8. Prefer **scene-local** puzzle logic and room-owned composition over global orchestration early on.
9. Prefer **ScriptableObjects** (or equivalent data assets) for definitions once gameplay data exists—avoid hardcoding scattered magic values.
10. **Avoid singletons and global frameworks** until a need is proven; favor explicit references and small scopes first.

## Wave 002 — Coding conventions and guardrails (2026-05-14)

11. **C# namespaces** follow `ADoorInsideTheDark.<Area>` and align with `Assets/_Project/Code/<Area>/` (`Core`, `Player`, `Interaction`, `Rooms`, `Puzzles`, `Pressure`, `Shadow`, `Hearth`, `Inventory`, `Lore`, `Save`, `UI`, `Utilities`).

12. **Serialized fields:** prefer `[SerializeField]` on **private** fields; use `OnValidate` for safe clamps and consistency checks; avoid public mutable serialized fields unless Unity or a rare API demands it.

13. **Null-safety:** missing references must **not** be swallowed silently—use **`Debug.LogWarning`** (or appropriate visible failure) with context; use graceful fallback only when clearly optional and still logged.

14. **Architecture:** keep the future **player controller generic**; **puzzle logic stays scene-local** unless explicitly approved; **no singleton / global service** unless approved; prefer **ScriptableObjects** for stable definitions when data exists; **no large frameworks** until at least **two rooms** validate a pattern; target **1–6 files** per implementation wave when practical.

15. **Stable string IDs** use lowercase dot-separated segments with domain prefix: `room.<area>.<slug>`, `puzzle.<slug>`, `lore.<slug>`, `anchor.<slug>`, `pressure.<area>.<slug>` (e.g. `room.main_floor.weight_of_door`, `puzzle.weight_of_door`, `lore.green_thermos`, `anchor.green_thermos`, `pressure.main_floor.door_static`). Breaking ID changes require a **BUILD_LOG** note.

## 2026-05-15 - Wave 003: First-Person Controller MVP (authorized + implemented)

Decision:
Authorize and deliver a narrow generic first-person controller MVP.

Context:
The project needs a controllable player body before Pressure, Shadow perception, Hearth, lore inspection, or puzzle behavior can be validated.

Approved scope:
- Generic movement (CharacterController-based MVP).
- Generic camera look.
- Generic interact raycast.
- Minimal `IInteractable` interface.
- Minimal `PlayerContext` data object if needed for interaction context.
- Debug interactable object.
- Minimal Input Actions asset.
- Minimal graybox test scene.
- Player prefab if practical.

Explicitly out of scope:
- Pressure, Shadow perception, Hearth.
- Inventory, save/load, puzzle framework, enemy AI, combat.
- Narrative or horror-specific coupling inside player movement.

Reasoning:
Foundation work should stay boring and reusable so later systems attach without contaminating player scripts.

Consequences:
Implemented wave recorded in `Docs/BUILD_LOG.md`. Future waves build on movement/look/interaction while keeping player scripts generic; Claude audits watch for contamination and serialization/build-log hygiene.

## 2026-05-15 - Wave 004 authorized: Interaction Prompt and Inspect Stub

Decision:
Wave 004 was authorized and implemented as a minimal interaction prompt and inspection UI stub.

Context:
The project needed a small player-facing inspect loop before adding Pressure, Shadow, Hearth, or lore systems.

Scope:
- Interaction prompt view
- Inspection panel view
- IInspectable contract
- Inspectable debug object
- PlayerInteractor routing to prompt/panel references

Constraints:
- No final lore database
- No inventory
- No Pressure
- No Shadow
- No Hearth
- No save/load
- No global UI manager
- No puzzle framework

Consequences:
Future waves may use the inspect loop for lore item MVP work, but inspection UI must remain scene-local until reuse justifies a broader UI architecture.


When reversing or revising a decision, add a new dated subsection rather than silently editing old bullets.
