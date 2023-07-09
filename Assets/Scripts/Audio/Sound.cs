using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioClip Clip;
    public enum AudioTypes { SFX, Music };
    public AudioTypes AudioType;
    public bool Loop = false;
    [Range(0f, 2f)] public float Volume = 1f;
    [Range(.1f, 3f)] public float Pitch = 1f;
}