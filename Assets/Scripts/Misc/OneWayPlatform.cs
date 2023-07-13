using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{   
    [SerializeField] private float _disableColliderTime = .5f;

    private Collider2D _collider;

    private void Awake() {
        _collider = GetComponent<Collider2D>();
    }

    public IEnumerator DisablePlatformColliderRoutine(PlayerController playerController)
    {
        Collider2D[] playerColliders = playerController.GetComponents<Collider2D>();

        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, _collider, true);
        }

        yield return new WaitForSeconds(_disableColliderTime);

        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, _collider, false);
        }
    }
}
