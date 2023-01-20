using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsLightningStaticHitbox : MonoBehaviour
{
    private EventsLightningRayCast _parentCode;
    private Collider _collider;
    private List<GameObject> _hitObjects;
    void Start()
    {
        _hitObjects = new List<GameObject>();
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
        _parentCode = GetComponentInParent<EventsLightningRayCast>();
        //remove parent after getting the code of the parent to avoid being affected by
        //the transform of the parent.
        transform.SetParent(null);
    }

    private void FixedUpdate()
    {
        if (_parentCode.TimePassed >= _parentCode.TimeBetweenWarningAndLightning)
        {
            _collider.enabled = true;
        }

        if (_parentCode.TimePassed - _parentCode.TimeBetweenWarningAndLightning > _parentCode.LightningHitboxDuration)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !_hitObjects.Contains(other.gameObject))
        {
            damageable.Damage(_parentCode.LightningDamage, _parentCode.DamageInterval);
            _hitObjects.Add(other.gameObject);
        }

    }
}