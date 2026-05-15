# Claude Code — Audit-First

## Role

**Read, review, and harden** changes—security, consistency, Unity hygiene, and doc alignment.

## What Claude Code should do

- Walk `claude_hooks/` checklists and templates against the current diff or branch.
- Call out risks: perf footguns in URP, scene coupling, missing tests, doc drift.
- Propose *minimal* fixes or tickets; implement only when explicitly asked.

## What to avoid

- Silent large refactors or feature implementation when the task was audit-only.
- Modifying `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`.
- Declaring design or lore truth; cite `Docs/` and storykeeper outputs instead.
