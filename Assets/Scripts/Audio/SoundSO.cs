using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// change to SO
[CreateAssetMenu()]
public class SoundSO : ScriptableObject
{
    public AudioClip Clip;
    public enum AudioTypes { SFX, Music };
    public AudioTypes AudioType;
    public bool Loop = false;
    public bool RandomizePitch = false;
    [Range(0f, 1f)]
    public float RandomPitchRangeModifier = .1f;
    [Range(0f, 2f)]
    public float Volume = 1f;
    [Range(.1f, 3f)]
    public float Pitch = 1f;
}