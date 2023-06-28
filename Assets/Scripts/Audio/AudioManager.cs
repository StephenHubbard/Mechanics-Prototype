using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Range(.1f, 1f)]
    [SerializeField] private float _masterVolume = .7f;
    [SerializeField] private AudioSource _sfxSource;

    public void PlaySound(AudioClip clip, float volume, float pitch) {
        // _sfxSource.outputAudioMixerGroup = audioMixerGroup;
        _sfxSource.pitch = pitch;
        _sfxSource.volume = volume * _masterVolume;
        _sfxSource.PlayOneShot(clip, volume);
    }
}
