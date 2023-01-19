using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EventsLightningEffect : MonoBehaviour
{
    public float LightningDamage;
    public float DamageInterval;
    public float TimeBetweenWarningAndLightning = 1;
    public GameObject LightningVFX;
    public GameObject LightningWarningVFX;
    public GameObject LightningHitbox;
    [HideInInspector] public float TimePassed;

    TileGrid _tileMap;
    TileGeneric _tile;
    Collider _colliderTrigger;
    Vector3 _tileCenter;
    Vector3 _lightningofset;


    private bool _hasEnteredContact = false;
    private bool _hasTriggeredVFX = false;

    private void OnEnable()
    {
        _colliderTrigger = GetComponent<Collider>();
        int random = Random.Range(0, 20);
        _tileMap = TileGrid.FindTilemap();

        //Small Lightning
        if (random < 14)
        {
            WhiteLightning();
        }

        //Medium Lightning in all its 4 possible shapes
        else if (random == 14)
        {
            // Middle, Up tiles
            _lightningofset = new Vector3(0, 0, 0.5f);
            BlueLightning(Random.Range(-1, 9), Random.Range(-4, 9), _lightningofset);
        }

        else if (random == 15)
        {
            // Middle, Up-Right tiles
            _lightningofset = new Vector3(0.5f, 0, 0.5f);
            BlueLightning(Random.Range(-1, 8), Random.Range(-4, 10), _lightningofset);
        }

        else if (random == 16)
        {
            // Middle, Right tiles
            _lightningofset = new Vector3(0.5f, 0, 0);
            BlueLightning(Random.Range(-1, 8), Random.Range(-4, 10), _lightningofset);
        }

        else if (random == 17)
        {
            // Middle, Down-Right tiles
            _lightningofset = new Vector3(0.5f, 0, -0.5f);
            BlueLightning(Random.Range(-1, 8), Random.Range(-3, 9), _lightningofset);
        }

        //Big Lightning
        else if (random >= 18)
        {
            PurpleLightning();
        }

    }

    private void FixedUpdate()
    {
        if (_hasEnteredContact == false)
        {
            transform.position += Vector3.down;
        }

        else
        {
            TimePassed += Time.deltaTime;
            if (TimePassed >= TimeBetweenWarningAndLightning && _hasTriggeredVFX == false)
            {
                _hasTriggeredVFX = true;
                Instantiate(LightningVFX, transform);
            }

            else if (TimePassed >= TimeBetweenWarningAndLightning * 2)
            {
                Destroy(gameObject);
            }

        }

        // If it didn't hit anything, it will delete itself to avoid clutter
        if (transform.position.y <= -10)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        //Check if the object in contact is Damageable
        if (damageable != null && _hasEnteredContact == false)
        {
            _hasEnteredContact = true;
            transform.position += new Vector3(0, 0.1f, 0);
            Instantiate(LightningWarningVFX, transform);

        }

    }
        
    private void WhiteLightning()
    {
        int tileXPosition = Random.Range(-1, 9);
        int tileYPosition = Random.Range(-4, 4);
        _tile = _tileMap.GetTile(new Vector2Int(tileXPosition, tileYPosition));
        _tileCenter = _tile.WorldCenter;
        transform.position = new Vector3(
            _tileCenter.x, 4, _tileCenter.z);
        //Lightning hitboxes get instantiated
        GameObject hitbox = Instantiate(LightningHitbox);
        hitbox.transform.position = transform.position;
        hitbox.transform.SetParent(transform);
    }

    private void BlueLightning(int tileXPosition, int tileYPosition, Vector3 lightningOfset)
    {
        transform.localScale = new Vector3(0.8f, transform.localScale.y, 0.8f);
        _tile = _tileMap.GetTile(new Vector2Int(tileXPosition, tileYPosition));
        _tileCenter = _tile.WorldCenter;
        transform.position = new Vector3(
            _tileCenter.x + lightningOfset.x, 4, _tileCenter.z + lightningOfset.z);
        //Lightning hitboxes get instantiated
        GameObject hitbox = Instantiate(LightningHitbox);
        hitbox.transform.position = transform.position - lightningOfset;
        hitbox.transform.SetParent(transform);

        GameObject hitbox2 = Instantiate(LightningHitbox);
        hitbox2.transform.position = transform.position + lightningOfset;
        hitbox2.transform.SetParent(transform);
    }

    private void PurpleLightning()
    {
        transform.localScale = new Vector3(1, transform.localScale.y, 1);
        int tileXPosition = Random.Range(-1, 8);
        int tileYPosition = Random.Range(-4, 3);
        _tile = _tileMap.GetTile(new Vector2Int(tileXPosition, tileYPosition));
        _tileCenter = _tile.WorldCenter;
        transform.position = new Vector3(
            _tileCenter.x + 0.5f, 4, _tileCenter.z + 0.5f);

        Vector3 ofset1 = new Vector3(0.5f, 0, 0.5f);
        Vector3 ofset2 = new Vector3(-0.5f, 0, 0.5f);

        //Lightning hitboxes get instantiated

        GameObject hitbox1 = Instantiate(LightningHitbox);
        hitbox1.transform.position = transform.position + ofset1;
        hitbox1.transform.SetParent(transform);

        GameObject hitbox2 = Instantiate(LightningHitbox);
        hitbox2.transform.position = transform.position - ofset1;
        hitbox2.transform.SetParent(transform);

        GameObject hitbox3 = Instantiate(LightningHitbox);
        hitbox3.transform.position = transform.position + ofset2;
        hitbox3.transform.SetParent(transform);

        GameObject hitbox4 = Instantiate(LightningHitbox);
        hitbox4.transform.position = transform.position - ofset2;
        hitbox4.transform.SetParent(transform);
    }

}
