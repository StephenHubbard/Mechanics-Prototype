using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Range(.1f, 1f)]
    [SerializeField] private float _masterVolume = .7f;
    [SerializeField] private AudioSource _sfxSource;

    public void PlaySound(AudioClip clip, AudioMixerGroup audioMixerGruop, float volume, float pitch) {
        if (!MechanicsManager.Instance.SFXToggle) { return; }
        
        _sfxSource.outputAudioMixerGroup = audioMixerGruop;
        _sfxSource.pitch = pitch;
        _sfxSource.volume = volume * _masterVolume;
        _sfxSource.PlayOneShot(clip, volume);
    }
}
