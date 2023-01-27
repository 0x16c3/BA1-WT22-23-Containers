using UnityEngine;

public class AIVolcanoCat : MonoBehaviour
{
    public float FireRate = 3f;

    [Range(1f, 5f)]
    public float ProjectileRadius = 10f;
    [Range(1f, 90f)]
    public float ProjectileAngle = 45f;
    [Range(0f, 1f)]
    public float FireChance = 0.1f;

    public GameObject ProjectilePrefab;

    Collider _collider;

    float _lastFired = -1f;

    void OnEnable()
    {
        _collider = GetComponent<Collider>();
        _lastFired = -1f;
    }

    void Update()
    {
        if (Time.time - _lastFired > FireRate + Random.Range(-1f, 1f))
        {
            var projectile = Instantiate(ProjectilePrefab, transform.position + new Vector3(0, _collider.bounds.size.y, 0), Quaternion.identity);
            projectile.transform.parent = transform;
            _lastFired = Time.time;
        }
    }
}
