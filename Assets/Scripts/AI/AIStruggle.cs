using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStruggle : MonoBehaviour
{
    public bool AirStruggle = false;
    public float MovementSpeed = 4f;

    [Range(0f, 5f)]
    public float Frequency = 0.5f;

    Rigidbody _rb;
    TileGrid _tilemap;

    void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        _tilemap = TileGrid.FindTilemap();
    }

    public void StruggleTowards(TileGeneric tile)
    {
        if (!AirStruggle)
            return;

        if (_rb.velocity.y < Physics.gravity.y)
            return;

        // note: has issues while dropping, it gets affected by the player direction

        // Create a noise value to simulate air resistance
        float noise = Mathf.PerlinNoise(Time.time * Frequency, 0f);

        // Try to move towards target in intervals
        Vector3 direction = (tile.WorldCenter - transform.position).normalized;
        direction.y = 0;

        _rb.MovePosition(transform.position + direction * (MovementSpeed * 0.85f) * Time.fixedDeltaTime * (noise * MovementSpeed * 2));
    }
}
