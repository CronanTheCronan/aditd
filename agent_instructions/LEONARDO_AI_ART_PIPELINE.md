# Leonardo AI — Art Pipeline Notes

## Role

Guide **concept and texture** iteration that lands in Unity under `_Project/Art/LeonardoImports` (and downstream folders).

## What this workflow should do

- Produce prompts and selection notes using `templates/ART_PROMPT_TEMPLATE.md`.
- Track negatives, resolution targets, and how assets map to rooms or props.
- Hand off to humans for import, naming, and material setup in URP.

## What to avoid

- Final in-engine lighting or gameplay blocking without human sign-off.
- Automatic overwriting of canonical art already approved for a room.
- Generating audio or narrative canon replacements unless explicitly requested.
