using UnityEngine;

public class Coin : PickupBehavior, IPickup
{
    public void Picking(Transform player)
    {
        MoveToPlayer(player, Pickup);
    }

    public void Pickup()
    {
        Debug.Log("Picked up 1 coin");
        this.gameObject.SetActive(false);
    }
}
