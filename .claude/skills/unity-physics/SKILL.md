---
name: unity-physics
description: Phase 6 — Configure physics settings, colliders, rigidbodies, and physics layers. Conditional — skip if blueprint says not needed.
---

You are executing Phase 6 of the Unity game builder pipeline. This phase is conditional. Check the blueprint — if it says physics is not needed, announce "Phase 6 skipped — no physics required" and confirm ready for Phase 7.

If physics IS needed:

1. PHYSICS SETTINGS
   Use manage_physics action "set_settings" to configure:
   - gravity (Vector3 for 3D, Vector2 for 2D)

   Use manage_physics action "set_collision_matrix" for layer interactions.

2. COLLIDERS
   Use manage_components action "add" to add colliders to GameObjects:
   - component_type: "BoxCollider" / "BoxCollider2D" for simple shapes
   - component_type: "SphereCollider" / "CircleCollider2D" for round objects
   - component_type: "MeshCollider" / "PolygonCollider2D" for complex shapes

   Use manage_components action "set_property" to configure:
   - isTrigger: true where overlap detection without physics response is needed
   - size, center, radius as needed

3. RIGIDBODIES
   Use manage_components action "add" with component_type "Rigidbody" / "Rigidbody2D".
   Use manage_physics action "configure_rigidbody" with params:
   - target: GameObject
   - mass, drag, angularDrag, useGravity, isKinematic, constraints

4. PHYSICS MATERIALS
   Use manage_physics action "create_physics_material" with params:
   - name, dynamicFriction, staticFriction, bounciness, frictionCombine, bounceCombine

   Use manage_physics action "assign_physics_material" to apply to colliders.

5. VERIFY
   Use manage_physics action "validate" to check scene physics setup.
   Use read_console action "get" with types: ["warning"] to check for physics warnings.
   Report: colliders added, rigidbodies configured, layers set up.

Confirm ready for Phase 7.