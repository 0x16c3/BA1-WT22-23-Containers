using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSound : MonoBehaviour
{
    void Start()
    {
        if (AudioController.instance != null ) AudioController.instance.VolumeController("Ambiance", 0);   
    }
}
