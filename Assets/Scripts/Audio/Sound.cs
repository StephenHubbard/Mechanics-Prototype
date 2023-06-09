using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private bool _randomizePitch;
    [SerializeField] private float _pitchRange = 0.1f;

    [Range(0f, 2f)]
    [SerializeField] private float _volume = 1f;

    [Range(0f, 2f)]
    [SerializeField] private float _pitch = 1f;

    [ContextMenu("Play Sound")]
    public void PlaySound()
    {
        if (!MechanicsManager.Instance.SFXToggle) { return; }

        int randomClip = Random.Range(0, _clips.Length);
        float finalPitch = _randomizePitch ? _pitch + Random.Range(-_pitchRange, _pitchRange) : _pitch;
        AudioManager.Instance.PlaySound(_clips[randomClip], _volume, finalPitch);
    }
}
