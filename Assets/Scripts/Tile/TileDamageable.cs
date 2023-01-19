using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDamageable : MonoBehaviour, IDamageable
{
    public GameObject firePrefab;

    public float TileHealth;
    public bool IsFireEternal;

    private GameObject _localFire;

    private bool _isDamaged;

    private float _tileTotalDamage;
    private float _timeBetweenDamage;
    private float _timePassed;
    private float _singleDamage;
    private float _damageDealt;

    private void Start()
    {
        _localFire = Instantiate(firePrefab, transform);
        _localFire.SetActive(false);
    }

    private void Update()
    {
        if (TileHealth <= 0)
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
            _timePassed = _timeBetweenDamage;
            _isDamaged = true;
            _localFire.SetActive(true);
        }

    }

    public void Damage(float damage, float interval) 
    {
        _tileTotalDamage = damage;
        _timeBetweenDamage = interval;
        // Instantiate(particleEffect,transform.parent);
    }

    public void DamageOver()
    {
        // this method can be called by other behaviors like a water bucket to end the damage over time of fire.
        _tileTotalDamage = 0;
        _isDamaged = false;
        _localFire.SetActive(false);
    }

    void Damaging()
    {
        _timePassed += Time.deltaTime;
        if (_damageDealt >= _tileTotalDamage && IsFireEternal == false)
        {
            DamageOver();
        }

        else if (_timeBetweenDamage <= _timePassed)
        {
            TileHealth -= _singleDamage;
            _damageDealt += _singleDamage;
            _timePassed = 0;
        }

    }
}
