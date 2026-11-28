using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class TouchControls : MonoBehaviour
{
    public enum SwipDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    [SerializeField] private float _minHoldDuration = 0.5f;
    [SerializeField] private float _minSwipDistance = 50.0f;
    [SerializeField] private Color _objectColor = Color.yellow;

    private Color _defaultObjectColor = Color.white;
    private float _holdDuration = 0f;
    private bool _isObjectSelected = false;
    private Vector2 _touchPosition= Vector2.zero;
    private GameObject _selectedObject= null;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
        {
            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    {
                        OnPress();
                        _touchPosition = touch.screenPosition;
                        break;
                    }
                case UnityEngine.InputSystem.TouchPhase.Moved:       
                    {
                        OnHold();
                        _touchPosition = touch.screenPosition;
                        break;
                    }
                case UnityEngine.InputSystem.TouchPhase.Stationary:
                    {
                        OnHold();
                        _touchPosition = touch.screenPosition;
                        break;
                    }
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    {
                        OnRelease();
                        Vector2 releaseDelta = touch.screenPosition - touch.startScreenPosition;
                        OnSwipe(releaseDelta);
                        break;
                    }
                case UnityEngine.InputSystem.TouchPhase.Ended:
                    {
                        OnRelease();
                        Vector2 releaseDelta = touch.screenPosition - touch.startScreenPosition;
                        OnSwipe(releaseDelta);
                        break;
                    }
                default:
                    break;
            }
        }

        if(_holdDuration>_minHoldDuration&&!_isObjectSelected)
        {
            Ray screenRay = Camera.main.ScreenPointToRay(_touchPosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(screenRay, out hitInfo,100.0f))
            {
                if(hitInfo.collider != null&&hitInfo.collider.gameObject!=null)
                {
                    MeshRenderer renderer=hitInfo.collider.gameObject.GetComponent<MeshRenderer>();
                    if(renderer!=null)
                    {
                        _defaultObjectColor = renderer.material.color;
                        renderer.material.color = _objectColor;
                        _selectedObject=hitInfo.collider.gameObject;

                        _isObjectSelected = true;
                    }
                }
            }
        }
    }

    private void OnPress()
    {
        _holdDuration = 0.0f;
        Debug.Log("Touch is PRESSED");
    }

    private void OnRelease()
    {
        if(_selectedObject!=null)
        {
            MeshRenderer renderer = _selectedObject.GetComponent<MeshRenderer>();
            renderer.material.color = _defaultObjectColor;
            _selectedObject=null;
            _isObjectSelected=false;
        }
        _holdDuration=0.0f;
        Debug.Log("Touch is RELEASED");
    }

    private void OnHold()
    {
        _holdDuration += Time.deltaTime;
        Debug.Log("Touch is HELD " + _holdDuration.ToString("F2"));
    }

    private void OnSwipe(Vector2 releaseDelta)
    {
        Vector2 dir = releaseDelta.normalized;
        float dotUp = Vector2.Dot(dir, Vector2.up);
        float dotRight = Vector2.Dot(dir, Vector2.right);
        if (Mathf.Abs(dotUp) >= Mathf.Abs(dotRight))
        {
            if (dotUp > 0.0f)
            {
                ProcessSwipe(SwipDirection.Up);
            }
            else
            {
                ProcessSwipe(SwipDirection.Down);
            }
        }
        else
        {
            if (dotRight > 0.0f)
            {
                ProcessSwipe(SwipDirection.Right);
            }
            else
            {
                ProcessSwipe(SwipDirection.Left);
            }
        }

        float speed = releaseDelta.magnitude / _holdDuration;

        Debug.Log("Swipe SPEED " + speed.ToString("F2") + "pix/s");
    }

    private void ProcessSwipe(SwipDirection direction)
    {
        switch (direction)
        {
            case SwipDirection.Up: Debug.Log("Swipe UP"); break;
            case SwipDirection.Down: Debug.Log("Swipe DOWN"); break;
            case SwipDirection.Left: Debug.Log("Swipe LEFT"); break;
            case SwipDirection.Right: Debug.Log("Swipe RIGHT"); break;
            default: break;
        }
    }
}
