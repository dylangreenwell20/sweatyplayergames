using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioClip clip; //audio clip reference

    public string name; //name of audio

    [Range(0f, 1f)]
    public float volume; //volume with a slider to adjust

    [Range(.1f, 3f)]
    public float pitch; //pitch with a slider to adjust

    public bool loop; //if audio is made to be looped

    [HideInInspector]
    public AudioSource source; //source of audio
}
