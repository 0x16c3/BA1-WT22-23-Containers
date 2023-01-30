using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AIVolcanoCatProjectile : MonoBehaviour
{
    Rigidbody _rigidbody;
    Vector3 _target;

    public AIVolcanoCat VolcanoCat;

    TileGrid _tileGrid;
    TileGeneric _tile;

    List<ContactPoint> _collisions = new List<ContactPoint>();

    bool _initialized = false;

    void OnEnable()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _tileGrid = TileGrid.FindTileGrid();
        _tile = _tileGrid.GetTile(transform.position);
    }

    void Update()
    {
        if (_initialized || VolcanoCat == null)
            return;

        var radius = VolcanoCat.ProjectileRadius;
        var speed = VolcanoCat.ProjectileSpeed;
        _target = GetRandomPoint(radius);

        JumpTowards(_target, speed);

        _initialized = true;
    }

    Vector3 GetRandomPoint(float radius)
    {
        // Get a random point in range 1 and radius from the center
        var randomPoint = Random.insideUnitSphere * radius;
        randomPoint += transform.position;

        randomPoint.y = 0;

        // if Too close to the center, try again
        if (Vector3.Distance(randomPoint, transform.position) < 1f)
            return GetRandomPoint(radius);

        return randomPoint;
    }

    void JumpTowards(Vector3 target, float speed)
    {
        // Calculate planar direction
        Vector3 direction = target - transform.position;
        direction = direction - (Vector3.Dot(direction, Vector3.up) * Vector3.up);

        float yDiff = direction.y;
        float distance = direction.magnitude;

        float angle0, angle1;
        bool targetInRange = CalcLaunchAngle(speed, distance, yDiff, Physics.gravity.magnitude, out angle0, out angle1);

        if (!targetInRange)
        {
            Debug.LogWarning("Target out of range");
            Destroy(gameObject);
            return;
        }

        // We use angle1 because it's the angle that gives us the highest angle of elevation
        float xAngle = angle1 * Mathf.Rad2Deg;
        float yAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(xAngle, yAngle, 0);
        _rigidbody.velocity = transform.up * speed;
    }

    public static bool CalcLaunchAngle(float speed, float distance, float yDiff, float gravity, out float angle0, out float angle1)
    {
        /*
        Calculate the launch angle for our projectile to hit the target.
        The launch angle is the angle between the horizontal and the vector that represents the launch velocity. 
        To find the launch angle, we can use the following equation:

        tan(angle) = (v^2 +- sqrt(v^4 - g(gx^2 + 2yv^2))) / (gx)
        where:
        v = projectile speed
        g = gravity
        x = horizontal distance to target
        y = vertical distance to target 

        The equation has two solutions, one for each possible launch angle
        */

        angle0 = angle1 = 0;

        float speedSquared = speed * speed;

        float operandA = Mathf.Pow(speed, 4);
        float operandB = gravity * (gravity * (distance * distance) + (2 * yDiff * speedSquared));

        // Target is not in range
        if (operandB > operandA)
            return false;

        float root = Mathf.Sqrt(operandA - operandB);

        angle0 = Mathf.Atan((speedSquared + root) / (gravity * distance));
        angle1 = Mathf.Atan((speedSquared - root) / (gravity * distance));

        return true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Add collision contacts to list
        _collisions.AddRange(collision.contacts);

        // Return if collided with trigger
        if (collision.collider.isTrigger || collision.gameObject == VolcanoCat.gameObject)
            return;

        // Check if hit a clip brush
        if (collision.gameObject.layer == LayerMask.NameToLayer("ClipBrush"))
        {
            // Get tile at position
            var tile = _tileGrid.GetTile(transform.position);

            // If tile is not null, set on fire
            var random = Random.Range(0f, 1f);
            if (tile != null && tile.GridPosition != _tile.GridPosition && tile.Damageable != null && random <= VolcanoCat.FireChance)
            {
                tile.Damageable.SetFire(true);
                Destroy(gameObject);
            }
            else if (tile != null && tile.Damageable != null)
            {
                tile.Damageable.Damage(VolcanoCat.Damage);
                Destroy(gameObject);
            }
            else
                Destroy(gameObject);
        }

        // If collided with the ground or any object that has IDamageable, destroy the projectile
        var damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(VolcanoCat.Damage);
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.color = Color.magenta;

        // Draw on top of everything
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        foreach (var collision in _collisions)
        {
            Handles.DrawWireDisc(collision.point, collision.normal, 0.3f);
        }
    }
#endif
}
