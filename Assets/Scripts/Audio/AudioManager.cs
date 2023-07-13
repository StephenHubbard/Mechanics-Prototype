using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundsCollectionSO _soundsCollectionSO;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [Range(0, 1f)]
    [SerializeField] private float _masterVolume = .7f;
    
    private AudioSource _currentMusic;

    #region Unity Methods

    private void Start() {
        FightMusic();
    }

    private void OnEnable()
    {
        Health.OnDeath += HandleDeath;
        Gun.OnShoot += Gun_OnShoot;
        Gun.OnGrenadeShoot += Gun_OnGrenadeShoot;
        PlayerController.OnPlayerHit += PlayerController_OnPlayerHit;
        PlayerController.OnJetpack += PlayerController_OnJetpack;
        DiscoballManager.OnDiscoballHit += Discoball_Music;
        PlayerController.OnJump += PlayerController_OnJump;
    }

    private void OnDisable()
    {
        Health.OnDeath -= HandleDeath;
        Gun.OnShoot -= Gun_OnShoot;
        Gun.OnGrenadeShoot -= Gun_OnGrenadeShoot;
        PlayerController.OnPlayerHit -= PlayerController_OnPlayerHit;
        PlayerController.OnJetpack -= PlayerController_OnJetpack;
        DiscoballManager.OnDiscoballHit -= Discoball_Music;
        PlayerController.OnJump -= PlayerController_OnJump;
    }

    #endregion

    #region Sound Methods

    private void PlayRandomSound(SoundSO[] sounds)
    {
        SoundSO soundSO = null;

        if (sounds != null && sounds.Length > 0)
        {
            soundSO = sounds[Random.Range(0, sounds.Length)];

            SoundToPlay(soundSO);
        }
    }

    public void SoundToPlay(SoundSO soundSO) {
        AudioClip audioClip = soundSO.Clip;
        AudioMixerGroup audioMixerGroup;

        switch (soundSO.AudioType)
        {
            case SoundSO.AudioTypes.SFX:
                audioMixerGroup = _sfxMixerGroup;
                break;

            case SoundSO.AudioTypes.Music:
                audioMixerGroup = _musicMixerGroup;
                break;
            
            default:
                audioMixerGroup = null;
                break;
        }

        float soundPitch;

        if (soundSO.RandomizePitch) {
            float randomPitchModifier = Random.Range(-soundSO.RandomPitchRangeModifier, soundSO.RandomPitchRangeModifier);
            soundPitch = soundSO.Pitch + randomPitchModifier;
        } else {
            soundPitch = soundSO.Pitch;
        }

        PlaySound(audioClip, audioMixerGroup, soundSO.Loop, transform.position, soundSO.Volume, soundPitch);
    }

    public void PlaySound(AudioClip audioClip, AudioMixerGroup audioMixerGroup, bool loop, Vector3 position, float volume, float pitch)
    {
        // garbage collection gonna add to performance negatively
        GameObject soundObject = new GameObject("Temp Audio Source");
        soundObject.transform.position = position;
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.loop = loop;
        audioSource.volume = volume * _masterVolume;
        audioSource.pitch = pitch;
        audioSource.Play();

        if (!loop) {
            Destroy(soundObject, audioClip.length);
        }

        if (audioMixerGroup == _musicMixerGroup) {
            if (_currentMusic != null) {
                _currentMusic.Stop();
            }

            _currentMusic = audioSource;
        }
    }

    #endregion

    #region Music

    public void Discoball_Music()
    {
        PlayRandomSound(_soundsCollectionSO.DiscoballMusic);
        float soundLength = _soundsCollectionSO.DiscoballMusic[0].Clip.length;
        Utils.RunAfterDelay(this, soundLength, FightMusic);
    }

    public void FightMusic()
    {
        PlayRandomSound(_soundsCollectionSO.FightMusic);
    }

    #endregion

    #region Sounds

    public void Health_OnDeath()
    {
        PlayRandomSound(_soundsCollectionSO.Splat);
    }

    public void Gun_OnShoot()
    {
        PlayRandomSound(_soundsCollectionSO.GunShoot);
    }

    public void Gun_OnGrenadeShoot()
    {
        PlayRandomSound(_soundsCollectionSO.GrenadeShoot);
    }

    public void Grenade_OnBeep()
    {
        PlayRandomSound(_soundsCollectionSO.GrenadeBeep);
    }

    public void Grenade_OnExplode()
    {
        PlayRandomSound(_soundsCollectionSO.GrenadeExplosion);
    }

    public void PlayerController_OnPlayerHit(Enemy enemy)
    {
        PlayRandomSound(_soundsCollectionSO.PlayerHit);
    }

    public void PlayerController_OnJetpack()
    {
        PlayRandomSound(_soundsCollectionSO.Jetpack);
    }

    public void MegaKill() {
        PlayRandomSound(_soundsCollectionSO.MegaKill);
    }

    public void PlayerController_OnJump()
    {
        PlayRandomSound(_soundsCollectionSO.Jump);
    }

    #endregion

    #region Custom SFX Logic

    private List<Health> _deadEnemies = new List<Health>();
    private Coroutine _enemyDeathCoroutine;
        
    private void HandleDeath(Health health)
    {
        if (health.IsEnemy()) {
            _deadEnemies.Add(health);
        }

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
