using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    public GameObject lightningColliderPrefab;

    public int lightningAmount;

    public int deltaTimeLightning;

    public float eventInterval; // not currently in use

    private float _timePassed;

    private void Update()
    {
        _timePassed += Time.deltaTime;
        if (_timePassed >= deltaTimeLightning)
        {
            if (lightningAmount > 0)
            {
                Lightning();
                lightningAmount--;
            }
            _timePassed = 0;
        }
    }
    void Lightning()
    {
        Instantiate(lightningColliderPrefab);
    }
}
