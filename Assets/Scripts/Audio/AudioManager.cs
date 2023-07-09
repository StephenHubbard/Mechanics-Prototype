using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [Range(0, 1f)]
    [SerializeField] private float _masterVolume = .7f;
    [SerializeField] private float _randomPitchRange = .1f;
    [SerializeField] private SoundsSO soundSO;
    
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

    public void SoundToPlay(Sound sound, bool randomizePitch = false) {
        AudioClip audioClip = sound.Clip;
        AudioMixerGroup audioMixerGroup;

        switch (sound.AudioType)
        {
            case Sound.AudioTypes.SFX:
                audioMixerGroup = _sfxMixerGroup;
                break;

            case Sound.AudioTypes.Music:
                audioMixerGroup = _musicMixerGroup;
                break;
            
            default:
                audioMixerGroup = null;
                break;
        }

        if (randomizePitch) { 
            PlaySoundRandomizePitch(audioClip, audioMixerGroup, sound.Loop, transform.position, sound.Volume, sound.Pitch);
        } else {
            PlaySound(audioClip, audioMixerGroup, sound.Loop, transform.position, sound.Volume, sound.Pitch);
        }
    }

    public void PlaySoundRandomizePitch(AudioClip audioClip, AudioMixerGroup audioMixerGroup, bool loop, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        float randomPitch = pitch - Random.Range(-_randomPitchRange, _randomPitchRange);
        PlaySound(audioClip, audioMixerGroup, loop, transform.position, volume, randomPitch);
    }

    public void PlaySound(AudioClip audioClip, AudioMixerGroup audioMixerGroup, bool loop, Vector3 position, float volume = 1f, float pitch = 1f)
    {
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
        Sound sound = soundSO.Discoball_Music[Random.Range(0, soundSO.MegaKill.Length)];
        SoundToPlay(sound);
        Utils.RunAfterDelay(this, sound.Clip.length, FightMusic);
    }

    public void FightMusic()
    {
        Sound sound = soundSO.Fight_Music[Random.Range(0, soundSO.Fight_Music.Length)];
        SoundToPlay(sound);
    }

    #endregion

    #region Sounds

    public void Health_OnDeath(Health sender) {
        Sound sound = soundSO.Splat[Random.Range(0, soundSO.Splat.Length)];
        SoundToPlay(sound, true);
    }

    public void Health_OnDeath()
    {
        Sound sound = soundSO.Splat[Random.Range(0, soundSO.Splat.Length)];
        SoundToPlay(sound, true);
    }

    public void Gun_OnShoot()
    {
        Sound sound = soundSO.GunShoot[Random.Range(0, soundSO.GunShoot.Length)];
        SoundToPlay(sound, true);
    }

    public void Gun_OnGrenadeShoot()
    {
        Sound sound = soundSO.GrenadeShoot[Random.Range(0, soundSO.GrenadeShoot.Length)];
        SoundToPlay(sound, true);
    }

    public void Grenade_OnBeep()
    {
        Sound sound = soundSO.GrenadeBeep[Random.Range(0, soundSO.GrenadeBeep.Length)];
        SoundToPlay(sound);
    }

    public void Grenade_OnExplode()
    {
        Sound sound = soundSO.GrenadeExplosion[Random.Range(0, soundSO.GrenadeExplosion.Length)];
        SoundToPlay(sound);
    }

    public void PlayerController_OnPlayerHit(Enemy enemy)
    {
        Sound sound = soundSO.PlayerHit[Random.Range(0, soundSO.PlayerHit.Length)];
        SoundToPlay(sound);
    }

    public void PlayerController_OnJetpack()
    {
        Sound sound = soundSO.Jetpack[Random.Range(0, soundSO.Jetpack.Length)];
        SoundToPlay(sound);
    }

    public void MegaKill() {
        Sound sound = soundSO.MegaKill[Random.Range(0, soundSO.MegaKill.Length)];
        SoundToPlay(sound);
    }

    public void PlayerController_OnJump()
    {
        Sound sound = soundSO.Jump[Random.Range(0, soundSO.MegaKill.Length)];
        SoundToPlay(sound, true);
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
