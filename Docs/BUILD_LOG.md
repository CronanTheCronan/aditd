# Build Log

Newest entries at the top.

---

## 2026-05-14 - Wave 000: Clean Project Foundation

### Summary

Established the `_Project` Unity asset layout, repository documentation folders, agent instruction stubs, Claude audit hook docs, spec templates, and `.gitkeep` placeholders so the repo tracks an empty but intentional structure. No gameplay systems, movement, or puzzle code were added.

### Files Changed

- `Assets/_Project/**` (folders + `.gitkeep`)
- `Docs/**` (this file, `DECISIONS.md`, empty spec subfolders with `.gitkeep`)
- `agent_instructions/**`
- `claude_hooks/**`
- `templates/**`

### What Was Implemented

- Full `Assets/_Project/` tree for art, audio, code namespaces, data, prefabs, scenes, settings, and tests (placeholders only).
- `Docs/` with build log, decisions log, and empty `ROOM_SPECS`, `PUZZLE_SPECS`, `AGENT_REPORTS`, `LORE_ITEMS`, `ART_PROMPTS`.
- Agent-facing markdown in `agent_instructions/` and audit-oriented markdown in `claude_hooks/`.
- Reusable templates under `templates/`.

### Manual Test Steps

1. Open the project in Unity 6000.x and confirm the `_Project` folder appears in the Project window.
2. Confirm no compiler errors (no new C# scripts were added for this wave).
3. Optionally verify Git shows the new paths and `.gitkeep` files as expected.

### Known Limitations

- Folders contain placeholders only; no scenes, prefabs, or ScriptableObjects were created in this wave.
- Unity may generate `.meta` files for new assets on first import; commit those in a follow-up if your workflow requires them.

### Rollback Notes

- Remove `Assets/_Project/`, `Docs/` (except any content you want to keep), `agent_instructions/`, `claude_hooks/`, and `templates/` and revert this commit if the structure needs to be redone.

---
