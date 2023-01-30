using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AIVolcanoCat : MonoBehaviour, ICustomBehavior
{
    
    [Tooltip ("Lava per minute")]public float FireRate = 3f;
    public int Damage = 1;

    public bool WanderWhileUsingAbility => true;

    [Range(1f, 5f)]
    public float ProjectileRadius = 3f;
    [Range(1f, 90f)]
    public float ProjectileSpeed = 45f;
    [Range(0f, 1f)]
    public float FireChance = 0.5f;

    public GameObject ProjectilePrefab;

    Collider _collider;

    float _nextFire = -1f;

    public bool Enabled
    {
        get => enabled;
        set => enabled = value;
    }

    void OnEnable()
    {
        _nextFire = Time.time + Random.Range(0f, 1f);
    }

    public void Ability()
    {
        if (_collider == null)
            _collider = GetComponent<Collider>();

        // Calculate range FireRate is objects per MINUTE
        var rate = (1f / FireRate) * 60f;

        if (Time.time > _nextFire)
        {
            var projectile = Instantiate(ProjectilePrefab, transform.position + new Vector3(0, _collider.bounds.size.y, 0), Quaternion.identity);

            var script = projectile.GetComponentInChildren<AIVolcanoCatProjectile>();
            if (!script)
                Debug.LogError("No AIVolcanoCatProjectile script found on projectile prefab");

            script.VolcanoCat = this;

            _nextFire = Time.time + rate + Random.Range(-0.1f, 0.1f);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!_collider)
        {
            _collider = GetComponent<Collider>();
            return;
        }

        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, ProjectileRadius);
    }
#endif
}
