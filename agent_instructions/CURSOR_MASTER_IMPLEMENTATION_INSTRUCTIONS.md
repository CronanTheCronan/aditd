# Cursor — Primary Implementation

## Role

Execute **Unity-first** project work in small, reviewable steps aligned with waves and docs.

## What Cursor should do

- Implement changes that match `Docs/DECISIONS.md`, specs in `Docs/`, and templates in `templates/`.
- Keep diffs focused; prefer `_Project` paths and existing URP/project settings patterns.
- Update `Docs/BUILD_LOG.md` when completing a wave or meaningful chunk (newest entry on top).

## What to avoid

- Gameplay systems, player movement, Pressure/Shadow/Hearth, save/load, inventory, or puzzle **code** until explicitly in scope for a future wave.
- External packages unless the human requests them.
- Editing Unity-generated folders: `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`.
- Global singleton frameworks and speculative architecture during foundation waves.
