# Build Log

Newest entries at the top.

---

## 2026-05-14 - Wave 002: Project Coding Conventions and Guardrails

### Summary

Added Cursor project rules for Unity C# conventions, namespaces, serialization and null-safety expectations, architecture guardrails, and stable ID formats. Documented the same decisions in `Docs/DECISIONS.md`. No gameplay scripts, scenes, or packages were added.

### Files Changed

- `.cursor/rules/adid-architecture-and-waves.mdc` (new)
- `.cursor/rules/adid-csharp-unity.mdc` (new)
- `Docs/DECISIONS.md`
- `Docs/BUILD_LOG.md`

### What Was Implemented

- **Always-on rule:** waves, out-of-scope systems, architecture limits, stable ID table, agent roles.
- **C# rule (when editing `Assets/_Project/**/*.cs`):** namespace ↔ folder table, `[SerializeField]` / `OnValidate`, null-safety logging, general Unity C# guardrails.

### Manual Test Steps

1. In Cursor, open **Settings → Rules** (or the rules panel) and confirm the two **A Door Inside the Dark** rules appear with the expected descriptions.
2. Open any existing `.cs` file under `Assets/` (e.g. tutorial scripts): confirm the C# rule applies only when the path matches `Assets/_Project/**/*.cs` (new project code); adjust globs later if you move third-party code.
3. Read `Docs/DECISIONS.md` § Wave 002 and confirm it matches team intent.

### Known Limitations

- Rules do not enforce compile-time checks; they guide agents and humans. Assembly definitions / analyzers can be added in a later wave if desired.
- Tutorial or non-`_Project` scripts are outside the C# rule glob until moved or the glob is extended.

### Rollback Notes

- Delete `.cursor/rules/adid-architecture-and-waves.mdc` and `.cursor/rules/adid-csharp-unity.mdc`, then remove the **Wave 002** subsection from `Docs/DECISIONS.md` and this build log entry from `Docs/BUILD_LOG.md` (or revert the commit).

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
