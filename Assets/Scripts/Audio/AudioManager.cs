using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds; //array of all sounds

    public static AudioManager instance; //instance of audio manager to check if one has been made already or not

    private void Awake()
    {
        if(instance == null) //if instance has not been made
        {
            instance = this; //create instance
        }
        else //else if instance has been made
        {
            Destroy(gameObject); //destroy this instance
            return; //return function
        }

        DontDestroyOnLoad(gameObject); //dont destroy audio manager on new scene load

        foreach(Sound s in sounds) //for each sound in sounds array
        {
            s.source = gameObject.AddComponent<AudioSource>(); //add audio source component
            s.source.clip = s.clip; //set clip to sound clip

            s.source.volume = s.volume; //set volume to sound volume
            s.source.pitch = s.pitch; //set pitch to sound pitch
            s.source.loop = s.loop; //set loop bool to sound loop bool
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); //find the specific sound

        if(s == null) //if audio was not found
        {
            Debug.LogWarning("Error: Sound '" +  name + "' was not found..."); //error message
            return; //return function
        }

        if(s.source.isPlaying) //if audio is already playing
        {
            return; //return function
        }
        s.source.Play(); //play the audio
    }

    public void PlayOverlap(string name) //for playing audio that does not matter if it overlaps itself (e.g gunshots)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); //find the specific sound

        if (s == null) //if audio was not found
        {
            Debug.LogWarning("Error: Sound '" + name + "' was not found..."); //error message
            return; //return function
        }

        s.source.Play(); //play the audio
    }
}
