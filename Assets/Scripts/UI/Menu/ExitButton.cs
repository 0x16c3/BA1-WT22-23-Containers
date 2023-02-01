using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void ExitGame()
    {
        if (AudioController.instance != null) AudioController.instance.PlayAudio("Close Game");
        Application.Quit();
    }
}
