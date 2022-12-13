using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeController : MonoBehaviour
{
    Slider VolumeSlider;
    TextMeshProUGUI VolumeValue;
    public static float MusicVolume = 100f;
    public static float AmbianceVolume = 100f;

    void Start()
    {
        VolumeSlider = GetComponent<Slider>();
        VolumeValue = GetComponent<TextMeshProUGUI>();
        VolumeSlider.value = MusicVolume;
        VolumeValue.text = MusicVolume.ToString();
    }

    public void UpdateMusicVolume(float value)
    {
        MusicVolume = value;
        VolumeValue.text = MusicVolume.ToString();
    }
    public void UpdateAmbianceVolume(float value)
    {
        AmbianceVolume = value;
        VolumeValue.text = AmbianceVolume.ToString();
    }
}
