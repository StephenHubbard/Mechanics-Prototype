using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class Fade : MonoBehaviour
{
    public float FadeTime => _fadeTime;

    [SerializeField] private float _fadeTime = 2f;
    [SerializeField] private Image _imageToFade;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _respawnPoint;

    private CinemachineVirtualCamera _cam;
    private IEnumerator fadeRoutine;

    public void Awake()
    {
        _cam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    public void RespawnPlayer() {
            FadeToBlackAndRespawn();
    }

    public void FadeToBlackAndRespawn()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = FadeRoutineAndRespawn(1f);
        StartCoroutine(fadeRoutine);
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float elapsedTime = 0f;
        float startValue = _imageToFade.color.a;

        while (elapsedTime < _fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, targetAlpha, elapsedTime / _fadeTime);
            _imageToFade.color = new Color(_imageToFade.color.r, _imageToFade.color.g, _imageToFade.color.b, newAlpha);
            yield return null;
        }

        _imageToFade.color = new Color(_imageToFade.color.r, _imageToFade.color.g, _imageToFade.color.b, targetAlpha);
    }

    private IEnumerator FadeRoutineAndRespawn(float targetAlpha)
    {
        yield return FadeRoutine(targetAlpha);
        Respawn();
        yield return FadeRoutine(0f);
    }

    private IEnumerator RespawnRoutine() {
        yield return null;
        Respawn();
    }

    private void Respawn()
    {
        GameObject player = Instantiate(_playerPrefab, _respawnPoint.position, Quaternion.identity);
        _cam.Follow = player.transform;
    }
}
