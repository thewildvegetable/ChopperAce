using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource chopSource;
    public AudioSource beeSource;
    public AudioSource ouchSource;
    public AudioSource musicSource;

    public AudioClip chopClip;
    public AudioClip beeClip;
    public AudioClip ouchClip;
    public AudioClip musicClip;

    public static SoundManager instance = null;

    public float volume = 0.5f;

    public bool muted = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayOuch()
    {
        if(!muted)
            ouchSource.PlayOneShot(ouchClip, volume);
    }

    public void PlayChop()
    {
        if (!muted)
            chopSource.PlayOneShot(chopClip, volume);
    }

    public void PlayBees()
    {
        if (!muted)
            beeSource.PlayOneShot(beeClip, volume);
    }

    public void MuteAudio()
    {
        if (muted)
            muted = false;
        else
            muted = true;
    }


}
