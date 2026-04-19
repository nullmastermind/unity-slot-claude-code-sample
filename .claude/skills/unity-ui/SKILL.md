---
name: unity-ui
description: Phase 4 — Build all UI screens, HUD elements, popups, and navigation using Canvas UI.
---

You are executing Phase 4 of the Unity game builder pipeline. Scenes, GameObjects, and scripts exist from prior phases. Your job is to build all UI defined in the blueprint's UI Blueprint section.

IMPORTANT: unity-mcp's manage_ui tool is for UI Toolkit (UXML/USS) only.
For game UI, use Canvas-based approach with these tools:
- manage_gameobject → create UI GameObjects (Canvas, Panel, Button, Text, etc.)
- manage_components → add/configure UI components (CanvasScaler, Image, Button, etc.)
- manage_packages → install TextMeshPro if needed
- batch_execute → group multiple UI creation calls for performance

SETUP

Before building UI elements:
1. Check if TextMeshPro is installed:
   Use manage_packages action "list_packages" to check.
   If not found, use manage_packages action "add_package" with package "com.unity.textmeshpro".
2. After installing TMP, use refresh_unity to trigger reimport.
3. Open the correct scene with manage_scene action "load".

BUILD ORDER

Build UI in this order for each scene:

1. CANVAS SETUP
   The root Canvas GameObject should exist from Phase 2.
   Use manage_components action "set_property" to configure:
   - Canvas component: renderMode = 0 (Screen Space - Overlay)
   - CanvasScaler: uiScaleMode = 1 (Scale With Screen Size),
     referenceResolution = {x: 1920, y: 1080},
     matchWidthOrHeight = 0.5
   - GraphicRaycaster: already added in Phase 2

2. SCREEN PANELS
   For each screen in the UI Blueprint, use manage_gameobject action "create":
   - parent: Canvas GameObject
   - name: "[ScreenName]Panel"
   - components_to_add: ["RectTransform", "CanvasRenderer", "Image"]
   - component_properties: {
       "RectTransform": {anchorMin: {x:0,y:0}, anchorMax: {x:1,y:1}, offsetMin: {x:0,y:0}, offsetMax: {x:0,y:0}},
       "Image": {color: {r:0.1, g:0.1, b:0.15, a:1}}
     }
   Only one panel active at a time (set_active: false for non-default panels).

3. LAYOUT STRUCTURE
   For each screen, create child GameObjects with layout components:
   - Vertical layouts: components_to_add ["VerticalLayoutGroup"]
     with properties: spacing, padding, childAlignment
   - Horizontal layouts: components_to_add ["HorizontalLayoutGroup"]
   - Content Size Fitter where elements should auto-size

4. UI ELEMENTS
   Create each element with manage_gameobject + manage_components:

   Text (TextMeshPro):
   - manage_gameobject create with components_to_add: ["TextMeshProUGUI"]
   - manage_components set_property: text, fontSize, alignment, color

   Buttons:
   - manage_gameobject create with components_to_add: ["Image", "Button"]
   - Create child TextMeshProUGUI for button label
   - manage_components set_property on Button: colors (normalColor, highlightedColor, pressedColor, disabledColor)

   Sliders:
   - manage_gameobject create with components_to_add: ["Slider"]
   - Create child objects: Background, Fill Area/Fill, Handle Slide Area/Handle
   - Configure Slider properties: minValue, maxValue, value, wholeNumbers

   Images:
   - manage_gameobject create with components_to_add: ["Image"]
   - Set color, sprite, raycastTarget as needed

5. POPUPS
   Create as Panel GameObjects with:
   - Higher sibling index (rendered on top)
   - set_active: false (shown by scripts at runtime)
   - Semi-transparent background overlay
   - Close button child

6. ANCHORING
   Use manage_components set_property on RectTransform:
   - Full-screen panels: anchorMin {0,0}, anchorMax {1,1}
   - Top bar: anchorMin {0,1}, anchorMax {1,1}, pivot {0.5, 1}
   - Bottom controls: anchorMin {0,0}, anchorMax {1,0}, pivot {0.5, 0}
   - Center content: anchorMin {0.5,0.5}, anchorMax {0.5,0.5} with sizeDelta
   - Buttons: fixed size with anchored position

BATCH STRATEGY

Use batch_execute to group operations. Each batch can hold up to 25 commands.
Organize batches by screen:
- Batch 1: Create all GameObjects for Screen A
- Batch 2: Configure all components for Screen A
- Batch 3: Create all GameObjects for Screen B
- etc.

Format for batch_execute:
```json
{
  "commands": [
    {"tool": "manage_gameobject", "params": {"action": "create", "name": "PlayButton", "parent": "MainMenuPanel", ...}},
    {"tool": "manage_gameobject", "params": {"action": "create", "name": "SettingsButton", "parent": "MainMenuPanel", ...}}
  ]
}
```

UI QUALITY RULES

- Consistent color scheme across all screens
- Minimum button size: 160×50 for touch-friendly targets
- Text hierarchy: Title (36-48pt), Subtitle (24-28pt), Body (18-22pt), Label (14-16pt)
- Visual feedback on all interactive elements (Button color transitions)
- Group related controls with spacing and visual separation
- Balance display should always be visible during gameplay
- Win amount display should be prominent and centered
- Spin button should be the most visually dominant interactive element

WIRING

After creating UI elements:
- Use manage_components action "add" to attach UI scripts to their GameObjects
  (e.g., SpinButton.cs on the spin button, WinDisplay.cs on the win text)
- Use manage_components action "set_property" to assign references between scripts
  (e.g., UIManager.spinButton = SpinButton GameObject)

VERIFY

After building all UI:
- Use find_gameobjects with searchMethod "by_component" to find all Canvas children
- Use read_console to check for missing references
- Report list of screens and element counts per screen

Confirm ready for Phase 5.