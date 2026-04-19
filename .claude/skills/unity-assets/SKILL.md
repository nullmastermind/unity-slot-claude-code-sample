---
name: unity-assets
description: Phase 5 — Create materials, configure lighting, set up VFX, and camera.
---

You are executing Phase 5 of the Unity game builder pipeline. Scenes, GameObjects, scripts, and UI exist from prior phases. Your job is to create visual assets defined in the blueprint's Assets section.

1. MATERIALS

Use manage_material for each material:

Create:
  manage_material action "create" with params:
  - materialPath: "Assets/Materials/[name].mat"
  - shader: "Universal Render Pipeline/Lit" (URP) or "Standard" (Built-in)
  - color: {r, g, b, a}

Set properties:
  manage_material action "set_material_shader_property" with params:
  - materialPath, property (e.g., "_Metallic", "_Smoothness", "_EmissionColor"), value

Assign to GameObjects:
  manage_material action "assign_material_to_renderer" with params:
  - target: GameObject name/path
  - materialPath: "Assets/Materials/[name].mat"
  - slot: 0 (default)

Use batch_execute to create multiple materials at once.

2. LIGHTING

Create lights with manage_gameobject (Directional Light, Point Light, etc.).

Configure environment with manage_graphics:
- action "skybox_set_ambient": set ambient color/mode
- action "skybox_set_fog": configure fog if needed

Post-processing (URP/HDRP):
- manage_graphics action "volume_create": create a Volume GameObject
- manage_graphics action "volume_add_effect": add Bloom, Vignette, ColorAdjustments
- manage_graphics action "volume_set_effect": configure effect parameters
  Example: {action: "volume_set_effect", target: "PostProcessVolume", effect: "Bloom", property: "intensity", value: 1.5}

3. VFX

Use manage_vfx for particle effects:

Create particle system:
  manage_vfx action "particle_create" with params:
  - target: parent GameObject (or create new)
  - name: effect name

Configure particle properties:
  - action "particle_set_main": duration, startLifetime, startSpeed, startSize, startColor, maxParticles
  - action "particle_set_emission": rateOverTime, bursts
  - action "particle_set_shape": shapeType, radius, angle
  - action "particle_set_color_over_lifetime": gradient
  - action "particle_set_size_over_lifetime": curve
  - action "particle_set_renderer": renderMode, material

Stop emission by default (triggered at runtime):
  manage_vfx action "particle_stop" on each VFX object.

4. CAMERA

Use manage_camera:
- action "set_lens": fieldOfView (perspective) or orthographicSize (ortho)
- action "set_target": lookAt target if needed

For camera position/rotation, use manage_gameobject action "modify" on Main Camera:
- position, rotation as needed to frame the game

For background, use manage_components set_property on Camera component:
- clearFlags, backgroundColor

5. APPLY AND VERIFY

- Use find_gameobjects to confirm all visual objects exist
- Use read_console action "get" with types ["error", "warning"] to check for shader errors
- Report: materials created, lights configured, VFX count, camera setup

Confirm ready for Phase 6 (or Phase 7 if physics is skipped).