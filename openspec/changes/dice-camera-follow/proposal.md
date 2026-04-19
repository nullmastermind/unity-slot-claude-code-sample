## Why

The DiceRoll scene's fixed camera loses the dice when it bounces high, breaking visual feedback during a key game moment. The camera needs to follow the dice vertically so the player always sees it in frame.

## What Changes

- New script `DiceCameraFollow.cs` added to `Assets/Scripts/`
- Script attached to Main Camera in the DiceRoll scene
- Camera gains vertical follow behavior: tracks dice Y position when it exits the top of the visible frustum, returns to default position when dice is back in view
- No changes to `DiceRoller.cs` or any other existing scripts

## Capabilities

### New Capabilities

- `dice-camera-follow`: Vertical camera follow for the DiceRoll scene — detects when the dice leaves the top of the camera frustum and smoothly adjusts camera Y to keep it in frame, then returns to default when the dice is back in view

### Modified Capabilities

## Impact

- `Assets/Scripts/DiceCameraFollow.cs` — new file
- DiceRoll scene — Main Camera gets `DiceCameraFollow` component attached
- No runtime dependencies on other scripts; uses `GameObject.Find("Dice")` to locate the dice created by `DiceRoller.cs`
