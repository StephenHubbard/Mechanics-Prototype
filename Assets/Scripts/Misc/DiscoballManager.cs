using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class DiscoballManager : MonoBehaviour
{
    public static Action OnDiscoballHit;

    [SerializeField] private float _discoPartyTime = 3f;
    [SerializeField] private float _discoGlobalLight = 0.1f;
    [SerializeField] private Light2D _globalLight;
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private VolumeProfile _volumeProfileLight;
    [SerializeField] private VolumeProfile _volumeProfileDark;

    private float _defaultGlobalLightIntensity;
    private Coroutine _discoCoroutine;
    private ColorSpotlight[] _allSpotlights;

    private void Awake()
    {
        _defaultGlobalLightIntensity = _globalLight.intensity;
    }

    private void Start()
    {
        _allSpotlights = FindObjectsOfType<ColorSpotlight>();
    }

    private void OnEnable() {
        OnDiscoballHit += DimTheLights;
    }

    private void OnDisable() {
        OnDiscoballHit -= DimTheLights;
    }

    public void DiscoballParty()
    {
        if (_discoCoroutine == null) { 
            OnDiscoballHit?.Invoke();
        }
    }

    private void DimTheLights() {
        _globalVolume.profile = _volumeProfileDark;
        _globalLight.intensity = _discoGlobalLight;

        foreach (ColorSpotlight spotLight in _allSpotlights)
        {
            StartCoroutine(spotLight.SpotLightDiscoParty(_discoPartyTime));
        }

        _discoCoroutine = StartCoroutine(GlobalLightResetRoutine());
    }

    private IEnumerator GlobalLightResetRoutine()
    {
        yield return new WaitForSeconds(_discoPartyTime);
        _globalVolume.profile = _volumeProfileLight;
        _globalLight.intensity = _defaultGlobalLightIntensity;
        _discoCoroutine = null;
    }
}
