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

    private float _posx;
    
    private float _posz;


    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        InvokeRepeating(nameof(RNG), 0, maxWaitTime);
    }

    void Update()
    {
        switch (_idle)
        {
            case true:
                StartCoroutine(nameof(Idling));
                break;

            case false:
                Wandering();
                break;
        }
    }

    // RNG method changes the values of the variables everytime it is invoked
    void RNG()
    {
        _posx = transform.position.x + Random.Range(-radius, radius);
        _posz = transform.position.z + Random.Range(-radius, radius);
    }

    // Wandering method points to the target and sets the agent towards it
    private void Wandering()
    {
        // point
        _target = new Vector3(_posx, transform.position.y, _posz);

        // set agent
        _agent.SetDestination(_target);

        // checks if agent has made it to its destination
        if (_agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            _idle = true;
        }
    }

    // Idling method waits an amount based on the waiting times
    IEnumerator Idling()
    {
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        _idle = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_target, transform.position);
    }
}