# Pre-Commit Audit Checklist

Quick pass before `git commit` on this project.

- [ ] Diff is free of `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/` noise.
- [ ] No unexpected `.cs` gameplay systems if the wave forbids them (check wave notes / `Docs/BUILD_LOG.md`).
- [ ] No new external packages or registry edits unless intentional.
- [ ] `Docs/BUILD_LOG.md` updated if the change completes a wave or user-facing milestone.
- [ ] Secrets not present (`.env`, keys, tokens).
- [ ] URP-related edits (if any) are in tracked project/settings paths, not generated caches.

If any box fails, fix or split the commit before proceeding.
