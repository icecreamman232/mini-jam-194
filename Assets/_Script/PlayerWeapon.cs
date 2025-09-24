using UnityEngine;

public class PlayerWeapon : Weapon
{
    [SerializeField] private float m_recoilForce;
    [SerializeField] [Range(1,5)] private float m_accuracy;
    
    public float RecoilForce => m_recoilForce;
    public Transform ShootingPivot => m_shootingPivot;
    
    private const float k_DefaultDifferentAngle = 12;
    private const float k_DifferentAnglePerAccuracy = 2;
    
    private Vector2 ApplyAccuracy(Vector2 inputDirection)
    {
        var differentAngle = k_DefaultDifferentAngle - ((m_accuracy > 5 ? 5 : m_accuracy) * k_DifferentAnglePerAccuracy);
        var randomAngle = Random.Range(-differentAngle, differentAngle);
        var finalDirection = Quaternion.Euler(0, 0, randomAngle) * inputDirection;
        return finalDirection;
    }

    public override bool Shoot(Vector2 aimDirection)
    {
        aimDirection = ApplyAccuracy(aimDirection);
        
        return base.Shoot(aimDirection);
    }
}
