using UnityEngine.Audio;
using UnityEngine;

public enum AudioType
{
    Music,
    Ambiance,
}
[System.Serializable]
public class Sounds
{
    public string Name;

    public AudioType AudioType;

    public AudioClip Clip;

    [Range(0f, 1f)]
    public float Volume = 1f;

    public bool Loop;

    [HideInInspector]
    public AudioSource AudioSource;


}