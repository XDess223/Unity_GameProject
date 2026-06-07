using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SoundEffectLibrary : MonoBehaviour
{
    [SerializeField] private SoundEffectGroup[] soundEffectGroups;
    private Dictionary<string, List<AudioClip>> soundDict;

    private void Awake()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        soundDict = new Dictionary<string, List<AudioClip>>();
        foreach(SoundEffectGroup soundEffectGroup in soundEffectGroups)
        {
            soundDict[soundEffectGroup.name] = soundEffectGroup.audioClips;
        }
    }
    public AudioClip GetRandomClip(string name)
    {
        if (soundDict.ContainsKey(name))
        {
            List<AudioClip> audioClips = soundDict[name];
            if (audioClips.Count > 0)
            {
                return audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
            }
        }
        return null;
    }
}

[Serializable]
public struct SoundEffectGroup
{
    public string name;
    public List<AudioClip> audioClips;
}