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
        OptionsMenu.SetActive(false);
        MainMenu.SetActive(true);

    }

    public void ExitOnClick()
    {
        // Exits Application    
        Application.Quit();
    }

    public void OptionsOnClick()
    {
        // Hides the main menu screen and Unhides the options screen

        OptionsMenu.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void PlayOnClick(){
        // Load the scene with the given name
        SceneManager.LoadScene(GameScene);
    }
}

