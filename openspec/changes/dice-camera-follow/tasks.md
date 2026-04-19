## 1. Script Creation

- [x] 1.1 Create `Assets/Scripts/DiceCameraFollow.cs` as a MonoBehaviour
- [x] 1.2 Add serialized fields: `smoothTime` (float, default 0.3), `padding` (float, default 0.5)
- [x] 1.3 Add private fields: `_defaultPosition` (Vector3), `_dice` (GameObject), `_velocityY` (float)
- [x] 1.4 Implement `Start`: store `transform.position` as `_defaultPosition`
- [x] 1.5 Implement `LateUpdate`: call `GameObject.Find("Dice")` if `_dice` is null, then run follow logic
- [x] 1.6 Implement frustum top-edge calculation using camera FOV and dice depth along camera forward
- [x] 1.7 Implement Y-follow: if dice Y > visible top edge, SmoothDamp camera Y toward `diceY - halfHeight + padding`
- [x] 1.8 Implement return-to-default: if dice Y <= visible top edge, SmoothDamp camera Y toward `_defaultPosition.y`
- [x] 1.9 Lock X and Z to `_defaultPosition.x` and `_defaultPosition.z` every frame ← (verify: script compiles with no errors, all fields and methods present)

## 2. Scene Setup

- [x] 2.1 Open the DiceRoll scene in the Unity Editor
- [x] 2.2 Select Main Camera in the hierarchy
- [x] 2.3 Add the `DiceCameraFollow` component to Main Camera
- [x] 2.4 Confirm `smoothTime` is 0.3 and `padding` is 0.5 in the Inspector
- [x] 2.5 Save the DiceRoll scene ← (verify: DiceRoll scene saved, Main Camera has DiceCameraFollow component with correct default values)

## 3. Validation

- [x] 3.1 Enter Play mode in the DiceRoll scene
- [x] 3.2 Click Roll and observe: camera should follow the dice upward when it bounces high
- [ ] 3.3 Verify camera returns smoothly to Y=6 after the dice descends back into frame
- [ ] 3.4 Verify camera X and Z remain at 0 and -4 throughout the roll
- [ ] 3.5 Verify camera rotation does not change during follow or return ← (verify: camera follow and return are smooth, X/Z/rotation unchanged, no console errors during play)
