using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsLightning : MonoBehaviour
{
    public float lightningDamage;

    public float damageInterval;

    public GameObject lightning;

    TileGrid _tileMap;

    TileGeneric _tile;

    Collider _trigger;

    Vector3 _tileCenter;

    private int _xpos;

    private int _ypos;
    private void OnEnable()
    {
        _trigger = GetComponent<Collider>();
        _trigger.enabled = false;
        _tileMap = TileGrid.FindTilemap();
        _xpos = Random.Range(-3, 10);
        _ypos = Random.Range(-4, 4);
        _tile = _tileMap.GetTile(new Vector2Int(_xpos, _ypos));
        _tileCenter = _tile.WorldCenter;
        transform.position = _tileCenter;
        StartCoroutine(nameof(Lightning));
    }
    IEnumerator Lightning()
    {
        // play lightning vfx
        float lightningEffectTime = 1f;
        yield return new WaitForSeconds(lightningEffectTime);
        Debug.Log("time passed");
        _trigger.enabled = true;
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("trigger");
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.Damage(lightningDamage,damageInterval,lightning);
        }
    }
}
