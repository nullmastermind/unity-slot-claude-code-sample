## ADDED Requirements

### Requirement: Camera follows dice vertically when out of frame
The system SHALL move the camera's Y position upward to keep the dice within the visible frustum when the dice rises above the camera's visible top edge. Only the Y axis SHALL move; X, Z, and rotation SHALL remain fixed at their default values.

#### Scenario: Dice rises above visible area
- **WHEN** the dice Y position exceeds the camera's calculated visible top edge (with padding)
- **THEN** the camera Y position SHALL increase smoothly using SmoothDamp to keep the dice in frame

#### Scenario: Dice returns to visible area
- **WHEN** the dice Y position is within the camera's visible frustum
- **THEN** the camera Y position SHALL smoothly return to its default Y value using SmoothDamp

#### Scenario: Camera X and Z are never modified
- **WHEN** the camera is following the dice upward or returning to default
- **THEN** the camera X and Z positions SHALL remain equal to their default values at all times

#### Scenario: Camera rotation is never modified
- **WHEN** the camera follow script is active
- **THEN** the camera rotation SHALL remain unchanged from its initial value

### Requirement: Default position is stored on Start
The script SHALL record the camera's world position at Start as the default position to return to.

#### Scenario: Default position captured
- **WHEN** the scene loads and the script's Start method runs
- **THEN** the camera's current world position SHALL be stored as the default position

### Requirement: Dice reference is acquired at runtime
The script SHALL locate the dice GameObject by name "Dice" at runtime without requiring a serialized reference or changes to DiceRoller.cs.

#### Scenario: Dice not yet created
- **WHEN** LateUpdate runs before the dice GameObject exists in the scene
- **THEN** the script SHALL skip camera adjustment without error

#### Scenario: Dice found and cached
- **WHEN** a GameObject named "Dice" is found via GameObject.Find
- **THEN** the reference SHALL be cached and reused each subsequent frame without calling Find again

### Requirement: Frustum-based visibility check
The script SHALL calculate the visible top Y boundary at the dice's depth using the camera's FOV and the dice's distance along the camera's forward axis, plus a configurable vertical padding value.

#### Scenario: Padding keeps dice away from edge
- **WHEN** the dice Y position equals the raw frustum top edge
- **THEN** the camera SHALL still adjust upward because the padding offset places the effective threshold below the raw edge

### Requirement: Smooth camera movement
Camera position changes SHALL use Mathf.SmoothDamp with a smoothTime of approximately 0.3 seconds for both follow and return-to-default transitions.

#### Scenario: Follow transition is smooth
- **WHEN** the dice exits the top of the frame
- **THEN** the camera SHALL not snap instantly but SHALL accelerate and decelerate smoothly toward the target Y

#### Scenario: Return transition is smooth
- **WHEN** the dice re-enters the visible area
- **THEN** the camera SHALL smoothly decelerate back to the default Y position rather than snapping
