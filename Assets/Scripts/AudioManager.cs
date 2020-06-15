using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour{

    public Sound[] sounds;

    public static AudioManager instance;

    void Awake(){

        //Used to handle Audio Manager carry over between scenes
        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);

        //Audio Manager setup
        foreach (Sound audio in sounds){
           audio.source = gameObject.AddComponent<AudioSource>();
           audio.source.clip = audio.audio;
           audio.source.volume = audio.volume;
           audio.source.loop = audio.loop;
        }
    }

    void Start(){
        Play("Background Music");
    }

    /* Find audio and play it from sounds array
                    */
    public void Play (string name){
        Sound audio = Array.Find(sounds, Sound => Sound.name == name);
        audio.source.Play();
    }
}



/* Sound variable type class
                * Used to setup AudioManager
                */
[System.Serializable]
public class Sound{

    public string name;

    [Range(0f, 1f)]
    public float volume;
    public bool loop;
    public AudioClip audio;

    [HideInInspector]
    public AudioSource source;
}
