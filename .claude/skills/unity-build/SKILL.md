---
name: unity-build
description: Phase 7 — Validate all game components, run tests, and build the game.
---

You are executing Phase 7 of the Unity game builder pipeline. All prior phases are complete. Your job is to validate, test, and build the game.

1. VALIDATE SCRIPTS

Use validate_script on every script in the project.
Collect all errors. If any found:
- Use script_apply_edits to fix
- Re-validate until all scripts compile clean

Use read_console to check for runtime compilation errors.

2. VALIDATE SCENE INTEGRITY

For each scene:
- Use manage_scene to open scene
- Use find_gameobjects to verify all expected GameObjects exist
- Check that scripts are attached to correct GameObjects
- Check for missing component references

Cross-reference against the blueprint:
- Every script listed → exists and compiles
- Every scene listed → exists and contains expected objects
- Every UI screen → exists with expected elements

3. VALIDATE REFERENCES

Use read_console after a refresh_unity to check for:
- Missing script references
- Missing material/texture references
- Null reference warnings
- Shader errors

Fix any issues found using manage_components or script_apply_edits.

4. TEST

If test scripts exist, use run_tests to execute them.
Use get_test_job to poll for results.

If no test scripts exist, skip this step.

5. BUILD

Use manage_build with these actions:

Read current settings:
  manage_build action "platform" (no target param → returns current platform)
  manage_build action "scenes" (no scenes param → returns current build scenes)

Configure platform:
  manage_build action "platform" with target: "StandaloneWindows64"
  (or ask user for preferred platform)

Set player settings:
  manage_build action "settings" with property: "productName", value: "[game name]"
  manage_build action "settings" with property: "companyName", value: "[company]"

Add scenes to build:
  manage_build action "scenes" with scenes: ["Assets/Scenes/MainMenu.unity", "Assets/Scenes/SlotGame.unity", ...]

Trigger build:
  manage_build action "build" with:
  - target: "StandaloneWindows64"
  - output_path: "Builds/[GameName]"
  - scenes: [scene list]
  - development: false

Poll build status:
  manage_build action "status" (returns job progress)

Report build result: success/failure, output path, build size.

6. FINAL REPORT

```
## Build Complete

Project: [name]
Genre: [genre]
Platform: [platform]

Scenes: [list with status]
Scripts: [count] total, [count] validated
UI Screens: [list]
Assets: [materials count], [VFX count]

Build: [SUCCESS/FAILED]
Output: [path]
Size: [size]

Warnings: [count, or "none"]
```

If build failed, report errors and suggest fixes.