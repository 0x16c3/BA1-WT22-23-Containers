using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsLightningHitbox : MonoBehaviour
{
    private EventsLightningEffect _parentCode;
    private bool _hasEnteredContact = false;
    void Start()
    {
        _parentCode = GetComponentInParent<EventsLightningEffect>();
        //remove parent after getting the code of the parent to avoid being affected by
        //the transform of the parent.
        transform.SetParent(null);
    }

    private void FixedUpdate()
    {
        if (_parentCode.TimePassed >= _parentCode.TimeBetweenWarningAndLightning - 0.1f &&
            _hasEnteredContact == false)
        {
                transform.position += Vector3.down;
        }

        else if (_parentCode.TimePassed >= _parentCode.TimeBetweenWarningAndLightning * 2)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(_parentCode.LightningDamage, _parentCode.DamageInterval);
            _hasEnteredContact = true;
        }

    }
}
