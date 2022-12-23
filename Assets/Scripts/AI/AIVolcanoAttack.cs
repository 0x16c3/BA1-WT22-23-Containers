using UnityEngine;

public class AIVolcanoAttack : MonoBehaviour
{
    public float AttackInterval;
    public float AttackRadius;
    public LayerMask LayerMask;

    private ParticleSystem _particleSystem;

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        InvokeRepeating(nameof(Explosion), 3, AttackInterval);
    }

    void Explosion()
    {
        // it will play the explosion effect to signal the attac
        if (_particleSystem != null)
            _particleSystem.Play();

        // Finds colliders that are damageable and destroys them (will later change to damage)
        Collider[] targets = Physics.OverlapSphere(transform.position, AttackRadius, LayerMask);

        foreach (Collider target in targets)
        {
            // It won't destroy itself or other Volcano Cats
            if (target.gameObject.name == "Volcano Cat")
                continue;

            GameObject.Destroy(target.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, AttackRadius);
    }
}
