using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class Fade : MonoBehaviour
{
    public float FadeTime => _fadeTime;

    [SerializeField] private float _fadeTime = 2f;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _respawnPoint;

    private Image _imageToFade;
    private CinemachineVirtualCamera _virtualCam;
    private IEnumerator fadeRoutine;

    public void Awake()
    {
        _imageToFade = GetComponent<Image>();
        _virtualCam = FindFirstObjectByType<CinemachineVirtualCamera>();
    }

    public void FadeIn()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = FadeOut(1f);
        StartCoroutine(fadeRoutine);
    }

    private IEnumerator FadeOut(float targetAlpha)
    {
        yield return FadeRoutine(targetAlpha);
        Respawn();
        yield return FadeRoutine(0f);
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

    private void Respawn()
    {
        GameObject player = Instantiate(_playerPrefab, _respawnPoint.position, Quaternion.identity);
        _virtualCam.Follow = player.transform;
    }
}
