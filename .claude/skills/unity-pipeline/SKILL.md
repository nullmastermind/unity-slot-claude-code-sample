---
name: unity-pipeline
description: Shared build pipeline for Unity game builder. Orchestrates phase skills to build a game from a genre blueprint.
---

You are the build pipeline for the Unity game builder. A genre skill has loaded a GAME BLUEPRINT into the conversation. Your job is to execute phase skills in order to build the game in Unity via unity-mcp.

PREREQUISITES

Before starting the pipeline:
- unity-mcp server must be running (default: localhost:8080)
- Unity Editor must be open with the target project
- The genre skill must have provided a GAME BLUEPRINT

If prerequisites are unclear, announce them and ask the user to confirm before proceeding.

---

PIPELINE

Execute phases in order. Each phase depends on prior phases completing successfully.

Phase 1 — Project Structure
Use the Skill tool to invoke "unity-architect".
Sets up folder structure, creates scenes, verifies unity-mcp connection.

Phase 2 — Scene Construction
Use the Skill tool to invoke "unity-scene".
Creates GameObjects, sets up hierarchy, adds components.

Phase 3 — Script Writing
Use the Skill tool to invoke "unity-script".
Creates all C# scripts, validates compilation, attaches to GameObjects.

Phase 4 — UI Construction
Use the Skill tool to invoke "unity-ui".
Builds all screens, HUD elements, popups, navigation.

Phase 5 — Visual Assets
Use the Skill tool to invoke "unity-assets".
Creates materials, configures lighting, sets up VFX, camera.

Phase 6 — Physics (conditional)
Check the blueprint. If it says "skip" or "not needed", skip this phase.
Otherwise, use the Skill tool to invoke "unity-physics".

Phase 7 — Build & Validate
Use the Skill tool to invoke "unity-build".
Validates everything, runs tests, builds the game.

---

RULES

1. Execute phases strictly in order. Do not skip ahead.
2. Announce each phase before starting: "Phase N: [name] — starting..."
3. After each phase, read Unity console output to catch errors.
   Use the unity-mcp read_console tool between phases.
4. If a phase reports errors that block the next phase, attempt to fix them
   before moving on. If unfixable, stop and report to user.
5. Use batch_execute when calling multiple unity-mcp tools in sequence.
   This is 10-100× faster than individual calls.
6. After all phases complete, announce the final summary.

---

PROGRESS REPORTING

Before each phase:
```
## Phase [N]/7: [Phase Name]
[one line describing what this phase will do]
```

After all phases:
```
## Pipeline Complete

Scenes: [list]
Scripts: [count] created, [count] validated
UI Screens: [list]
Assets: [count] materials, [count] VFX
Build: [status]

The game is ready to test in Unity Editor.
```

If pipeline stops due to error:
```
## Pipeline Stopped at Phase [N]

Error: [description]
Console output: [relevant errors]

Suggested fix: [if known]
```