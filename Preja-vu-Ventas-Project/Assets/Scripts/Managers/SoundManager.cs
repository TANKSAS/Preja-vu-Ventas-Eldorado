using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Controller Sound")]
    [SerializeField]
    Button muteButton;
    [SerializeField]
    bool mute;
    [SerializeField]
    Sprite[] songIcons;
    public AudioMixer audioMixer;
    public List<Sound> soungs = new List<Sound>();
    [Header("Provicional Stats Sound Controller")]
    public List<AudioSource> provicionalLevelSound = new List<AudioSource>();
    [SerializeField]
    List<AudioSource> pauseSoundProvicional = new List<AudioSource>();
    float currentVolumen;

    public void CreateSoundsLevel()
    {
        foreach (Sound song in soungs)
        {
            song.source = gameObject.AddComponent<AudioSource>();
            provicionalLevelSound.Add(song.source);
            song.source.playOnAwake = song.playOnAwake;
            song.source.volume = song.volume;
            song.source.pitch = song.pitch;
            song.source.loop = song.loop;
            song.source.outputAudioMixerGroup = song.mixerGroup;
            song.source.clip = song.song;

            //if (song.nameSong != "")
            //{
            //    song.source.clip = LanguageManager.Instance.GetAudioClipValue(song.nameSong);
            //}
            //else
            //{
            //}
        }
    }

    public void SetVoiceAudioLanguage()
    {
        Debug.Log("Cambiando Pistas");

        for (int i = 0; i < soungs.Count; i++)
        {
            if (soungs[i].musicType == MusicType.Voice)
            {
                AudioClip newClip = LanguageManager.Instance.GetAudioClipValue(soungs[i].nameSong);
                soungs[i].source.clip  = newClip;
            }
        }
    }

    public void DeleteSoundsLevel()
    {
        foreach (AudioSource s in provicionalLevelSound)
        {
            Destroy(s);
        }

        provicionalLevelSound.Clear();
    }

    public void MuteAudio()
    {
        mute = !mute;

        if (mute)
        {
            muteButton.image.sprite = songIcons[1];
            audioMixer.GetFloat(Tags.VOLUMENMASTER_TAG, out currentVolumen);
            audioMixer.SetFloat(Tags.VOLUMENMASTER_TAG, 0);
        }
        else
        {
            muteButton.image.sprite = songIcons[0];
            audioMixer.SetFloat(Tags.VOLUMENMASTER_TAG, currentVolumen);
        }
    }

    public void PlayNewSound(AudioSource selectedSource)
    {
        AudioSource s = provicionalLevelSound.Find(source => source == selectedSource);
        
        if (s != null)
        {
            if (!s.isPlaying)
            {
                s.Play();
            }
        }
    }
    public void PauseSound(AudioSource name, bool validation)
    {
        AudioSource s = provicionalLevelSound.Find(sounds => sounds == name);
        
        if (validation)
        {
            if (s != null)
            {
                s.Pause();
            }
        }
        else
        {
            if (s != null)
            {
                s.Play();
            }
        }
    }
    public void PauseAllSounds(bool validation)
    {
        if (validation)
        {
            foreach (AudioSource s in provicionalLevelSound)
            {
                if (s.isPlaying)
                {
                    pauseSoundProvicional.Add(s);
                    s.Pause();
                }
            }
        }
        else
        {
            for (int i = 0; i < pauseSoundProvicional.Count; i++)
            {
                pauseSoundProvicional[i].Play();
            }

            pauseSoundProvicional.Clear();
        }
    }

    public void EndSound(AudioSource name)
    {
        AudioSource s = provicionalLevelSound.Find(sounds => sounds == name);

        if (s != null)
        {
            s.Stop();
        }
    }
}


