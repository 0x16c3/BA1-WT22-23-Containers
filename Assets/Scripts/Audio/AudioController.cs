using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public Sounds[] Sounds;
    public static AudioController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sounds audio in Sounds)
        {
            audio.AudioSource = gameObject.AddComponent<AudioSource>();
            audio.AudioSource.clip = audio.Clip;

            audio.AudioSource.volume = audio.Volume;
            audio.AudioSource.loop = audio.Loop;
        }
    }

    public void PlayAudio(string name)
    {
        Sounds audio = Array.Find(Sounds, Sounds => Sounds.Name == name);
        if (audio == null) 
        {
            Debug.LogWarning("Sound of name " + name + " not found");
            return;
        }
        // set the volume to the one set in the menu by the player depending on the AudioType variable
        audio.AudioSource.Play();
    }

    /*AudioSource audioSource;

private void Start()
{ 
    gameObject.SetActive(false);
}

private void OnEnable()
{
    audioSource = GetComponent<AudioSource>();
    // Wait for the audio to finish playing before deactivating the game object
    StartCoroutine(DeactivateAfterAudioFinish(audioSource.clip.length));
}

 private void Update()
{
    if (gameObject.tag == "AudioMusic"){
        audioSource.volume = AudioVolumeController.MusicVolume / 100;
    }
    else{
        audioSource.volume = AudioVolumeController.AmbianceVolume / 100;
    }  
}

IEnumerator DeactivateAfterAudioFinish(float delay)
{
    yield return new WaitForSeconds(delay);
    gameObject.SetActive(false);
}*/
}
