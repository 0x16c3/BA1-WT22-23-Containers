using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthShip : MonoBehaviour
{
    Slider _healthBar;
    ScoreSystem _scoreSystem;
    void Start()
    {
        _healthBar = GetComponent<Slider>();
        _scoreSystem = GameObject.Find("ScoreSystem").GetComponent<ScoreSystem>();
    }

    void Update()
    {
        float percentage = (float)_scoreSystem.ShipCondition / (float)_scoreSystem.MaxShipCondition;
        _healthBar.value = percentage;
    }
}
