using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeathSplatterHandler : MonoBehaviour
{
    private void OnEnable()
    {
        Health.OnDeath += DeathVFX;
        Health.OnDeath += DeathSplatter;
    }

    private void OnDisable()
    {
        Health.OnDeath -= DeathVFX;
        Health.OnDeath -= DeathSplatter;
    }

    public void DeathSplatter(Health sender)
    {
        GameObject deathSplatter = sender.DeathSplatter;
        Vector3 deathPosition = sender.transform.position;
        GameObject newSplatter = Instantiate(deathSplatter, deathPosition, Quaternion.identity);
        newSplatter.transform.SetParent(this.transform);
        SpriteRenderer splatterSpriteRenderer = newSplatter.GetComponent<SpriteRenderer>();
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();

        if (colorChanger) { splatterSpriteRenderer.color = colorChanger.DefaultColor; }
    }

    public void DeathVFX(Health sender)
    {
        GameObject deathVFX = sender.DeathVFX;
        Vector3 deathPosition = sender.transform.position;
        GameObject newDeathVFX = Instantiate(deathVFX, deathPosition, Quaternion.identity);
        ParticleSystem.MainModule ps = newDeathVFX.GetComponent<ParticleSystem>().main;
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();
        
        if (colorChanger) { ps.startColor = colorChanger.DefaultColor; }
    }
}
