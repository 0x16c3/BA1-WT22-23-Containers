using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDamageable : MonoBehaviour, IDamageable
{
    public float tileHealth;

    public bool isFireEternal;

    private bool _isDamaged;

    private float _tileTotalDamage;

    private float _timeBetweenDamage;

    private float _timePassed;

    private float _singleDamage;

    private float _damageDealt;

    private void Update()
    {
        if (tileHealth <= 0)
        {
            // If health is below 0, tile is destroyed
            Object.Destroy(gameObject);
        }

        if (_isDamaged == true)
        {
            
            Damaging();
        }
        else if (_tileTotalDamage != 0)
        {
            // tileTotalDamage is 0 unless Damage() method has been called by a damaging force
            _singleDamage = _tileTotalDamage / 5f;
            _isDamaged = true;
            _timePassed = _timeBetweenDamage;
            // fire vfx to be enabled here
        }
    }

    public void Damage(float damage, float interval, UnityEngine.GameObject particleEffect) 
    {
            _tileTotalDamage = damage;
            _timeBetweenDamage = interval;
            Instantiate(particleEffect,transform.parent);
    }

    public void DamageOver()
    {
        // this method can be called by other behaviors like a water bucket to end the damage over time of fire for example.
        _tileTotalDamage = 0;
        _isDamaged = false;
        // fire vfx to be disabled here
    }

    void Damaging()
    {
        _timePassed += Time.deltaTime;
        if (_damageDealt >= _tileTotalDamage && isFireEternal == false)
        {
            DamageOver();
        }
        else if (_timeBetweenDamage <= _timePassed)
        {
            tileHealth -= _singleDamage;
            _damageDealt += _singleDamage;
            _timePassed = 0;
        }
    }
}
