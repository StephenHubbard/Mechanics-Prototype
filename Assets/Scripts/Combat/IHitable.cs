using UnityEngine;

interface IHitable
{
    void TakeHit(RaycastHit2D hit, Vector2 playerPosOnFire, float knockbackForce, int damageAmount);
}