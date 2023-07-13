using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// So ListSO or SoundCollectionSO
[CreateAssetMenu()]
public class SoundsCollectionSO : ScriptableObject 
{
    // Music
    public SoundSO[] DiscoballMusic;
    public SoundSO[] FightMusic;

    // SFX
    public SoundSO[] GunShoot;
    public SoundSO[] GrenadeShoot;
    public SoundSO[] GrenadeBeep;
    public SoundSO[] GrenadeExplosion;
    public SoundSO[] PlayerHit;
    public SoundSO[] Splat;
    public SoundSO[] Jetpack;
    public SoundSO[] MegaKill;
    public SoundSO[] Jump;
}
