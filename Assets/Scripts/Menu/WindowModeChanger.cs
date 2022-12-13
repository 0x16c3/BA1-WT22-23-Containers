using UnityEngine;
using TMPro;

public class WindowModeChanger : MonoBehaviour
{
    // The DropDown element that we want to track the selected value of
    public TMP_Dropdown dropdown;

    // This function will be called when the value of the DropDown element changes
    public void OnValueChanged(int newValue)
    {
        // Get the selected window mode from the DropDown element
        var selectedWindowMode = dropdown.options[newValue].text;

        // Set the window mode to the selected window mode
        if (selectedWindowMode == "Windowed")
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else if (selectedWindowMode == "Borderless")
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (selectedWindowMode == "Fullscreen")
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
    }
}