using UnityEngine;

public class PlayerWeapon : Weapon
{
    [SerializeField] private float m_recoilForce;
    
    public float RecoilForce => m_recoilForce;
    public Transform ShootingPivot => m_shootingPivot;
}
