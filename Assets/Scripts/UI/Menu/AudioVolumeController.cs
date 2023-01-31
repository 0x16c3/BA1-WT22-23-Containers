using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeController : MonoBehaviour
{
    Slider volumeSlider;
    TextMeshProUGUI volumeValue;
    public float MusicVolume = 100f;
    public float AmbianceVolume = 100f;

    void Start()
    {
        volumeSlider = GetComponent<Slider>();
        volumeValue = GetComponent<TextMeshProUGUI>();
        volumeSlider.value = MusicVolume;
        volumeValue.text = MusicVolume.ToString();
    }

    public void UpdateAmbianceVolume(float value)
    {
        AmbianceVolume = value;
        AudioController.instance.VolumeController("Ambiance", AmbianceVolume);
        volumeValue.text = AmbianceVolume.ToString();
    }

    public void UpdateMusicVolume(float value)
    {
        MusicVolume = value;
        AudioController.instance.VolumeController("Music", MusicVolume);
        volumeValue.text = MusicVolume.ToString();
    }
}
