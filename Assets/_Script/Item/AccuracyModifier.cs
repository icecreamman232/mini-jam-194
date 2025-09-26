using SGGames.Script.Core;
using UnityEngine;

public class AccuracyModifier : ItemModifier
{
    public override void Apply(float val)
    {
        var playerRef = ServiceLocator.GetService<LevelManager>().Player;
        var playerController = playerRef.GetComponent<PlayerController>();

        playerController.WeaponHandler.Weapon.ModifyAccuracy(val);
        
        Debug.Log($"Apply accuracy modifier:{val}");
        base.Apply(val);
    }
}
