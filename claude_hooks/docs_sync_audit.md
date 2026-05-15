# Docs Sync Audit

Ensures written artifacts stay aligned without rewriting design intent.

## Sources of truth (by domain)

- **Story / lore / puzzle intent:** Gemini Notebook exports + human edits → mirrored in `Docs/` using `templates/`.
- **Engineering defaults:** `Docs/DECISIONS.md`.
- **Shipped or attempted work:** `Docs/BUILD_LOG.md` (newest first).

## Checks

- [ ] Room specs reference the same room IDs/names as puzzle specs where linked.
- [ ] Lore items list related puzzles and rooms consistently.
- [ ] `agent_instructions/` still matches `Docs/DECISIONS.md` (roles, guardrails).
- [ ] Templates unchanged in section headings unless a deliberate template revision was agreed.

## If drift is found

Open a small **change request** using `templates/CHANGE_REQUEST_TEMPLATE.md` instead of silent mass edits.
