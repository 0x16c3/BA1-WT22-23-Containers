using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMachete : MonoBehaviour
{
    public GameObject Machete;

    // Start is called before the first frame update
    void Start()
    {
        if (Machete == null)
            Machete = GameObject.Find("Machete");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) )
        {
        }
    }
}
