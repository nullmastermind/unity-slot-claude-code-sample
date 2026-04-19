using UnityEngine;

/// <summary>
/// Attaches to Main Camera. Follows the runtime-created "Dice" GameObject upward
/// when it leaves the top of the camera frustum, then returns to the default position.
/// Only the Y axis moves; X, Z, and rotation are locked to their default values.
/// </summary>
[RequireComponent(typeof(Camera))]
public class DiceCameraFollow : MonoBehaviour
{
    [SerializeField] private float followSmoothTime = 0.05f;
    [SerializeField] private float returnSmoothTime = 0.3f;
    [SerializeField] private float padding = 0.5f;

    private Vector3 _defaultPosition;
    private Vector3 _defaultForward;
    private GameObject _dice;
    private float _velocityY;
    private Camera _camera;
    private float _defaultHalfHeight;

    void Start()
    {
        _defaultPosition = transform.position;
        _defaultForward = transform.forward;
        _camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (_dice == null)
            _dice = GameObject.Find("Dice");

        if (_dice == null)
            return;

        Vector3 dicePos = _dice.transform.position;

        // Stable frustum calc from default position (no feedback loop)
        float distance = Vector3.Dot(dicePos - _defaultPosition, _defaultForward);
        if (distance <= 0f)
            return;

        float halfHeight = distance * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

        // Continuous target: where camera Y should be to keep dice at top of frame
        float desiredY = dicePos.y - halfHeight + padding;

        // Never go below default — clamp creates a smooth continuous target
        float targetY = Mathf.Max(desiredY, _defaultPosition.y);

        // Pick smooth time based on whether camera needs to be above default
        float smooth = (targetY > _defaultPosition.y + 0.01f) ? followSmoothTime : returnSmoothTime;

        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref _velocityY, smooth);

        transform.position = new Vector3(_defaultPosition.x, newY, _defaultPosition.z);
    }
}
