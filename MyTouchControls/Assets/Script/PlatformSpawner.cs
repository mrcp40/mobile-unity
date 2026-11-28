using System.Collections.Generic;
using UnityEngine;


public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] private float _minCenterDistance = 2.8f;
    [SerializeField] private float _maxCenterDistance = 4.8f;

    [SerializeField, Range(0f, 45f)] private float _maxTurnAngle = 20f;

    [SerializeField] private float _edgePadding = 0.15f;

    [SerializeField] private Vector3 _startPosition = Vector3.zero;
    [SerializeField] private float _startRadius = 1.3f;

    [SerializeField] private int _keepBehind = 2;
    [SerializeField] private Transform _platformParent;

    public JumpPlatform Current { get; private set; }
    public JumpPlatform Next { get; private set; }

    private readonly List<JumpPlatform> _platforms = new List<JumpPlatform>();
    private int _nextIndex;

   
    public void ResetGame()
    {
        for (int i = 0; i < _platforms.Count; i++)
        {
            if (_platforms[i] != null)
                Destroy(_platforms[i].gameObject);
        }

        _platforms.Clear();
        _nextIndex = 0;

        Current = CreatePlatform(_startPosition, _startRadius);
        Next = SpawnNext();
    }

    public JumpPlatform SpawnNext()
    {
        if (Current == null)
            return null;

        Vector3 position = PickNextPosition(Current, out float radius);
        Next = CreatePlatform(position, radius);
        return Next;
    }

    public void Advance()
    {
        if (Next == null)
            return;

        Current = Next;
        Next = null;

        SpawnNext();
        CleanupBehind();
    }

    
    public JumpPlatform FindPlatformAt(Vector3 worldPoint)
    {
        JumpPlatform best = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < _platforms.Count; i++)
        {
            JumpPlatform platform = _platforms[i];
            if (platform == null || !platform.Contains(worldPoint))
                continue;

            float distance = platform.DistanceToCenter(worldPoint);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = platform;
            }
        }

        return best;
    }

    public Vector3 GetJumpDirection(Vector3 from)
    {
        if (Next == null)
            return Vector3.forward;

        Vector3 delta = Next.transform.position - from;
        delta.y = 0f;
        return delta.sqrMagnitude > 0.0001f ? delta.normalized : Vector3.forward;
    }

    private Vector3 PickNextPosition(JumpPlatform from, out float radius)
    {
        radius = 1.0f;

        for (int attempt = 0; attempt < 16; attempt++)
        {

            float minDistance = Mathf.Max(_minCenterDistance, from.Radius + radius + _edgePadding);
            float maxDistance = Mathf.Max(minDistance, _maxCenterDistance);
            float distance = Random.Range(minDistance, maxDistance);

            
            Vector3 candidate = from.transform.position+  Vector3.forward * distance;
            candidate.y = 0f;

            if (!OverlapsAny(candidate, radius))
                return candidate;
        }

        Vector3 fallback = from.transform.position
                           + Vector3.forward * (_maxCenterDistance + from.Radius + radius + 1f);
        fallback.y = 0f;
        return fallback;
    }

    private bool OverlapsAny(Vector3 candidate, float radius)
    {
        for (int i = 0; i < _platforms.Count; i++)
        {
            JumpPlatform platform = _platforms[i];
            if (platform == null)
                continue;

            float minDistance = platform.Radius + radius + _edgePadding;
            Vector3 delta = platform.transform.position - candidate;
            delta.y = 0f;

            if (delta.sqrMagnitude < minDistance * minDistance)
                return true;
        }

        return false;
    }

    private JumpPlatform CreatePlatform(Vector3 topCenter, float radius)
    {
        GameObject root = new GameObject("Platform " + _nextIndex);
        root.transform.SetParent(_platformParent != null ? _platformParent : transform, false);
        root.transform.position = topCenter;

       
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        visual.name = "Visual";
        visual.transform.SetParent(root.transform, false);
        visual.transform.localPosition = new Vector3(0f, -0.5f, 0f);

       
        Collider collider = visual.GetComponent<Collider>();
        if (collider != null)
            Destroy(collider);

        JumpPlatform platform = root.AddComponent<JumpPlatform>();
        platform.Setup(_nextIndex, radius);

        _nextIndex++;
        _platforms.Add(platform);
        return platform;
    }

    private void CleanupBehind()
    {
        if (Current == null)
            return;

        int minIndex = Current.Index - _keepBehind;

        for (int i = _platforms.Count - 1; i >= 0; i--)
        {
            JumpPlatform platform = _platforms[i];

            if (platform == null)
            {
                _platforms.RemoveAt(i);
                continue;
            }

            if (platform == Current || platform == Next)
                continue;

            if (platform.Index < minIndex)
            {
                _platforms.RemoveAt(i);
                Destroy(platform.gameObject);
            }
        }
    }
}