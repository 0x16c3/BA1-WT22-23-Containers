using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WanderingContainer : MonoBehaviour
{
    public float minWaitTime;

    public float maxWaitTime;

    public float radius = 10f;

    private Vector3 _target;
    
    private NavMeshAgent _agent = null;
    
    private bool _idle = true;

    private bool _idlingIsHappening = false;

    private float _posx;
    
    private float _posz;


    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (transform.parent != null)
        {
            _agent.enabled = false;
            _idle = true;
        }
        else
        {
            if (_agent.enabled == false)
            {
                Invoke(nameof(PathfindReactivation), 0);
            }
            
            if (_idle == true)
            {
                StartCoroutine(nameof(Idling));
            }
            else
            {
                Wandering();
            }
        }

    }

    // Wandering method points to the target and sets the agent towards it
    private void Wandering()
    {
        // point
        _target = new Vector3(_posx, transform.position.y, _posz);

        // set agent
        _agent.SetDestination(_target);

        // checks if agent has made it to its destination
        if (_agent.remainingDistance <= 0.5f)
        {
            _agent.ResetPath();
            Debug.Log("path complete");
            _idle = true;
        }
    }

    // Idling method waits an amount based on the waiting times, it also changes the position it will target next to a random one
    IEnumerator Idling()
    {

        if (_idlingIsHappening == false)
        {
            _idlingIsHappening = true;
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            _posx = transform.position.x + Random.Range(-radius, radius);
            _posz = transform.position.z + Random.Range(-radius, radius);
            _idle = false;
            _idlingIsHappening = false;
        }
    }

    private void PathfindReactivation()
    {
        _agent.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_target, transform.position);
    }
}