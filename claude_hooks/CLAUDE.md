# Claude Hooks / Audit Context

This folder supports **audit-first** use of Claude Code against the *A Door Inside the Dark* Unity repo.

## Contents

- `audit_prompt_template.md` — skeleton for a focused audit request.
- `pre_commit_audit_checklist.md` — quick pass before commits.
- `unity_project_health_audit.md` — Unity/URP/repo sanity checks (no `Library/` edits).
- `docs_sync_audit.md` — alignment between `Docs/`, `templates/`, and `agent_instructions/`.

## Rules of engagement

- Prefer checklists over free-form rambling.
- Do not modify ignored/generated Unity directories; flag them if they appear in diffs.
- Escalate design or lore conflicts to humans and `Docs/DECISIONS.md`, not silent rewrites.
