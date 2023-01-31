using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiHealthPlayer : MonoBehaviour
{
    int _representedAmount;
    RawImage _image;
    PlayerDamageable _damageable;
    void Start()
    {
        _representedAmount = Convert.ToInt32(gameObject.name);
        _image = GetComponent<RawImage>();
        GameObject player = GameObject.Find("Player With Model");
        _damageable = player.GetComponent<PlayerDamageable>();

        if (_damageable == null)
        {
            Debug.LogWarning("player damageable not found in " + player);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_damageable.Health < _representedAmount)
        {
            _image.enabled = false;
        }
        else
        {
            _image.enabled = true;
        }
        if (_damageable.Health >= 7)
        {
            _image.color = new Color(0,250,0);
        }
        else if (_damageable.Health < 7 && _damageable.Health >= 3)
        {
            _image.color = new Color(250, 250, 250);
        }
        else if (_damageable.Health < 3)
        {
            _image.color = new Color(250, 0, 0);
        }
    }
}
