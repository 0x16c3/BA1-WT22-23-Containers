using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // The name of the scene that we want to switch to
    public string gameScene;
    public GameObject optionsMenu;
    public GameObject mainMenu;

    // This function will be called when the user presses the PLAY button
    public void PlayOnClick(){
        // Load the scene with the given name
        SceneManager.LoadScene(gameScene);
    }
    public void OptionsOnClick(){
        // Hides the main menu screen and Unhides the options screen
        
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void ExitOnClick(){
        // Exits Application    
        Application.Quit();
    }
    public void BackOnClick()
    {
        // Red Button that pushes the tab back to the previous one (from options to Main as its the only extra tab currently)
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void Start()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}

