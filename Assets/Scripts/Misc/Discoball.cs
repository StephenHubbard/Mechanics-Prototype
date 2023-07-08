using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class Discoball : MonoBehaviour, IHitable
{
    private DiscoballManager _discoballManager;

    private void Awake()
    {
        _discoballManager = FindObjectOfType<DiscoballManager>();
    }

    public void TakeHit(RaycastHit2D hit, Vector2 playerPosOnFire, float knockbackForce, int damageAmount)
    {
        _discoballManager.DiscoballParty();
        
        Flash flash = GetComponent<Flash>();
        StartCoroutine(flash.FlashRoutine());
    }
}
