using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New Sound", menuName = "ScriptableObjects/Sound", order = 2)]
public class Sound : ScriptableObject
{
    public string nameSong;
    [Range(0f, 1.5f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    public AudioClip song;
    public bool loop;
    public bool playOnAwake;
    public AudioMixerGroup mixerGroup;
    [HideInInspector]
    public AudioSource source;
    public MusicType musicType;
}

