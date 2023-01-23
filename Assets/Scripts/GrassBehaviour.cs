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
    [Tooltip("Array size has to be the amount of stages in grass," +
        " leave the last element with a value of 0 as this is the impassable one." +
        " Write the percentage as a whole number.")]
    public float[] SpeedReductionsPercent;
 
    Damageable _damageable;
    List<Transform> _modelsList = new List<Transform>();
    float _timePassed;
    int _currentIteration = 0;
    bool _overgrown = false;

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
            if (_transform.name != "Fire(Clone)") // Don't count the fire effect from damageables.
            {
                _modelsList.Add(_transform);
                _transform.gameObject.SetActive(false);
            }

        }
        _modelsList[_currentIteration].gameObject.SetActive(true);
        if (_modelsList.Count > NumberOfStages)
            Debug.LogWarning("In grass object more children than declared");
    }

    void Update()
    {
        // the stage of the grass is dependent on the amount of health it has,
        // so check for health every frame.
        _timePassed += Time.deltaTime;
        if (_damageable.Health > _damageable.SelectedHealth * 4)
        {
            _damageable.Health = _damageable.SelectedHealth * 4;
        }
        else if (_damageable.Health <= _damageable.SelectedHealth && _currentIteration != 0)
        {
            ActiveStage(0);
        }
        else if (_damageable.Health <= _damageable.SelectedHealth * 2 &&
            _damageable.Health > _damageable.SelectedHealth && _currentIteration != 1)
        {
            ActiveStage(1);
        }
        else if (_damageable.Health <= _damageable.SelectedHealth * 3 &&
            _damageable.Health > _damageable.SelectedHealth * 2 && _currentIteration != 2)
        {
            ActiveStage(2);
        }
        else if (_damageable.Health <= _damageable.SelectedHealth * 4  &&
            _damageable.Health > _damageable.SelectedHealth * 3 && _currentIteration != 3)
        {
            ActiveStage(3);
        }

        // increase health to move to the next stage
        if (_timePassed >= GrowTime && _currentIteration < NumberOfStages - 1)
        {
            _damageable.Health += _damageable.SelectedHealth;
            _timePassed = 0;
        }
        else if (_timePassed >= (GrowTime * 2) + Time.fixedDeltaTime)
        {
            Destroy(gameObject); // Overgrown
        }
        else if (_timePassed >= GrowTime * 2)
        {
            _overgrown = true; // Overgrow destroys floor tile
        }
        
    }
    void CheckForOtherColliders() // Check if there is Floor or Older Grass where it was instantiated and act accordingly
    {
        bool willBeDestroyed = true;
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.25f);
        foreach (Collider hit in hits)
        {
            Damageable previousGrass = hit.GetComponent<Damageable>();
            if (hit.gameObject.tag == "Grass" && hit.gameObject != gameObject && previousGrass != null)
            {
              //  Debug.Log("found older grass, combining");
                previousGrass.Health += 3;
                willBeDestroyed = true;
                break;
            }
            else if (hit.gameObject.tag == "FloorTile")
            {
              //  Debug.Log("found ground tile");
                willBeDestroyed = false;
            }
        }

        if (willBeDestroyed)
        {
          //  Debug.Log("destroying");
            Destroy(gameObject);
        }

    }
    private void ActiveStage(int currentIteration) // turns off the model of the previous stage and turns on the model for the new stage
    {
        _modelsList[_currentIteration].gameObject.SetActive(false);
        _currentIteration = currentIteration;
        _modelsList[_currentIteration].gameObject.SetActive(true);
    }

    public void OnCut(int damage) // Gets called by machete
    {
        _damageable.Health -= damage;
        _timePassed = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        IDamageable damage = other.gameObject.GetComponent<IDamageable>();
        // Destroy the floor when it has overgrown
        if (_overgrown == true && damage != null)
        {
            damage.Damage(50, 0);
        }

        // If Time between damage is 0 it means the grass is burning, so burn the player or crate on top
        if (_damageable.TimeBetweenDamage > 0 && other.gameObject.tag != "FloorTile" && damage != null)
        {
            
            damage.Damage(6, 3);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerLocomotion player = other.GetComponent<PlayerLocomotion>();
        if (player != null && !player.IsDebuffed) // Slow down the player depending on the stage of the grass
        {
            player.IsDebuffed = true;
            player.MovementSpeed *= 1 - (SpeedReductionsPercent[_currentIteration] / 100);
          //  Debug.Log("reducing health by " + SpeedReductionsPercent[_currentIteration] + "% to" + player.MovementSpeed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerLocomotion player = other.GetComponent<PlayerLocomotion>();
        if (player != null)  // Reset the speed of the player
        {
            player.IsDebuffed = false;
            player.MovementSpeed = player.InitialSpeed;
           // Debug.Log("reverted speed to " + player.MovementSpeed);
        }
    }
}
