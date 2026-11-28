using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [SerializeField] private Transform _visual;

    public int Index { get; private set; }
    public float Radius { get; private set; } = 1f;

    private void Awake()
    {
        if (_visual == null && transform.childCount > 0)
            _visual = transform.GetChild(0);
    }


    public void Setup(int index, float radius)
    {
        Index = index;
        Radius = Mathf.Max(0.1f, radius);

        if (_visual == null && transform.childCount > 0)
            _visual = transform.GetChild(0);

        if (_visual != null)
        {
            _visual.localScale = new Vector3(Radius * 2f, 0.5f, Radius * 2f);
        }
    }

    public bool Contains(Vector3 worldPoint)
    {
        float dx = worldPoint.x - transform.position.x;
        float dz = worldPoint.z - transform.position.z;
        return dx * dx + dz * dz <= Radius * Radius;
    }

    public float DistanceToCenter(Vector3 worldPoint)
    {
        float dx = worldPoint.x - transform.position.x;
        float dz = worldPoint.z - transform.position.z;
        return Mathf.Sqrt(dx * dx + dz * dz);
    }
}