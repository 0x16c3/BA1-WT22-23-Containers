using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsLightning : MonoBehaviour
{
    public float lightningDamage;

    public float fireInterval;

    public GameObject lightningVFX;

    TileGrid _tileMap;

    TileGeneric _tile;

    Collider _colliderTrigger;

    Vector3 _tileCenter;

    private int _tileXPosition;

    private int _tileYPosition;

    private float _timePassed;

    private void OnEnable()
    {
        _colliderTrigger = GetComponent<Collider>();
        _colliderTrigger.enabled = false;
        _tileMap = TileGrid.FindTilemap();
        _tileXPosition = Random.Range(-3, 10);
        _tileYPosition = Random.Range(-4, 4);
        _tile = _tileMap.GetTile(new Vector2Int(_tileXPosition, _tileYPosition));
        _tileCenter = _tile.WorldCenter;
        transform.position = _tileCenter;
    }
    private void Update()
    {
        _timePassed += Time.deltaTime;
        if (_timePassed >= 1)
        {
            Debug.Log("time passed");
            _colliderTrigger.enabled = true;
        }
        if (_timePassed > 1 + Time.fixedDeltaTime)
        {
            _timePassed = 0;
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("trigger");
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(lightningDamage,fireInterval,lightningVFX);
        }
    }
}
