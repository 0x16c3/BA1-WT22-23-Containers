using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{

    [Header("General Controller")]
    public int ChancePercent;
    public float ChanceAttemptInterval;
    public float EventInterval;
    [Space(20)]
    [Header("Lightning Event")]
    public GameObject LightningEffectColliderPrefab;
    public int LightningAmount;
    public int LightningEventDuration;
    public float LightningMinimumInterval;

    private float _eventTimePassed;
    private float _timeBetweenEvents;
    private float _chanceTimer;
    private int _randomEvent;

    private int _lightningAmountSet;

    private void Start()
    {
       _lightningAmountSet = LightningAmount;
    }
    private void Update()
    {
        if (_timeBetweenEvents >= EventInterval)
        {
            //Reset Amounts
            LightningAmount = _lightningAmountSet;

            //Pick a random number tied to an event
            _randomEvent = Random.Range(0, 1);
        }

        switch (_randomEvent)
        {
            case 0:
                if (LightningAmount > 0)
                {
                    LightningEvent();
                }

                break;

            case 1:
                //other event
                Debug.LogError("Range Exceeded.");
                break;
            case 2:
                //other event
                break;
        }

    }

    void LightningEvent()
    {
        _eventTimePassed += Time.deltaTime;
        // If lightning has not happened for the length of the event divided by the amount of lightning, it will happen
        if (_eventTimePassed >= LightningEventDuration / _lightningAmountSet)
        {
            LightningStrike();
        }

        // Chance for lightning to strike after a minimum waiting time between strikes
        else if (_eventTimePassed >= LightningMinimumInterval)
        {
            _chanceTimer += Time.deltaTime;
            if (_chanceTimer >= ChanceAttemptInterval)
            {
                float lightningChance = Random.Range(0, 100);
                _chanceTimer = 0;
                if (lightningChance <= ChancePercent)
                {
                    LightningStrike();
                }

            }

        }

    }

    void LightningStrike()
    {
        Vector3 outOfBounds = new Vector3(0, 0, 10);
        Instantiate(LightningEffectColliderPrefab, outOfBounds, Quaternion.identity);
        LightningAmount--;
        _eventTimePassed = 0;
    }
}
