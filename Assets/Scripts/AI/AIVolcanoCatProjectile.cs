using UnityEngine;

public class AIVolcanoCatProjectile : MonoBehaviour
{
    Rigidbody _rigidbody;
    Vector3 _target;

    AIVolcanoCat _volcanoCat;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        var parent = transform.parent;

        _volcanoCat = parent.GetComponent<AIVolcanoCat>();
        if (_volcanoCat == null)
            Debug.LogError("No AIVolcanoCat component found on parent");

        var radius = _volcanoCat.ProjectileRadius;
        var angle = _volcanoCat.ProjectileAngle;
        JumpTowards(GetRandomPoint(radius), angle);
    }

    Vector3 GetRandomPoint(float radius)
    {
        var parentBounds = transform.parent.GetComponent<Collider>().bounds;

        // Get random point in radius, can be negative as well
        var randomDirection = Random.insideUnitCircle * transform.parent.position;
        var randomDistance = Random.Range(parentBounds.size.magnitude, radius);

        // Convert to world point
        var worldPoint = transform.parent.position + new Vector3(randomDirection.x, 0, randomDirection.y) * randomDistance;

        // If inside parent's bounds, retry
        if (parentBounds.Contains(worldPoint))
            return GetRandomPoint(radius);

        return worldPoint;
    }

    void JumpTowards(Vector3 target, float initialAngle)
    {
        float gravity = Physics.gravity.magnitude;
        float angle = initialAngle * Mathf.Deg2Rad;

        Vector3 planarTarget = new Vector3(target.x, 0, target.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

        // Distance between the objects on the same plane
        float distance = Vector3.Distance(planarTarget, planarPostion);
        float yOffset = transform.position.y - target.y;

        // Calculate the initial velocity needed to land the projectile on the target object
        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Apply rotation to the initial velocity
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (target.x > transform.position.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        _rigidbody.AddForce(finalVelocity * _rigidbody.mass, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        var damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.Damage(1);

        var tile = collision.gameObject.GetComponent<TileDamageable>();
        if (tile != null && !tile.OnFire && Random.value < _volcanoCat.FireChance)
            tile.SetFire(true);

        Destroy(gameObject);
    }
}
