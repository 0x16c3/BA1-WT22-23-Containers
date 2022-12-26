using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damaging : MonoBehaviour
{
    private float _timePassed = 0f;

    private void Update()
    {
        _timePassed += Time.deltaTime;
    }

    public virtual void Damage (float health, float damage, float interval, GameObject particleEffect)
    {
        if (_timePassed >= interval)
        {
            health -= damage;
            Instantiate(particleEffect);
            _timePassed = 0f;
        }
    }
}
