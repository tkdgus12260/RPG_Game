using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundName;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;

    public static SoundManager Inst
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    public Sound[] bgmSounds;

    public AudioSource bgmPlayer;

    private void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        bgmPlayer.clip = bgmSounds[1].clip;
        bgmPlayer.Play();
    }
}
