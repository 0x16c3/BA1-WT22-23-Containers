using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardeningGoatMovementTest : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float turnInterval = 5f;
    private float nextTurnTime;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nextTurnTime = Time.time + turnInterval;
    }

    void Update()
    {
        transform.position += transform.forward * walkSpeed * Time.deltaTime;
        if (Time.time >= nextTurnTime)
        {
            float turnAngle = Random.Range(0, 2) == 0 ? 90 : -90;
            transform.Rotate(new Vector3(0, turnAngle, 0));
            nextTurnTime = Time.time + turnInterval;
        }
    }
}