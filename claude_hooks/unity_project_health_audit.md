# Unity Project Health Audit

Use for periodic or pre-merge review of the **Unity + repo** shell—not gameplay features.

## Project layout

- [ ] `Assets/_Project/` remains the home for game content; avoid scattering WIP at `Assets/` root without reason.
- [ ] Scenes intended for shipping live under `_Project/Scenes` once work begins (SampleScene migration is a future choice).

## Generated / ignored paths

- [ ] No commits from `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`.
- [ ] `.gitignore` still matches team expectations for Unity 6000.

## URP

- [ ] Render pipeline assets referenced by Project Settings remain valid paths.
- [ ] No duplicate/conflicting RP assets introduced casually.

## Tests

- [ ] When tests exist, Edit Mode vs Play Mode assemblies align with `Assets/_Project/Tests/`.

## Output

Summarize pass/fail per section; file GitHub issues or `Docs/` notes for follow-ups.
