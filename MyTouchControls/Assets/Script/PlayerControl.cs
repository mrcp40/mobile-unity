using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private Rigidbody _rigidbody = null;
    [SerializeField] private float _maxDistance = 100.0f;

    private Vector3 _moveVelocity = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = Vector2.zero;
        foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved || touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary)
            {
                moveInput = (touch.screenPosition - touch.startScreenPosition).normalized;
                moveInput = moveInput.normalized * Mathf.Clamp((moveInput.magnitude / _maxDistance), 0.0f, 1.0f) * _moveSpeed;
            }
        }

        _moveVelocity.x = moveInput.x;
        _moveVelocity.z = moveInput.y;
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = _moveVelocity * _moveSpeed;
    }
}
