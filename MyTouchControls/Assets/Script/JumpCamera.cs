using UnityEngine;

public class JumpCamera : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private PlatformSpawner _spawner;
    [SerializeField] private float _pitch = 42f;
    [SerializeField] private float _distance = 12.5f;
    [SerializeField] private float _focusHeight = 1.0f;
    [SerializeField, Range(0f, 1f)] private float _lookAhead = 0.3f;

    [SerializeField] private float _smoothTime = 0.25f;

    private Vector3 _focus;
    private Vector3 _focusVelocity;

    private void Start()
    {
        SnapToTarget();
    }

    private void LateUpdate()
    {
        if (_player == null)
            return;

        Vector3 target = GetFocusTarget();
        _focus = Vector3.SmoothDamp(_focus, target, ref _focusVelocity, _smoothTime);
        Apply();
    }

    public void SnapToTarget()
    {
        if (_player == null)
            return;

        _focus = GetFocusTarget();
        _focusVelocity = Vector3.zero;
        Apply();
    }

    private Vector3 GetFocusTarget()
    {
        Vector3 playerPosition = _player.position;
        Vector3 target = playerPosition;

        if (_spawner != null && _spawner.Next != null)
            target = Vector3.Lerp(playerPosition, _spawner.Next.transform.position, _lookAhead);

        target.y = _focusHeight;
        return target;
    }

    private void Apply()
    {
        Quaternion rotation = Quaternion.Euler(_pitch, 0f, 0f);
        Vector3 offset = rotation * (Vector3.back * _distance);

        transform.position = _focus + offset;
        transform.rotation = Quaternion.LookRotation(_focus - transform.position, Vector3.up);
    }
}
