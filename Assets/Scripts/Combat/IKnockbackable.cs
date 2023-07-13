using UnityEngine;

public interface IKnockbackable
{
    void HandleKnockback(Vector2 playerPosOnFire, float knockbackForce);
}
