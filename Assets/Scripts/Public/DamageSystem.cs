using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageSystem : MonoBehaviour
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
            Instantiate(particleEffect,transform);
            _timePassed = 0f;
        }
    }
}
public interface IDamageable
{
    void Damage(float damage, float interval, GameObject particleEffect);
}