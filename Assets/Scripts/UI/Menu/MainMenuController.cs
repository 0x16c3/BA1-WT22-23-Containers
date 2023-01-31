using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // The name of the scene that we want to switch to
    public string GameScene;
    public GameObject OptionsMenu;
    public GameObject MainMenu;
    public GameObject Opener;
    public GameObject Credits;

    private void Start()
    {
        OptionsMenu.SetActive(false);
        MainMenu.SetActive(false);
        Opener.SetActive(true);
    }
    
    public void StartOnClick()
    {
        AudioController.instance.PlayAudio("Button Click");
        Opener.SetActive(false);
        MainMenu.SetActive(true);
    }
    public void BackOnClick()
    {
        // Red Button that pushes the tab back to the previous one (from options to Main as its the only extra tab currently)
        AudioController.instance.PlayAudio("Close Game");
        OptionsMenu.SetActive(false);
        MainMenu.SetActive(true);

    }

    public void CreditsBackOnClick() 
    {
        AudioController.instance.PlayAudio("Close Game");
        Credits.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void CreditsOnClick()
    {
        AudioController.instance.PlayAudio("Button Click");
        Credits.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void ExitOnClick()
    {
        // Exits Application    
        AudioController.instance.PlayAudio("Button Click");
        Application.Quit();
    }

    public void OptionsOnClick()
    {
        // Hides the main menu screen and Unhides the options screen
        AudioController.instance.PlayAudio("Button Click");
        OptionsMenu.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void PlayOnClick(){
        // Load the scene with the given name
        AudioController.instance.PlayAudio("Button Click");
        SceneManager.LoadScene(GameScene);
    }

    public void AmbianceTestOnClick()
    {
        AudioController.instance.PlayAudio("Cat Roar");
    }

    public void MusicTestOnClick()
    {
        AudioController.instance.PlayAudio("Music Test");
    }
}

