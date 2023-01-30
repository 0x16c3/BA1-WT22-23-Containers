using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanksFadeOut : MonoBehaviour
{
    MeshRenderer _mr;
    int _plankValue;
    void Start()
    {
        _plankValue = Convert.ToInt32(gameObject.name);
        _mr = GetComponent<MeshRenderer>();
    }
    private void Update()
    {
        if (_plankValue >= PlayerNailing.HasMaterials)
        {
            _mr.enabled = false;
        }
        else
        {
            _mr.enabled = true;
        }
    }
}
