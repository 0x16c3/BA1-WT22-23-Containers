using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDamageable : MonoBehaviour, IDamageable
{
    public float tileHealth;

    private float _timePassed = 0f;

    private void Update()
    {
        _timePassed += Time.deltaTime;

        if (tileHealth <= 0)
        {
            Object.Destroy(gameObject);
        }
    }
    public void Damage(float damage, float interval, UnityEngine.GameObject particleEffect) 
    {
        if (_timePassed >= interval)
        {
            tileHealth -= damage;
            Instantiate(particleEffect,transform.parent);
            _timePassed = 0f;
        }
    }
}
