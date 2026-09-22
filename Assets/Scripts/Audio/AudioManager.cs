using System;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;

    public static AudioManager Instance { get { return instance; } }

    [SerializeField] AudioStore audioStore;
    [SerializeField] AudioSource vehicleEngine;
    [SerializeField] public AudioSource boatEngine;
    [SerializeField] public AudioSource BoatStart;

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
        // DontDestroyOnLoad(vehicleEngine.gameObject);
        // DontDestroyOnLoad(boatEngine.gameObject);
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
    public void PlayBoatStartBg(AudioState state)
    {
        AudioClip clip = audioStore.GetAudioClip(state, AudioType.bg);
        BoatStart.clip = clip;
        BoatStart.Play();
        BoatStart.loop = true;
    }


    public void PlayBoatBg(AudioState state)
    {
        AudioClip clip = audioStore.GetAudioClip(state, AudioType.bg);
        boatEngine.clip = clip;
        boatEngine.Play();
        boatEngine.loop = true;
    }
    public void PlayVehicleBg(AudioState state)
    {
        AudioClip clip = audioStore.GetAudioClip(state, AudioType.bg);
        vehicleEngine.clip = clip;
        vehicleEngine.Play();
        vehicleEngine.loop = true;
    }

    public void Stop()
    {
        audioSource.Stop();
        boatEngine.Stop();
        BoatStart.Stop();
    }
    
    public void BoatEngineSoundStop()
    {
        boatEngine.Stop();
        BoatStart.Stop();
    }
    public void VehicleEngineSoundStop()
    {
        vehicleEngine.Stop();
    }
}
