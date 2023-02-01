using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPause : MonoBehaviour
{
    float _time;
    [HideInInspector] public bool Active = false;
    public GameObject UI;
    private void Update()
    {
        if (Active)
        {
            UI.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            UI.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    public void ContinueOnClick()
    {
        AudioController.instance.PlayAudio("Button Click");
        Active = false;
        gameObject.SetActive(false);
    }

    public void ExitOnClick()
    {
        Active = false;
        SceneManager.LoadScene("MainMenu");
    }
}
