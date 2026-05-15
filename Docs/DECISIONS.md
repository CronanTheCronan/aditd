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

## 2026-05-15 - Wave 003 authorized: First-Person Controller MVP

Decision:
Authorize a narrow generic first-person controller MVP for Wave 003.

Context:
The project needs a controllable player body before Pressure, Shadow perception, Hearth, lore inspection, or puzzle behavior can be validated.

Approved scope:
- Generic movement.
- Generic camera look.
- Generic interact raycast.
- Minimal Input Actions asset.
- Minimal graybox test scene.
- Debug interactable object.

Explicitly out of scope:
- Pressure.
- Shadow perception.
- Hearth.
- Inventory.
- Save/load.
- Puzzle framework.
- Enemy AI.
- Combat.
- Any narrative or horror-specific coupling inside player movement.

Reasoning:
The controller is foundation work. It should remain boring, stable, and reusable so later symbolic systems can attach without contaminating the player scripts.

Consequences:
Cursor may now implement player movement only within the Wave 003 scope. Claude must audit for player-controller contamination, singleton creep, missing scene references, and missing build log/manual test coverage.

## Wave 002 — Coding conventions and guardrails (2026-05-14)

11. **C# namespaces** follow `ADoorInsideTheDark.<Area>` and align with `Assets/_Project/Code/<Area>/` (`Core`, `Player`, `Interaction`, `Rooms`, `Puzzles`, `Pressure`, `Shadow`, `Hearth`, `Inventory`, `Lore`, `Save`, `UI`, `Utilities`).

12. **Serialized fields:** prefer `[SerializeField]` on **private** fields; use `OnValidate` for safe clamps and consistency checks; avoid public mutable serialized fields unless Unity or a rare API demands it.

13. **Null-safety:** missing references must **not** be swallowed silently—use **`Debug.LogWarning`** (or appropriate visible failure) with context; use graceful fallback only when clearly optional and still logged.

14. **Architecture:** keep the future **player controller generic**; **puzzle logic stays scene-local** unless explicitly approved; **no singleton / global service** unless approved; prefer **ScriptableObjects** for stable definitions when data exists; **no large frameworks** until at least **two rooms** validate a pattern; target **1–6 files** per implementation wave when practical.

15. **Stable string IDs** use lowercase dot-separated segments with domain prefix: `room.<area>.<slug>`, `puzzle.<slug>`, `lore.<slug>`, `anchor.<slug>`, `pressure.<area>.<slug>` (e.g. `room.main_floor.weight_of_door`, `puzzle.weight_of_door`, `lore.green_thermos`, `anchor.green_thermos`, `pressure.main_floor.door_static`). Breaking ID changes require a **BUILD_LOG** note.

When reversing or revising a decision, add a new dated subsection rather than silently editing old bullets.
