using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [Range(0f, 10f)]
    public float ShakeMagnitude = 3f;
    [Range(0f, 50f)]
    public float SailingFrequency = 4f;

    Vector3 _originalPosition;
    Quaternion _originalRotation;

    void OnEnable()
    {
        _originalPosition = transform.localPosition;
        _originalRotation = transform.localRotation;
    }

    void OnDisable()
    {
        transform.localPosition = _originalPosition;
        transform.localRotation = _originalRotation;
    }

    void Update()
    {
        SailingShake();
    }

    void SailingShake()
    {
        float freq = SailingFrequency / 10f;
        float mag = ShakeMagnitude / 10f;

        // Apply smooth shake using Mathf.PerlinNoise
        float x = Mathf.PerlinNoise(Time.time * freq, 0f) * 2f - 1f;
        float y = Mathf.PerlinNoise(0f, Time.time * freq) * 2f - 1f;
        float z = Mathf.PerlinNoise(Time.time * freq, Time.time * freq) * 2f - 1f;

        Vector3 shake = new Vector3(x, y, z) * mag;
        transform.localPosition = _originalPosition + shake;

        // Apply rotation
        transform.localRotation = _originalRotation * Quaternion.Euler(shake * 2f);
    }
}
