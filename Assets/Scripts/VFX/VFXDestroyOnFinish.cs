using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXDestroyOnFinish : MonoBehaviour
{
    ParticleSystem _particleSystem;
    private float _time;
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        _time =+ Time.deltaTime;
        if (_particleSystem.main.duration <= _time)
        {
            Destroy(gameObject);
        }
    }
}
