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
            Destroy(gameObject);
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
    public void StopAudio(string name)
    {
        Sounds audio = Array.Find(Sounds, Sounds => Sounds.Name == name);
        if (audio == null)
        {
            Debug.LogWarning("Sound of name " + name + " not found");
            return;
        }
        // set the volume to the one set in the menu by the player depending on the AudioType variable
        StartCoroutine(FadeOut(audio.AudioSource));
    }
    public void StopLoop(string name)
    {
        Sounds audio = Array.Find(Sounds, Sounds => Sounds.Name == name);
        if (audio == null)
        {
            Debug.LogWarning("Sound of name " + name + " not found");
            return;
        }
        // set the volume to the one set in the menu by the player depending on the AudioType variable
        StartCoroutine(PlayAudioToEnd(audio.AudioSource));
    }

    public bool IsAudioPlaying(string name)
    {
        Sounds audio = Array.Find(Sounds, Sounds => Sounds.Name == name);
        if (audio == null)
        {
            Debug.LogWarning("Sound of name " + name + " not found");
            return false;
        }
        return audio.AudioSource.isPlaying;
    }
    private IEnumerator PlayAudioToEnd(AudioSource audio)
    {
        audio.loop = false;
        while (audio.isPlaying)
        {
            yield return null;
        }
        audio.loop = true;
    }

    private IEnumerator FadeOut (AudioSource audio)
    {
        float startVolume = audio.volume;
        float time = 0f;

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, 0f, time);
            yield return null;
        }

        audio.Stop();
        audio.volume = startVolume;
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
