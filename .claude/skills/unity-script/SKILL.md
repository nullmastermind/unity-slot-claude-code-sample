---
name: unity-script
description: Phase 3 — Write all C# scripts, validate compilation, attach to GameObjects.
---

You are executing Phase 3 of the Unity game builder pipeline. Scenes and GameObjects exist from prior phases. Your job is to write all C# scripts defined in the blueprint.

WORKFLOW

For each script in the blueprint's Scripts section:

1. CREATE SCRIPT
   Use create_script with exact parameters:
   - path: "Assets/Scripts/[category]/[Name].cs" (Assets-relative)
   - contents: full C# source code as string
   - namespace: project namespace (e.g., "SlotGame")
   - script_type: "MonoBehaviour" or "ScriptableObject"

2. VALIDATE
   Use validate_script with params:
   - uri: "Assets/Scripts/[category]/[Name].cs"
   - level: "standard" (for thorough check)
   - include_diagnostics: true
   If errors found:
   - Read the error messages
   - Use script_apply_edits with params: name, path, edits (structured edit operations)
     Supported edit ops: replace_method, insert_method, delete_method, anchor_insert, regex_replace
   - Validate again
   Do not move to the next script until the current one compiles clean.

3. ATTACH TO GAMEOBJECT
   Use manage_components action "add" with params:
   - target: GameObject name from Phase 2 hierarchy
   - component_type: script class name (e.g., "SlotMachine")

4. CHECK CONSOLE
   Use read_console action "get" with types: ["error", "exception"] periodically
   to catch compilation errors that validate_script might miss.

SCRIPT ORDER

Write scripts in dependency order. Scripts that other scripts depend on come first.

Typical order:
1. Data classes and ScriptableObjects (SymbolDatabase, PaylineDefinition, ReelStrip)
2. Core systems (BalanceManager, BetController)
3. Game logic (ReelController, PaylineEvaluator, SlotMachine)
4. Bonus systems (BonusManager)
5. Managers (GameManager, AudioManager)
6. UI scripts (UIManager, SpinButton, WinDisplay)

CODING STANDARDS

- Namespace: use project name (e.g., namespace SlotGame)
- [SerializeField] for inspector-exposed private fields
- [RequireComponent(typeof(X))] where dependencies exist
- ScriptableObjects: use [CreateAssetMenu] attribute
- UnityEvents for decoupled communication between systems
- Summaries on public methods and classes
- No magic numbers — use constants or SerializeField

SCRIPTABLEOBJECT INSTANCES

After creating ScriptableObject scripts, use manage_scriptable_object
action "create" to create asset instances:
- typeName: "SymbolDatabase", folderPath: "Assets/ScriptableObjects", assetName: "DefaultSymbols"
  Then action "modify" with patches to set symbol definitions and payouts
- typeName: "PaylineDefinition" for each payline pattern
- typeName: "ReelStrip" for each reel configuration

After all scripts are written and validated, confirm ready for Phase 4.