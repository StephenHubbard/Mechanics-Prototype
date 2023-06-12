using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] _clips;

    [Range(0f, 2f)]
    [SerializeField] private float _volume = 1f;

    [Range(0f, 2f)]
    [SerializeField] private float _pitch = 1f;

    [ContextMenu("Play Sound")]
    public void PlaySound(int soundToPlay)
    {
        AudioManager.Instance.PlaySound(_clips[soundToPlay], null, _volume, _pitch);
    }
}
