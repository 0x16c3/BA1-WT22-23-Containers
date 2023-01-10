using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfVanishing : MonoBehaviour
{
    public float VanishTime = 90f;

    float _timePassed = 0;
    Color _matColor;

    private void Start()
    {
        _matColor = GetComponent<MeshRenderer>().material.color;
    }

    void Update()
    {
        _timePassed += Time.deltaTime;
        float percentageComplete = _timePassed / VanishTime;
        _matColor.a = Mathf.Lerp(1, 0, percentageComplete);
        GetComponent<MeshRenderer>().material.color = _matColor;
        if (_matColor.a == 0)
            Destroy(gameObject);
    }
}
