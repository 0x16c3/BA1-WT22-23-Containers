using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfVanishing : MonoBehaviour
{
    [Tooltip("How long does it take for obj to vanish")]
    public float VanishTime = 60f;
    //[Tooltip("Time from object existing and starts to disappear")]
    //public float VanishDelay = 0f;
    public bool DieAfterVanish = false;

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
        if (_matColor.a == 0 && DieAfterVanish)
            Destroy(gameObject);
    }
}
