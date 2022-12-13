using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioTest : MonoBehaviour
{
    public GameObject MusicTestAudio;
    public GameObject AmbianceTestAudio;

    // This function will be called when the user presses the T button
    public void MusicTestOnClick()
    {
        // Turns on the test audio selected
        MusicTestAudio.SetActive(true);
    }
    public void AmbianceTestOnClick()
    {
        // Turns on the test audio selected


        AmbianceTestAudio.SetActive(true);
    }
}
