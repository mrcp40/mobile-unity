using UnityEngine;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class JumpPlayer : MonoBehaviour
{
    [SerializeField] private PlatformSpawner _spawner;

   
    [SerializeField] private float _maxChargeTime = 1.0f;
    [SerializeField] private float _minJumpDistance = 0.6f;
    [SerializeField] private float _maxJumpDistance = 7.0f;

    [SerializeField] private float _minAirTime = 0.35f;
    [SerializeField] private float _maxAirTime = 0.55f;
    [SerializeField] private float _minJumpHeight = 1.1f;
    [SerializeField] private float _maxJumpHeight = 2.4f;

    [SerializeField] private float _fallGravity = 30f;
    [SerializeField] private float _fallStartSpeed = 1.5f;
    [SerializeField] private float _failHeight = -8f;

    public event System.Action<JumpPlatform> Landed;
    public event System.Action Fell;
    public event System.Action PressBegan;

    public bool AcceptInput { get; set; } = true;
    public bool IsCharging => _state == State.Charging;

    public float Charge01 =>_maxChargeTime <= 0f ? 1f : Mathf.Clamp01(_chargeTime / _maxChargeTime);

    private enum State
    {
        Idle,
        Charging,
        Jumping,
        Falling,
        Dead
    }

    private State _state = State.Idle;
    private float _chargeTime;
    private int _activeTouchId = -1;

    private Vector3 _jumpStart;
    private Vector3 _jumpDirection = Vector3.forward;
    private float _jumpDistance;
    private float _airTime;
    private float _jumpTime;
    private float _jumpVelocityY;
    private float _gravity;
    private float _startHeight;
    private float _fallSpeed;

    private void OnEnable()
    {
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Disable();
        _activeTouchId = -1;
    }

    private void Update()
    {
        ReadInput(out bool pressed, out bool released);

        if (pressed)
        {
            bool canCharge = AcceptInput && _state == State.Idle;
            PressBegan?.Invoke();

            if (canCharge && AcceptInput && _state == State.Idle)
            {
                _state = State.Charging;
                _chargeTime = 0f;
            }
        }

        switch (_state)
        {
            case State.Charging:
                _chargeTime = Mathf.Min(_chargeTime + Time.deltaTime, _maxChargeTime);
                if (released)
                    Jump();
                break;

            case State.Jumping:
                UpdateJump();
                break;

            case State.Falling:
                UpdateFall();
                break;

            case State.Idle:
            case State.Dead:
            default:
                break;
        }
    }

    private void ReadInput(out bool pressed, out bool released)
    {
        pressed = false;
        released = false;

        var touches = Touch.activeTouches;
        for (int i = 0; i < touches.Count; i++)
        {
            Touch touch = touches[i];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                if (_activeTouchId < 0)
                {
                    _activeTouchId = touch.touchId;
                    pressed = true;
                }
            }
            else if (touch.touchId == _activeTouchId &&
                     (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                      touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled))
            {
                _activeTouchId = -1;
                released = true;
            }
        }

        if (!pressed && !released && _activeTouchId < 0 && Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                pressed = true;

            if (Mouse.current.leftButton.wasReleasedThisFrame)
                released = true;
        }
    }

    private void Jump()
    {
        Vector3 direction = _spawner != null
            ? _spawner.GetJumpDirection(transform.position)
            : Vector3.forward;

        float t = Charge01;

        _jumpDistance = Mathf.Lerp(_minJumpDistance, _maxJumpDistance, t);
        _airTime = Mathf.Lerp(_minAirTime, _maxAirTime, t);

        
        float height = Mathf.Lerp(_minJumpHeight, _maxJumpHeight, t);
        _gravity = 8f * height / (_airTime * _airTime);
        _jumpVelocityY = 4f * height / _airTime;

        _jumpStart = transform.position;
        _startHeight = _jumpStart.y;
        _jumpDirection = direction;
        _jumpTime = 0f;
        _chargeTime = 0f;
        _state = State.Jumping;

        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private void UpdateJump()
    {
        _jumpTime += Time.deltaTime;

        if (_jumpTime < _airTime)
        {
            float k = _jumpTime / _airTime;

            Vector3 position = _jumpStart + _jumpDirection * (_jumpDistance * k);
            position.y = _startHeight
                         + _jumpVelocityY * _jumpTime
                         - 0.5f * _gravity * _jumpTime * _jumpTime;

            transform.position = position;
            return;
        }

        Vector3 end = _jumpStart + _jumpDirection * _jumpDistance;
        end.y = _startHeight;
        transform.position = end;

        ResolveLanding();
    }

    private void ResolveLanding()
    {
        JumpPlatform platform = _spawner != null
            ? _spawner.FindPlatformAt(transform.position)
            : null;

        if (platform != null)
        {
            _state = State.Idle;
            Landed?.Invoke(platform);
            return;
        }

        _state = State.Falling;
        _fallSpeed = _fallStartSpeed;
    }

    private void UpdateFall()
    {
        _fallSpeed += _fallGravity * Time.deltaTime;
        transform.position += Vector3.down * (_fallSpeed * Time.deltaTime);

        if (transform.position.y <= _failHeight)
        {
            _state = State.Dead;
            Fell?.Invoke();
        }
    }
    public void ResetOn(Vector3 platformTopCenter)
    {
        transform.position = platformTopCenter;
        transform.rotation = Quaternion.identity;

        _state = State.Idle;
        _chargeTime = 0f;
        _fallSpeed = 0f;
        _jumpTime = 0f;
    }
}
