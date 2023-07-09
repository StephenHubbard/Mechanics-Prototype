using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ColorSpotlight : MonoBehaviour
{
    [SerializeField] private GameObject _spotlightHead;
    [SerializeField] private Light2D _light;
    [SerializeField] private Color[] _lightColors;
    [SerializeField] private float _rotationSpeed = 1f;
    [SerializeField] private float _discoPartyRotationSpeed = 7f;
    [SerializeField] private float _maxRotation = 45;

    private float _randomStartingZ;
    private float _timeOffset;

    // Start method might make a good challenge.  Kinda feels like I overly complicated it though. 
    private void Start()
    {
        StartingRotation();
        ChangeLightColor();
        StartCoroutine(ChangeColorRoutine());
    }

    private void Update()
    {
        RotateHead();
    }

    public IEnumerator SpotLightDiscoParty(float discoPartyTime) {
        float defaultRotSpeed = _rotationSpeed;
        _rotationSpeed = _discoPartyRotationSpeed;
        yield return new WaitForSeconds(discoPartyTime);
        _rotationSpeed = defaultRotSpeed;
    }

    private void RotateHead() {
        float z = Mathf.PingPong((Time.time - _timeOffset) * _rotationSpeed, _maxRotation);
        _spotlightHead.transform.localRotation = Quaternion.Euler(0, 0, z);
    }

    private void StartingRotation() {
        _randomStartingZ = Random.Range(0, _maxRotation);
        _spotlightHead.transform.localEulerAngles = new Vector3(0, 0, _randomStartingZ);
        _timeOffset = _randomStartingZ / _maxRotation;
    }

    private void ChangeLightColor() {
        int randomColor = Random.Range(0, _lightColors.Length);
        _light.color = _lightColors[randomColor];
    }

    private IEnumerator ChangeColorRoutine() {
        while (true)
        {
            float minChangeTime = 2f;
            float maxChangeTime = 4f;
            float changeColorInterval = Random.Range(minChangeTime, maxChangeTime);
            yield return new WaitForSeconds(changeColorInterval);
            ChangeLightColor();
        }
    }

}
