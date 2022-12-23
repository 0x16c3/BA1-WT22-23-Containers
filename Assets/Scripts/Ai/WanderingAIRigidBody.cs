using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WanderingAIRigidBody : MonoBehaviour
{
    public float moveInterval = 1.0f; // interval between moves

    public float moveForce = 10.0f; // force applied to the rigid body

    public LayerMask layerMask;

    public float detectionRadius;

    private bool _isPathClear;

    private int _randomDirection;

    private Vector3 _previousDirection;

    private Rigidbody _rb; // the rigid body to move

    private List<Vector3> _freePath = new List<Vector3>();
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        // Start moving the rigid body in a random direction every moveInterval seconds
        InvokeRepeating("Move", 0.0f, moveInterval);
    }

    void Move()
    {
        if (transform.parent == null)
        {
            // Detection Phase

            for (int i = 0; i < 360; i += 90)
            {
                // Convert the angle to a direction vector
                Vector3 _detection = Quaternion.Euler(0, i, 0) * Vector3.forward;

                // Check if the direction is clear by casting rays in a cone of 45 degrees
                _isPathClear = true;
                for (int j = -10; j <= 10; j += 2)

                {
                    // Raycast in the direction
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, Quaternion.Euler(0, j, 0) * _detection, out hit, detectionRadius, layerMask))
                    {
                        // An obstacle was hit, so this direction is not clear
                        _isPathClear = false;
                        break;
                    }
                }

                // If the direction is clear, add it to the list of possible directions
                if (_isPathClear)
                {
                    _freePath.Add(_detection);

                }
                else
                {
                    Debug.Log("path not found " + _detection);
                }
            }

            //Movement Phase

            if (_freePath.Count == 0)
            {
                Debug.Log("no path found");
                // Generate a random direction
                _randomDirection = Random.Range(1, 4);
                transform.eulerAngles = new Vector3(0.0f, _randomDirection * 90, 0.0f);
                Vector3 _moveDirection = transform.forward;

                // Apply force to the rigid body in the generated direction
                _rb.AddForce(_moveDirection * moveForce * 2, ForceMode.Impulse);
                _freePath = new List<Vector3>();

            }
            else
            {
                // Pick a Direction from the collected available ones in the detection phase
                _randomDirection = Random.Range(0, _freePath.Count);
                Vector3 _direction = _freePath[_randomDirection];

                // If it is chosen to move backwards, try to find an alternative path and use it instead. If there is none, move backwards.
                while (_direction == _previousDirection && _freePath.Count > 1)
                {
                    _randomDirection = Random.Range(0, _freePath.Count);
                    _direction = _freePath[_randomDirection];
                }

                //Move
                _rb.AddForce(_direction * moveForce, ForceMode.Impulse);

                //Log direction and reset list
                _previousDirection = -_direction;
                _freePath = new List<Vector3>();
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}