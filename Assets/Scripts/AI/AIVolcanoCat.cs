using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class AIVolcanoCat : MonoBehaviour
{
    public float fireRate;

    public GameObject lavaClumpPrefab;

    private Vector3 _projectilePosition;

    private float _projectileHeight = 0f;

    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(VolcanoMortar), fireRate, fireRate);
        
    }
    void VolcanoMortar()
    {
        _projectilePosition = new Vector3(transform.position.x, transform.position.y + _projectileHeight, transform.position.z);
        Instantiate(lavaClumpPrefab, _projectilePosition, Quaternion.identity);
    }
}
