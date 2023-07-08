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

    private float _defaultGlobalLightIntensity;
    private Coroutine _discoCoroutine;
    private SpotlightCustom[] _allSpotlights;

    private void Awake()
    {
        _defaultGlobalLightIntensity = _globalLight.intensity;
    }

    private void Start()
    {
        _allSpotlights = FindObjectsOfType<SpotlightCustom>();
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
        _globalLight.intensity = _discoGlobalLight;

        foreach (SpotlightCustom spotLight in _allSpotlights)
        {
            spotLight.SpotLightDiscoParty(_discoPartyTime);
        }

        _discoCoroutine = StartCoroutine(GlobalLightResetRoutine());
    }

    private IEnumerator GlobalLightResetRoutine()
    {
        yield return new WaitForSeconds(_discoPartyTime);
        _globalLight.intensity = _defaultGlobalLightIntensity;
        _discoCoroutine = null;
    }
}
