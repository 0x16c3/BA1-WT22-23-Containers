using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource audioSource;
    private void Start(){
        
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        // Wait for the audio to finish playing before deactivating the game object
        StartCoroutine(DeactivateAfterAudioFinish(audioSource.clip.length));
    }
    private void Update(){
        if (gameObject.tag == "AudioMusic"){
            audioSource.volume = AudioVolumeController.MusicVolume / 100;
        }
        else{
            audioSource.volume = AudioVolumeController.AmbianceVolume / 100;
        }  
    }


    IEnumerator DeactivateAfterAudioFinish(float delay){
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
