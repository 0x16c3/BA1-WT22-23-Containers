using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingAIRigidBody : MonoBehaviour
{
    public float moveInterval = 1.0f; // interval between moves

    public float moveForce = 10.0f; // force applied to the rigid body

    private Rigidbody _rb; // the rigid body to move

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
            // Generate a random direction
            float randomDirection = Random.Range(0.0f, 360.0f);
            transform.eulerAngles = new Vector3(0.0f, randomDirection, 0.0f);
            Vector3 moveDirection = transform.forward;

            // Apply force to the rigid body in the generated direction
            _rb.AddForce(moveDirection * moveForce, ForceMode.Impulse);
        }
    }
}