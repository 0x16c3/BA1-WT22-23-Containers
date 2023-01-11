using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsLightning : MonoBehaviour
{
    public float lightningDamage;

    public float damageInterval;

    public float timeBetweenWarningAndLightning = 1;

    public GameObject lightningVFX;

    public GameObject lightningWarningVFX;

    GameObject _localGlow;

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

        _localGlow = Instantiate(lightningWarningVFX, transform);
        _localGlow.transform.position = new Vector3 (transform.position.x, transform.position.y + 0.05f, transform.position.z);
    }
    private void Update()
    {
        _timePassed += Time.deltaTime;
        if (_timePassed >= timeBetweenWarningAndLightning)
        {
            Debug.Log("time passed");
            _colliderTrigger.enabled = true;
        }
        if (_timePassed > timeBetweenWarningAndLightning + 0.1f)
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
            damageable.Damage(lightningDamage,damageInterval,lightningVFX);
        }
    }
}
