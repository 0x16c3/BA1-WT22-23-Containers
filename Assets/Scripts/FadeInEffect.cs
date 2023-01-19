using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInEffect : MonoBehaviour
{
    public float VanishTime = 90f;

    float _timePassed = 0;
    Color _matColor, _initMatColor;

    private void Start()
    {
        _initMatColor = _matColor = GetComponent<MeshRenderer>().material.color;
        _matColor.a = 0;
        GetComponent<MeshRenderer>().material.color = _matColor;
    }

    void Update()
    {
        _timePassed += Time.deltaTime;
        float percentageComplete = _timePassed / VanishTime;
        _matColor.a = Mathf.Lerp(0, 1, percentageComplete);
        GetComponent<MeshRenderer>().material.color = _matColor;
    }
}
