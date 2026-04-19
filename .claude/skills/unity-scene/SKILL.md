---
name: unity-scene
description: Phase 2 — Create GameObjects, set up scene hierarchy and add components from blueprint.
---

You are executing Phase 2 of the Unity game builder pipeline. Scenes exist from Phase 1. Your job is to populate them with GameObjects and components.

For each scene in the blueprint:

1. OPEN SCENE
   Use manage_scene action "load" with name or path parameter.

2. CREATE GAMEOBJECTS
   Read the blueprint to identify what GameObjects this scene needs.
   Use manage_gameobject action "create" for each one.

   Parameters for create:
   - name: GameObject name
   - parent: parent GameObject name/path (null for root)
   - position: [x, y, z] local position
   - rotation: [x, y, z] local euler angles
   - scale: [x, y, z] local scale
   - components_to_add: list of component type names
   - tag: optional tag
   - set_active: true/false

   Typical structure for a game scene:
   - Managers (empty GameObjects): GameManager, AudioManager, UIManager
   - Gameplay objects: depend on genre
   - Canvas: for UI (with components_to_add: ["Canvas", "CanvasScaler", "GraphicRaycaster"])
   - Camera: Main Camera (default exists, configure in Phase 5)
   - EventSystem: components_to_add: ["EventSystem", "StandaloneInputModule"]

3. SET UP HIERARCHY
   Use manage_gameobject to parent objects correctly.
   Group related objects under empty parent GameObjects.

   Example for slot game SlotGame scene:
   ```
   Root
   ├── GameManager
   ├── AudioManager
   ├── SlotMachine
   │   ├── ReelContainer
   │   │   ├── Reel1
   │   │   ├── Reel2
   │   │   ├── Reel3
   │   │   ├── Reel4
   │   │   └── Reel5
   │   └── PaylineDisplay
   ├── Canvas (UI root)
   ├── EventSystem
   └── Main Camera
   ```

4. ADD COMPONENTS
   Use manage_components action "add" with params:
   - target: GameObject name or path
   - component_type: component type name (e.g., "AudioSource", "Canvas")

   Use manage_components action "set_property" to configure:
   - target: GameObject name
   - component_type: which component
   - property: property name
   - value: property value

   Components to add in this phase:
   - AudioSource on AudioManager
   - Canvas, CanvasScaler, GraphicRaycaster on Canvas (if not added during create)
   - Script components are added in Phase 3

5. VERIFY
   Use find_gameobjects with searchTerm to confirm objects exist.
   Report the scene hierarchy.

Use batch_execute to group create operations per scene (max 25 per batch):
```json
{"commands": [
  {"tool": "manage_gameobject", "params": {"action": "create", "name": "GameManager"}},
  {"tool": "manage_gameobject", "params": {"action": "create", "name": "SlotMachine"}},
  {"tool": "manage_gameobject", "params": {"action": "create", "name": "ReelContainer", "parent": "SlotMachine"}},
  ...
]}
```

After all scenes are populated, confirm ready for Phase 3.