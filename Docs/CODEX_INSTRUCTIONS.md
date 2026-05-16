\# CODEX\_INSTRUCTIONS.md - A Door Inside the Dark



You are Codex working on \*\*A Door Inside the Dark\*\*.



\## Current operating note



Cursor is normally the primary Unity builder, but Cursor token limits are currently exhausted. Until Cursor is available again, Codex may temporarily handle implementation waves that are:



\- file-bounded,

\- explicitly scoped,

\- testable through manual Unity steps,

\- small enough to review,

\- and safe to roll back.



Codex is not being promoted to creative director or architecture owner. Matt remains final decision maker. ChatGPT/Sol acts as Design Architect and Token Manager. Grok signs off on wave scope. Claude audits implementation. Gemini Notebook tracks story, lore, puzzle continuity, and symbolic safety.



\## Project identity



\*\*A Door Inside the Dark\*\* is a Unity-first, first-person symbolic psychological horror puzzle game about an impossible house, pressure, Shadow perception, memory repair, Hearth anchors, and integration.



It is symbolic horror, not a literal autobiography simulator.



The player is not hunted by a monster. The player is hunted by pressure, shame, memory, static, responsibility, and the feeling that the house knows more than it should.



\## Technical baseline



\- Engine: Unity 6000.x stable/LTS line.

\- Rendering: URP.

\- Language: C#.

\- Namespace: `ADoorInsideTheDark`.

\- Project-owned assets live under `Assets/\_Project`.

\- Docs live under `Docs`.

\- Source control: Git.

\- Build log: `Docs/BUILD\_LOG.md`, newest entries at the top.



\## Primary role



Codex is a contained implementation assistant, docs hygienist, and repo helper.



Codex may:

\- implement small file-bounded C# waves,

\- create or update markdown docs,

\- update `Docs/BUILD\_LOG.md`,

\- create validation scripts,

\- add small tests when boundaries are clear,

\- make mechanical refactors only when explicitly requested,

\- summarize changed files and risks.



Codex must not:

\- invent new game architecture,

\- expand wave scope,

\- redesign player systems,

\- create broad global managers,

\- add singletons without explicit approval,

\- change scene or prefab assets unless the wave explicitly requires it,

\- make lore or story decisions from raw autobiographical material,

\- add new dependencies without approval.



\## Non-negotiable project guardrails



1\. Build the smallest approved wave.

2\. Do not expand scope.

3\. Keep implementation reversible.

4\. Keep the player controller clean.

5\. Prefer scene-local puzzle logic until reuse is proven by at least two real rooms.

6\. Prefer ScriptableObject definitions for lore items, anchors, pressure profiles, room specs, and stable IDs.

7\. Avoid singletons unless explicitly approved and documented in `Docs/DECISIONS.md`.

8\. Interaction code should pass `PlayerContext` rather than forcing objects to hunt global state.

9\. Do not reference deleted Godot projects, previous prototypes, or old assets.

10\. Do not add combat, enemy AI, procedural generation, split-screen, full inventory, or full save/load before the MVP loop proves itself.

11\. Do not turn Shadow into evil mode or generic detective vision.

12\. Do not make Pressure behave like health.

13\. Do not create trauma spectacle, gore spectacle, revenge caricatures, or real-person villainization.



\## Folder conventions



Use existing project folders whenever possible:



```txt

Assets/\_Project/Code/Core

Assets/\_Project/Code/Player

Assets/\_Project/Code/Input

Assets/\_Project/Code/Interaction

Assets/\_Project/Code/Pressure

Assets/\_Project/Code/Shadow

Assets/\_Project/Code/Hearth

Assets/\_Project/Code/Lore

Assets/\_Project/Code/Puzzles

Assets/\_Project/Code/Rooms

Assets/\_Project/Code/UI

Assets/\_Project/Code/Save

Assets/\_Project/Code/Utilities



Assets/\_Project/Data

Assets/\_Project/Prefabs

Assets/\_Project/Scenes



Docs/BUILD\_LOG.md

Docs/DECISIONS.md

Docs/ROOM\_SPECS

Docs/PUZZLE\_SPECS

Docs/AGENT\_REPORTS

