using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Range(0, 1f)]
    [SerializeField] private float _masterVolume = .7f;
    [SerializeField] private float _randomPitchRange = .1f;
    [SerializeField] private SoundsSO soundSO;

    private List<Health> _deadEnemies = new List<Health>();
    private Coroutine _enemyDeathCoroutine;

    private void OnEnable()
    {
        Health.OnDeath += HandleDeath;
        Gun.OnShoot += Gun_OnShoot;
        Gun.OnGrenadeShoot += Gun_OnGrenadeShoot;
        Grenade.OnBeep += Grenade_OnBeep;
        Grenade.OnExplode += Grenade_OnExplode;
        PlayerController.OnPlayerHit += PlayerController_OnPlayerHit;
        PlayerController.OnJetpack += PlayerController_OnJetpack;
    }

    private void OnDisable() {
        Health.OnDeath -= HandleDeath;
        Gun.OnShoot -= Gun_OnShoot;
        Gun.OnGrenadeShoot -= Gun_OnGrenadeShoot;
        Grenade.OnBeep -= Grenade_OnBeep;
        Grenade.OnExplode -= Grenade_OnExplode;
        PlayerController.OnPlayerHit -= PlayerController_OnPlayerHit;
        PlayerController.OnJetpack -= PlayerController_OnJetpack;
    }

    #region Sound Methods

    public void SoundToPlay(Sound sound, bool randomizePitch = false) {
        AudioClip audioClip = sound.clip;
        if (randomizePitch) { 
            PlaySoundRandomizePitch(audioClip, transform.position, sound.volume, sound.pitch);
        } else {
            PlaySound(audioClip, transform.position, sound.volume, sound.pitch);
        }
    }

    public void PlaySoundRandomizePitch(AudioClip audioClip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        float randomPitch = pitch - Random.Range(-_randomPitchRange, _randomPitchRange);
        PlaySound(audioClip, transform.position, volume, randomPitch);
    }

    public void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        GameObject soundObject = new GameObject("Temp Audio Source");
        soundObject.transform.position = position;
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume * _masterVolume;
        audioSource.pitch = pitch;
        audioSource.Play();
        Destroy(soundObject, audioClip.length);
    }

    #endregion

    #region Sounds

    private void Health_OnDeath(Health sender) {
        Sound sound = soundSO.Splat[Random.Range(0, soundSO.Splat.Length)];
        SoundToPlay(sound, true);
    }

    private void Health_OnDeath()
    {
        Sound sound = soundSO.Splat[Random.Range(0, soundSO.Splat.Length)];
        SoundToPlay(sound, true);
    }

    private void Gun_OnShoot()
    {
        Sound sound = soundSO.GunShoot[Random.Range(0, soundSO.GunShoot.Length)];
        SoundToPlay(sound, true);
    }

    private void Gun_OnGrenadeShoot()
    {
        Sound sound = soundSO.GrenadeShoot[Random.Range(0, soundSO.GrenadeShoot.Length)];
        SoundToPlay(sound, true);
    }

    private void Grenade_OnBeep()
    {
        Sound sound = soundSO.GrenadeBeep[Random.Range(0, soundSO.GrenadeBeep.Length)];
        SoundToPlay(sound);
    }

    private void Grenade_OnExplode(Grenade sender)
    {
        Sound sound = soundSO.GrenadeExplosion[Random.Range(0, soundSO.GrenadeExplosion.Length)];
        SoundToPlay(sound);
    }

    private void PlayerController_OnPlayerHit()
    {
        Sound sound = soundSO.PlayerHit[Random.Range(0, soundSO.PlayerHit.Length)];
        SoundToPlay(sound);
    }

    private void PlayerController_OnJetpack()
    {
        Sound sound = soundSO.Jetpack[Random.Range(0, soundSO.Jetpack.Length)];
        SoundToPlay(sound);
    }

    private void MegaKill() {
        Sound sound = soundSO.MegaKill[Random.Range(0, soundSO.MegaKill.Length)];
        SoundToPlay(sound);
    }

    #endregion

    #region Custom SFX Logic
        
    private void HandleDeath(Health enemy)
    {
        _deadEnemies.Add(enemy);

        if (_enemyDeathCoroutine == null)
        {
            _enemyDeathCoroutine = StartCoroutine(EnemyDeathWindowCoroutine());
        }
    }

    private IEnumerator EnemyDeathWindowCoroutine()
    {
        yield return null;

        int megaKillMinAmount = 3;
        
        if (_deadEnemies.Count >= megaKillMinAmount)
        {
            MegaKill();
        }

        Health_OnDeath();
        
        _deadEnemies.Clear();
        _enemyDeathCoroutine = null;
    }

    #endregion
}
