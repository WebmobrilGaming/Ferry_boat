using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AudioStore",menuName ="Store/Audio")]
public class AudioStore : ScriptableObject
{
    [Header("Audio_Datas")]
    [SerializeField] List<AudioData> audioDatas = new List<AudioData>();

    public AudioClip GetAudioClip(AudioState state, AudioType type)
    {
        return audioDatas.Find(x => x.type == type && x.state == state).clip;
    }
}

[Serializable]
public class AudioData
{
    public AudioClip clip;
    public AudioState state;
    public AudioType type;
}


public enum AudioState { crash,boat };
public enum AudioType { sfx,bg }
