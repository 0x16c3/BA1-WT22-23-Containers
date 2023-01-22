using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GardeningGoatPooping : MonoBehaviour
{
    public float Interval = 2f;
    public float Duration = 8f;
    public GameObject GrassPrefab;
    private float _elapsedTime;

    void Start()
    {
        _elapsedTime = 0f;
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime < Duration)
        {
            if (_elapsedTime >= Interval)
            {
                Poop();
                _elapsedTime = 0f;
            }
        }
    }

    void Poop()
    {
        GameObject poop = Instantiate(GrassPrefab,transform.position,Quaternion.identity);
        if (poop != null)
        {
            Vector3 poopPos = poop.transform.position;

            poopPos.x = Rounder(poopPos.x);
            poopPos.z = Rounder(poopPos.z);
            poopPos.y = 0;
            poop.transform.position = poopPos;
        }

    }
    float Rounder(float pos)
    {
        if (pos <= Mathf.Round(pos))
        {
            pos = Mathf.Round(pos) - 0.5f;
        }
        else if (pos > Mathf.Round(pos))
        {
            pos = Mathf.Round(pos) + 0.5f;
        }

        return pos;
    }
}


