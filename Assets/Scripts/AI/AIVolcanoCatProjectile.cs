using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AIVolcanoCatProjectile : MonoBehaviour
{
    public float horizontalSpeed;

    public float gravity;

    TileGrid _tileMap;

    TileGeneric _tile;

    Rigidbody _rigidbody;

    Vector3 _tileCenter;

    private float _verticalSpeed;

    private int _xpos;

    private int _ypos;

    void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _tileMap = TileGrid.FindTilemap();
        // Selects a random tile (THE RANDOM SELECTION SHOULD BE BETWEEN 0 AND THE MAX LENGTH OF THE SIDE. NOT YET IMPLEMENTED)
        _xpos = Random.Range(0, 5);
        _ypos = Random.Range(0, 5);
        _tile = _tileMap.GetTile(new Vector2Int(_xpos, _ypos));
        _tileCenter = _tile.WorldCenter;

        ParabolaCalc();
        _rigidbody.AddForce(new Vector3(0, _verticalSpeed), ForceMode.VelocityChange);
        
    }

    void Update()
    {
        _tileCenter.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, _tileCenter, horizontalSpeed * Time.deltaTime);

        // Pulls the object down
        _rigidbody.AddForce(new Vector3(0, gravity * Time.deltaTime), ForceMode.VelocityChange);
    }

    private void ParabolaCalc()
    {
        // Calculates the force required to create a parabola between the object and the target
        float _z = _tileCenter.z - transform.position.z;
        float _x = _tileCenter.x - transform.position.x;
        float _distance = Mathf.Sqrt(_x * _x + _z * _z);
        float _time = _distance / horizontalSpeed;
        _verticalSpeed = -(gravity * _time/2);
    }
}
