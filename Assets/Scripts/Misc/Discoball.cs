using UnityEngine;
using System;

public class Discoball : MonoBehaviour, IDamageable
{
    private DiscoballManager _discoballManager;
    private Flash _flash;

    private void Awake()
    {
        _discoballManager = FindFirstObjectByType<DiscoballManager>();
        _flash = GetComponent<Flash>();
    }

    public void TakeHit(int damageAmount)
    {
        _discoballManager.DiscoballParty();
        _flash.StartFlash();
    }
}
