using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SplatterParent : MonoBehaviour
{
    public void DeathSplatter(Transform deathTransform, GameObject deathSplatter, ColorChanger colorChanger)
    {
        GameObject newSplatter = Instantiate(deathSplatter, deathTransform.position, Quaternion.identity);
        SpriteRenderer splatterSpriteRenderer = newSplatter.GetComponent<SpriteRenderer>();
        newSplatter.transform.SetParent(this.transform);
        
        if (colorChanger != null) { splatterSpriteRenderer.color = colorChanger.CurrentColor; }
    }
}
