using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Range(.1f, 1f)]
    [SerializeField] private float _masterVolume = .7f;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip _groupDeathSFX;
    [SerializeField] private AudioClip _onEnemyDeathSFX;

    private List<Health> _deadEnemies = new List<Health>();
    private Coroutine _enemyDeathCoroutine;

    public void PlaySound(AudioClip clip, float volume, float pitch)
    {
        _sfxSource.pitch = pitch;
        _sfxSource.volume = volume * _masterVolume;
        _sfxSource.PlayOneShot(clip, volume);
    }

    private void OnEnable()
    {
        Health.OnEnemyDeath += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        Health.OnEnemyDeath -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath(Health enemy)
    {
        _deadEnemies.Add(enemy);

        if (_enemyDeathCoroutine == null)
        {
            _enemyDeathCoroutine = StartCoroutine(EnemyDeathWindowCoroutine());
        }
    }

    private IEnumerator EnemyDeathWindowCoroutine()
    {
        float timeWindow = .05f;
        yield return new WaitForSeconds(timeWindow); 

        int megaKillInt = 3;
        if (_deadEnemies.Count >= megaKillInt)
        {
            PlaySound(_groupDeathSFX, .8f, 1f);
        }
        
        PlaySound(_onEnemyDeathSFX, .8f, 1f);

        _deadEnemies.Clear();
        _enemyDeathCoroutine = null;
    }
}
