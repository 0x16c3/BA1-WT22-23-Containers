using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GrassBehaviour : MonoBehaviour
{
    public int NumberOfStages = 4;
    public float GrowTime = 5f;
 
    Damageable _damageable;
    List<Transform> _modelsList = new List<Transform>();
    float _timePassed;
    int _currentIteration = 0;
    bool _isFinished = false;

    private float _originalPlayerMovementSpeed;
    // Start is called before the first frame update
    void Start()
    {
        CheckForOtherColliders();
        _damageable = gameObject.GetComponent<Damageable>();
        foreach (Transform _transform in transform)
        {
            //To use instead of assebling prefab children manually
            /*float _spawnPositionY = -0.01f;   
            _transform.position = new Vector3(transform.parent.position.x, _spawnPositionY, transform.parent.position.z);
            */
            _modelsList.Add(_transform);
            _transform.gameObject.SetActive(false);
        }
        _modelsList[_currentIteration].gameObject.SetActive(true);
        if (_modelsList.Count > NumberOfStages)
            Debug.LogWarning("In grass object more children than declared");
    }

    // Update is called once per frame
    void Update()
    {
        
        _timePassed += Time.deltaTime;
        if (_damageable.Health > 12)
        {
            _damageable.Health = 12;
        }
        else if (_damageable.Health <= 3 && _currentIteration != 0)
        {
            ActiveStage(0);
        }
        else if (_damageable.Health <= 6 && _damageable.Health > 3 && _currentIteration != 1)
        {
            ActiveStage(1);
        }
        else if (_damageable.Health <= 9 && _damageable.Health > 6 && _currentIteration != 2)
        {
            ActiveStage(2);
        }
        else if (_damageable.Health <= 12 && _damageable.Health > 9 && _currentIteration != 3)
        {
            ActiveStage(3);
        }

        if (_timePassed >= GrowTime && _currentIteration < NumberOfStages - 1)
        {
            _damageable.Health += 3;
            _timePassed = 0;
        }
        else if (_timePassed >= (GrowTime * 2) + 0.1f)
        {
            Destroy(gameObject);
        }
        else if (_timePassed >= GrowTime * 2)
        {
            _isFinished = true;
        }
        
    }
    void CheckForOtherColliders()
    {
        bool willBeDestroyed = true;
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.25f);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.tag == "Grass" && hit.gameObject != gameObject)
            {
                Damageable previousGrass = hit.GetComponent<Damageable>();
                if (previousGrass != null)
                {
                    Debug.Log("found older grass, combining");
                    previousGrass.Health += 3;
                    willBeDestroyed = true;
                    break;
                }

            }
            else if (hit.gameObject.tag == "FloorTile")
            {
                Debug.Log("found ground tile");
                willBeDestroyed = false;
            }
        }

        if (willBeDestroyed)
        {
            /*for (int i = 0, imax = hits.Length; i < imax; i++)
            {
                Debug.Log(Convert.ToString(hits[i].gameObject.name));
                Debug.Log(Convert.ToString(hits[i].gameObject.tag));
            }*/
            Debug.Log("destroying");
            Destroy(gameObject);
        }

    }
    private void ActiveStage(int currentIteration)
    {
        _modelsList[_currentIteration].gameObject.SetActive(false);
        _currentIteration = currentIteration;
        _modelsList[_currentIteration].gameObject.SetActive(true);
    }

    public void OnCut(int damage)
    {
        _damageable.Health -= damage;
        _timePassed = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        IDamageable damage = other.gameObject.GetComponent<IDamageable>();
        // Destroy the floor
        if (_isFinished == true && damage != null)
        {
            damage.Damage(50, 0);
        }

        // If Time between damage is 0 it means the grass is burning
        if (_damageable.TimeBetweenDamage > 0 && other.gameObject.tag != "FloorTile" && damage != null)
        {
            
            damage.Damage(6, 3);
        }

    }
}
