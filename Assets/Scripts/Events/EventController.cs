using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    public GameObject lightning;

    public float eventInterval;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(Lightning), eventInterval, eventInterval);
    }
    void Lightning()
    {
        Instantiate(lightning);
    }
}
