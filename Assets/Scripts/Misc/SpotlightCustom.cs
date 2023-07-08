using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpotlightCustom : MonoBehaviour
{
    [SerializeField] private GameObject _spotlightHead;
    [SerializeField] private Light2D _light;
    [SerializeField] private Color[] _lightColors;
    [SerializeField] private float _rotSpeed = 1f;
    [SerializeField] private float _discoPartyRotSpeed = 7f;
    [SerializeField] private float _maxRot = 90f;

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
        float defaultRotSpeed = _rotSpeed;
        _rotSpeed = _discoPartyRotSpeed;
        yield return new WaitForSeconds(discoPartyTime);
        _rotSpeed = defaultRotSpeed;
    }

    private void RotateHead() {
        float z = Mathf.PingPong((Time.time - _timeOffset) * _rotSpeed, _maxRot);
        _spotlightHead.transform.localRotation = Quaternion.Euler(0, 0, z);
    }

    private void StartingRotation() {
        _randomStartingZ = Random.Range(0, _maxRot);
        _spotlightHead.transform.localEulerAngles = new Vector3(0, 0, _randomStartingZ);
        _timeOffset = _randomStartingZ / _maxRot;
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
