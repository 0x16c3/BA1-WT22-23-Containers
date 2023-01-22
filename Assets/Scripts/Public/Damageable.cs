using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour, IDamageable
{
    public GameObject firePrefab;

    public float Health;
    public bool IsFireEternal;

    private GameObject _localFire;

    [HideInInspector] public bool _isDamaged;
    [HideInInspector] public float TimeBetweenDamage;

    private float _totalDamageReceived;
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
        if (Health <= 0)
        {
            // If health is below 0, tile is destroyed
            Object.Destroy(gameObject);
        }

        if (_isDamaged == true)
        {
            
            Damaging();
        }

        else if (_totalDamageReceived != 0)
        {
            // tileTotalDamage is 0 unless Damage() method has been called by a damaging force
            _singleDamage = _totalDamageReceived / 5f;
            _timePassed = TimeBetweenDamage;
            _isDamaged = true;
            _localFire.SetActive(true);
        }

    }

    public void Damage(float damage, float interval) 
    {
        _totalDamageReceived = damage;
        TimeBetweenDamage = interval;
    }

    public void DamageOver()
    {
        // this method can be called by other behaviors like a water bucket to end the damage over time of fire.
        _totalDamageReceived = 0;
        _isDamaged = false;
        _localFire.SetActive(false);
    }

    void Damaging()
    {
        _timePassed += Time.deltaTime;
        if (_damageDealt >= _totalDamageReceived && IsFireEternal == false)
        {
            DamageOver();
        }

        else if (TimeBetweenDamage <= _timePassed)
        {
            Health -= _singleDamage;
            _damageDealt += _singleDamage;
            _timePassed = 0;
        }

    }
}
