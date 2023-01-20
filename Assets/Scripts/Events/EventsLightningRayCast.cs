using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsLightningRayCast : MonoBehaviour
{
    [Header("Lightning Strike Settings")]
    public float LightningDamage;
    public float DamageInterval;
    public float TimeBetweenWarningAndLightning = 1;
    public float LightningHitboxDuration;
    [Header("Lightning Types Chance Weights")]
    [Range(0, 100)] public int WhiteLightningChanceWeight;
    [Range(0, 100)] public int BlueLightningChanceWeight;
    [Range(0, 100)] public int PurpleLightningChanceWeight;
    [Header("Prefabs")]
    public GameObject LightningVFX;
    public GameObject LightningWarningVFX;
    public GameObject LightningHitbox;
    [HideInInspector] public float TimePassed;

    TileGrid _tileMap;
    TileGeneric _tile;
    Vector3 _tileCenter;
    Vector3 _lightningofset;

    private bool _hasEnteredContact = false;
    private bool _hasTriggeredVFX = false;

    private void OnEnable()
    {
        _tileMap = TileGrid.FindTilemap();
        LightningSelector();
    }

    private void FixedUpdate()
    {      
        if (_hasEnteredContact == true)
        {
            TimePassed += Time.deltaTime;
        }

        if (TimePassed >= TimeBetweenWarningAndLightning && _hasTriggeredVFX == false)
        {
            _hasTriggeredVFX = true;
            FindTarget(transform.position, LightningVFX);
        }

        else if (TimePassed >= TimeBetweenWarningAndLightning * 2)
        {
            Destroy(gameObject);
        }
    }
    private void LightningSelector()
    {
        int random = Random.Range(0, 3);
        int procChance = Random.Range(0, 100);
        switch (random)
        {
            case 0:
                if (procChance < WhiteLightningChanceWeight)
                {
                    WhiteLightning();
                }

                else
                {
                    LightningSelector();
                }

                break;

            case 1:
                if (procChance < BlueLightningChanceWeight)
                {
                    BlueLightningSelector();
                }
                else
                {
                    LightningSelector();
                }

                break;

            case 2:
                if (procChance < PurpleLightningChanceWeight)
                {
                    PurpleLightning();
                }

                else
                {
                    LightningSelector();
                }

                break;

        }
    }

    private void BlueLightningSelector()
    {
        int random = Random.Range(0, 4);

        if (random == 0)
        {
            // Middle, Up tiles
            _lightningofset = new Vector3(0, 0, 0.5f);
            BlueLightning(Random.Range(-1, 9), Random.Range(-4, 3), _lightningofset);
        }

        else if (random == 1)
        {
            // Middle, Up-Right tiles
            _lightningofset = new Vector3(0.5f, 0, 0.5f);
            BlueLightning(Random.Range(-1, 8), Random.Range(-4, 4), _lightningofset);
        }

        else if (random == 2)
        {
            // Middle, Right tiles
            _lightningofset = new Vector3(0.5f, 0, 0);
            BlueLightning(Random.Range(-1, 8), Random.Range(-4, 4), _lightningofset);
        }

        else if (random == 3)
        {
            // Middle, Down-Right tiles
            _lightningofset = new Vector3(0.5f, 0, -0.5f);
            BlueLightning(Random.Range(-1, 8), Random.Range(-3, 3), _lightningofset);
        }
    }

    private void FindTarget(Vector3 originPosition, GameObject instantiation)
    {
        RaycastHit hit;
        if (Physics.Raycast(originPosition, Vector3.down, out hit, 10))
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if ( damageable != null)
            {
                _hasEnteredContact = true;
                Vector3 hitPosition = new Vector3(originPosition.x, originPosition.y - hit.distance + 0.1f, originPosition.z);
                GameObject instantiatedObject = Instantiate(instantiation, transform);
                instantiatedObject.transform.position = hitPosition;

            }

        }
        else
        {
            Debug.Log("no damageable was hit.");
        }
    }

    private void InstantiateHitboxes(Vector3[] hitPositions)
    {
        for (int i = 0; i < hitPositions.Length; i++)
        {
            FindTarget(hitPositions[i], LightningHitbox);
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
        FindTarget(transform.position, LightningWarningVFX);
        Vector3 strikePosition = transform.position;
        FindTarget(strikePosition, LightningHitbox);

    }

    private void BlueLightning(int tileXPosition, int tileYPosition, Vector3 lightningOfset)
    {
        transform.localScale = new Vector3(0.8f, transform.localScale.y, 0.8f);
        _tile = _tileMap.GetTile(new Vector2Int(tileXPosition, tileYPosition));
        _tileCenter = _tile.WorldCenter;
        transform.position = new Vector3(
            _tileCenter.x + lightningOfset.x, 4, _tileCenter.z + lightningOfset.z);
        FindTarget(transform.position, LightningWarningVFX);
        //Lightning hitboxes get instantiated

        Vector3[] strikePositions = new Vector3[] {
        transform.position - lightningOfset,
        transform.position + lightningOfset
        };

        InstantiateHitboxes(strikePositions);
    }

    private void PurpleLightning()
    {
        transform.localScale = new Vector3(0.8f, transform.localScale.y, 0.8f);
        int tileXPosition = Random.Range(-1, 8);
        int tileYPosition = Random.Range(-4, 3);
        _tile = _tileMap.GetTile(new Vector2Int(tileXPosition, tileYPosition));
        _tileCenter = _tile.WorldCenter;
        transform.position = new Vector3(
            _tileCenter.x + 0.5f, 4, _tileCenter.z + 0.5f);

        Vector3 ofset1 = new Vector3(0.5f, 0, 0.5f);
        Vector3 ofset2 = new Vector3(-0.5f, 0, 0.5f);
        FindTarget(transform.position, LightningWarningVFX);
        //Lightning hitboxes get instantiated

        Vector3[] strikePositions = new Vector3[] {
        transform.position + ofset1,
        transform.position - ofset1,
        transform.position + ofset2,
        transform.position - ofset2
        };

        InstantiateHitboxes(strikePositions);
    }

}
