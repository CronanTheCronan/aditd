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

When reversing or revising a decision, add a new dated subsection rather than silently editing old bullets.
