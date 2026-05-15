# Master Agent Loop

## Purpose

A lightweight **round-trip** between design truth, critique, implementation, and audit for this Unity-first horror puzzle game.

## Suggested loop

1. **Gemini Notebook** — refine story/lore/puzzle intent; export or mirror into `Docs/` specs using `templates/`.
2. **Grok** — grounded critique pass on readability, tone, and plausibility.
3. **Cursor** — implement the scoped wave in Unity/repo; log in `Docs/BUILD_LOG.md`.
4. **Claude Code** — audit-only pass using `claude_hooks/` checklists (when engaged).

## Guardrails

- Small waves with explicit “in scope / out of scope.”
- No Pressure/Shadow/Hearth/save/inventory/puzzle **code** until a dedicated wave says so.
- Never treat any single agent as sole source of truth for both code and canon.
