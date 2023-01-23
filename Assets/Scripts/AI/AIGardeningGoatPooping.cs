using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AIGardeningGoatPooping : MonoBehaviour
{
    public float Interval = 2f;
    public int AmountPerTimeShitting = 4;
    public float TimeElapsedTillDiahrrea;
    public float PoopingSessionsCooldown;
    public GameObject GrassPrefab;
    
    private List<GameObject> _amountOfTiles;
    private float _elapsedTimeforCalm;
    private float _elapsedTimeforDiahrrea;
    private float _elapsedTimeforInterval;
    private int _amountSharted;
    private bool _isTheShipMissingTiles;
    private bool _isItPooping = false;

    void Start()
    {
        _amountOfTiles = new List<GameObject>();
        Collider[] tiles = Physics.OverlapSphere(Vector3.zero, 15);
        foreach (Collider tile in tiles)
        {
            Damageable damageable = tile.GetComponent<Damageable>();
            if (tile.gameObject.tag == "FloorTile" && damageable != null)
            {
                _amountOfTiles.Add(tile.gameObject);
            }
        }
       // Debug.Log("Amount of tiles is currently " + _amountOfTiles.Count);
        _elapsedTimeforCalm = 0f;
    }

    void Update()
    {
        _elapsedTimeforCalm += Time.deltaTime;
        _elapsedTimeforDiahrrea += Time.deltaTime;
        _elapsedTimeforInterval += Time.deltaTime;

        if (_elapsedTimeforCalm > 2 && !_isItPooping)
        { // check if all the tiles are free
            IsTheShipMissingTiles();
            _elapsedTimeforCalm = 0;
        }

        if (_elapsedTimeforDiahrrea >= TimeElapsedTillDiahrrea && _elapsedTimeforInterval >= Interval &&
            _amountSharted < AmountPerTimeShitting * 2)
        { // if goat hasn't sharted in a minute, shit really hard
            Poop();
        }
        else if (!_isTheShipMissingTiles && _elapsedTimeforInterval >= Interval &&
            _amountSharted < AmountPerTimeShitting)
        { // if no tile is destroyed, shit a little
            _elapsedTimeforDiahrrea = 0;
            Poop();
        }
        else if (_elapsedTimeforInterval >= PoopingSessionsCooldown)
        { // if no shitting has happened start scanning and reset the amount sharted
            _isItPooping = false;
            if (_amountSharted >= AmountPerTimeShitting * 2)
            {
                _elapsedTimeforDiahrrea = 0;
            }
            _amountSharted = 0;
        }
    }
    void IsTheShipMissingTiles()
    { // Scan all the current tiles and compare them to the amount of tiles scanned at the start
        List<GameObject> currentTiles = new List<GameObject> ();
        Collider[] tiles = Physics.OverlapSphere(Vector3.zero, 15);
        foreach (Collider tile in tiles)
        {
            Damageable damageable = tile.GetComponent<Damageable>();
            if (tile.gameObject.tag == "FloorTile" && damageable != null)
            {
                currentTiles.Add(tile.gameObject);
            }

        }
        
        if (currentTiles.Count >= _amountOfTiles.Count)
        {
           // Debug.Log("Contains all tiles. Amount of tiles is currently " + currentTiles.Count);
            _isTheShipMissingTiles = false;
        }
        else
        {
           // Debug.Log("tiles missing. Amount of tiles is currently " + currentTiles.Count);
            _isTheShipMissingTiles = true;
        }
    }
    void Poop()
    { //poop grass
        _amountSharted++;
        _elapsedTimeforInterval = 0;
        _isItPooping = true;
        GameObject poop = Instantiate(GrassPrefab,transform.position,Quaternion.identity);
        Debug.Log("Amount Sharted is " + _amountSharted);
        if (poop != null)
        {
            Vector3 poopPos = poop.transform.position;

            poopPos.x = Rounder(poopPos.x);
            poopPos.z = Rounder(poopPos.z);
            poopPos.y = 0;
            poop.transform.position = poopPos;
        }

    }
    float Rounder(float pos)
    {
        if (pos <= Mathf.Round(pos))
        {
            pos = Mathf.Round(pos) - 0.5f;
        }
        else if (pos > Mathf.Round(pos))
        {
            pos = Mathf.Round(pos) + 0.5f;
        }

        return pos;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Vector3.zero, 15);
    }
}


