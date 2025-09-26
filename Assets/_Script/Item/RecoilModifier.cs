using SGGames.Script.Core;
using UnityEngine;

public class RecoilModifier : ItemModifier
{
    public override void Apply(float value)
    {
        var playerRef = ServiceLocator.GetService<LevelManager>().Player;
        var controller = playerRef.GetComponent<PlayerController>();
        controller.WeaponHandler.Weapon.ModifyRecoilForce(value);
        
        Debug.Log($"Apply recoil modifier:{value}");
        base.Apply(value);
    }
}
