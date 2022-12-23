using System.Collections.Generic;
using UnityEngine;

public class AIWander : MonoBehaviour
{
    public float IntervalMin = 1.0f;
    public float IntervalMax = 3.0f;
    public float MovementSpeed = 10.0f;
    public float DetectionRadius;

    Vector3 _previousDirection;
    Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        InvokeRepeating("Move", 0.0f, IntervalMin);
    }

    void Move()
    {
        if (transform.parent != null)
            return;

        List<Vector3> directions = GetAvaliableDirections();

        if (directions.Count == 0)
        {
            // Generate a random direction
            int randDirection = Random.Range(1, 4);
            transform.eulerAngles = new Vector3(0.0f, randDirection * 90, 0.0f);
            Vector3 moveDirection = transform.forward;

            // Apply force to the rigid body in the generated direction
            _rb.AddForce(moveDirection * MovementSpeed * 2, ForceMode.Impulse);
        }
        else
        {
            // Pick a Direction from the collected available ones in the detection phase
            int randDirection = Random.Range(0, directions.Count);
            Vector3 direction = directions[randDirection];

            // If it is chosen to move backwards, try to find an alternative path and use it instead. If there is none, move backwards.
            while (direction == -_previousDirection && directions.Count > 1)
            {
                randDirection = Random.Range(0, directions.Count);
                direction = directions[randDirection];
            }

            _rb.AddForce(direction * MovementSpeed, ForceMode.Impulse);
            _previousDirection = direction;
        }
    }

    List<Vector3> GetAvaliableDirections()
    {
        List<Vector3> directions = new List<Vector3>();
        for (int i = 0; i < 360; i += 90)
        {
            // Convert the angle to a direction vector
            Vector3 angle = Quaternion.Euler(0, i, 0) * Vector3.forward;

            // Check if the direction is clear by casting rays in a cone of 45 degrees
            bool isClear = true;
            for (int j = -10; j <= 10; j += 2)
            {
                // Raycast in the direction
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Quaternion.Euler(0, j, 0) * angle, out hit, DetectionRadius))
                {
                    // An obstacle was hit, so this direction is not clear
                    isClear = false;
                    break;
                }
            }

            if (!isClear)
                continue;

            directions.Add(angle);
        }

        return directions;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
    }
}