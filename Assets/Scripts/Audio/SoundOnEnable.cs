using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnEnable : MonoBehaviour
{
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private bool _randomizePitch;
    [SerializeField] private float _pitchRange = 0.1f;

    [Range(0f, 2f)]
    [SerializeField] private float _volume = 1f;

    [Range(0f, 2f)]
    [SerializeField] private float _pitch = 1f;

    private void OnEnable()
    {
        PlaySound(); 
    }

    [ContextMenu("Play Sound")]
    public void PlaySound() {
        int randomClip = Random.Range(0, _clips.Length);
        float finalPitch = _randomizePitch ? _pitch + Random.Range(-_pitchRange, _pitchRange) : _pitch;
        AudioManager.Instance.PlaySound(_clips[randomClip], _volume, finalPitch); 
    }
}
