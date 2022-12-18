using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class VolcanoCatAi : MonoBehaviour
{
    public float attackInterval;
    public float attackRadius;
    public LayerMask layerMask;
    private Collider[] _attackCollider;
    private ParticleSystem _particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        //It will attack after (attackInterval) amount of seconds
        InvokeRepeating(nameof(Explosion), 3, attackInterval);
    }

    void Explosion()
    {
        // it will play the explosion effect to signal the attack
        _particleSystem.Play();

        // Finds colliders that are damageable and destroys them (will later change to damage
        // )
        _attackCollider = Physics.OverlapSphere(transform.position, attackRadius, layerMask);

        foreach (Collider _otherCollider in _attackCollider)
        {
            // it won't destroy itself or other Volcano Cats
            if (_otherCollider.gameObject.name != "Volcano Cat")
            {
                GameObject.Destroy(_otherCollider.gameObject);
            }
            
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,attackRadius);
    }
}
