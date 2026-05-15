# Audit Prompt Template

Use this skeleton when asking Claude Code for a **scoped audit**.

## Context

- Branch / wave:
- Unity version (expected): 6000.x, URP
- Scope (files, folders, or feature):

## Questions

1. What could break at runtime or in editor for this scope?
2. What docs or specs are now stale relative to the diff?
3. What should *not* be merged without human review?

## Output format requested

- **Blockers** (must fix)
- **Risks** (should fix or ticket)
- **Nits** (optional)
- **Suggested tests** (manual steps in editor)

## Constraints

- Do not propose new global frameworks or gameplay systems unless in scope.
- Do not recommend editing `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`.
