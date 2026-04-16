using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;

    public static AudioManager Instance { get { return instance; } }

    [SerializeField] AudioStore audioStore;

    AudioSource audioSource;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(instance.gameObject);
        }

        audioSource = this.GetComponent<AudioSource>();

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioState state)
    {
        AudioClip clip = audioStore.GetAudioClip(state,AudioType.sfx);

        audioSource.PlayOneShot(clip,1.0f);
    }

    public void PlayBg(AudioState state)
    {
        AudioClip clip = audioStore.GetAudioClip(state, AudioType.bg);

        audioSource.clip = clip;
        audioSource.Play();
        audioSource.loop = true;
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}
