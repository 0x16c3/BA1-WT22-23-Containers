using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : Damaging
{
    [Tooltip("How often events occur (in seconds)")]
    [Range(0f, 500f)]
    public float EventsFrequency = 2f;
    [Tooltip("(in seconds)")]
    [Range(0.3f, 1f)]
    public float EventsFrequencyMultiplier = 1f;
    public int ShipHealth = 100; //or GameObject in the future

    [Header("Lightning")]
    public GameObject LightningEffect;
    [MinMaxRange(1f, 10f)]
    public MinMaxFloat LightningDamageRange;

    [Header("Flooding")]
    public GameObject FloodingEffect;
    [MinMaxRange(1f, 10f)]
    public MinMaxFloat FloodingDamageRange;

    [Header("Waves")]
    public GameObject WavesEffect;
    [MinMaxRange(1f, 10f)]
    public MinMaxFloat WavesDamageRange;

    float _timeSinceLastEvent = 0f;
    float _timeSinceLastFrequencyMultiplier = 0f;
    const int _eventsQuantity = 3;

    void Update()
    {
        _timeSinceLastEvent += Time.deltaTime;
        _timeSinceLastFrequencyMultiplier += Time.deltaTime;

        if (_timeSinceLastEvent >= EventsFrequency)
        {
            RandomEventOccur();
            if (_timeSinceLastFrequencyMultiplier >= 20f) //increases the difficulty every 20 seconds
            {
                EventsFrequency *= EventsFrequencyMultiplier;
                _timeSinceLastFrequencyMultiplier = 0f;
            }
            _timeSinceLastEvent = 0f;
        }
    }

    public override void Damage(float health, float damage, float interval, GameObject particleEffect)
    {
        base.Damage(health, damage, interval, particleEffect);
    }

    private void RandomEventOccur()
    {
        int rnd = Random.Range(1, _eventsQuantity);
        
        switch (rnd)
        {
            case 1:
                Lightning();
                break;
            case 2:
                Flooding();
                break;
            case 3:
                Waves();
                break;

        }
    }

    private void Lightning()
    {
        Debug.Log("Lighting hits");
        //Damage()
    }

    private void Flooding()
    {
        Debug.Log("Flooding happens");
    }

    private void Waves()
    {
        Debug.Log("Waves hit the ship");
    }
}
