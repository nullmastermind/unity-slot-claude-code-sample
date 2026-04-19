## Context

The DiceRoll scene uses a fixed camera at (0, 6, -4) with ~50° downward tilt and FOV 60. The dice spawns at Y=3 and can bounce significantly higher due to physics material bounciness (dice: 0.4, ground: 0.3). When the dice exceeds the camera's visible top edge, it leaves the frame entirely, breaking the player's visual connection to the roll outcome.

The dice is created at runtime by `DiceRoller.cs` as a GameObject named "Dice". No camera scripts currently exist.

## Goals / Non-Goals

**Goals:**
- Keep the dice visible during high bounces by moving the camera up on the Y axis
- Return the camera smoothly to its default position once the dice is back in view
- Self-contained script with no modifications to existing code
- Smooth, non-jarring movement using `Mathf.SmoothDamp`

**Non-Goals:**
- Following the dice on X or Z axes
- Rotating the camera to track the dice
- Zooming or FOV changes
- Following the dice below the default camera position
- Any behavior after the dice has settled (camera returns to default and stays)

## Decisions

**Frustum top-edge calculation via `Camera.VerticalToHorizontalFieldOfView` / trigonometry**

The visible top Y at the dice's world Z position is calculated from the camera's frustum. Given the camera position, FOV, and the dice's distance along the camera's forward axis, the half-height of the visible area at that depth can be derived with:

```
distance = Vector3.Dot(dicePos - cameraPos, camera.transform.forward)
halfHeight = distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad)
visibleTopY = cameraPos.y + halfHeight - padding
```

This is more accurate than a fixed threshold and adapts if FOV or camera position changes.

Alternative considered: fixed Y threshold (e.g., "if dice.y > 7, follow"). Rejected because it's brittle — it doesn't account for the actual frustum geometry and would need manual tuning if camera settings change.

**SmoothDamp on Y only**

`Mathf.SmoothDamp` with `smoothTime ≈ 0.3s` gives responsive but smooth tracking. Only the Y component of `transform.position` is modified; X and Z are locked to default values each frame to prevent drift.

**`GameObject.Find("Dice")` with null guard**

The dice is created at runtime, so the script caches the reference after finding it. A null check each frame handles the window between scene load and dice creation, and also handles the brief moment after the dice is destroyed (if ever). No dependency on `DiceRoller.cs` is introduced.

**LateUpdate for camera movement**

Using `LateUpdate` ensures the camera moves after physics and all other scripts have updated the dice position for that frame, preventing one-frame lag artifacts.

## Risks / Trade-offs

- `GameObject.Find` is called once per frame until the dice is found, then cached. This is a minor cost during the first few frames only. → Mitigation: cache on first successful find, skip find once cached.
- If the dice name changes in `DiceRoller.cs`, the follow script silently stops working. → Mitigation: document the name dependency in a comment in the script; the proposal notes no changes to `DiceRoller.cs` are needed.
- The frustum calculation assumes the camera's forward vector points roughly toward the dice. If the camera were rotated drastically, the depth projection could be inaccurate. → Acceptable: camera rotation is fixed and the geometry is well-defined for this scene.

## Migration Plan

1. Create `Assets/Scripts/DiceCameraFollow.cs`
2. Wait for Unity compilation to complete with no errors
3. Open the DiceRoll scene
4. Select Main Camera and add the `DiceCameraFollow` component
5. Save the scene
6. Enter Play mode and roll the dice to verify follow behavior and return-to-default behavior
7. Rollback: remove the component from Main Camera and delete the script file
