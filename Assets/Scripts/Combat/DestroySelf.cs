using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [SerializeField] private float _selfDestroyTimer = .1f;

    private IEnumerator Start() {
        yield return new WaitForSeconds(_selfDestroyTimer);
        Destroy(gameObject);
    }
}
