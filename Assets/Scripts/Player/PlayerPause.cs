using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPause : MonoBehaviour
{

    public GameObject Pause;
    UIPause Ui;
    private void Start()
    {
        Ui= Pause.GetComponent<UIPause>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Ui.Active = true;
        }
    }
}
