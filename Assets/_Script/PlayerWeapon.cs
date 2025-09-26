using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : Weapon
{
    [SerializeField] private int m_magazineSize;
    [SerializeField] private int m_currrentMagazine;
    [SerializeField] private float m_reloadTime;
    [SerializeField] private float m_recoilForce;
    [SerializeField] [Range(1,5)] private float m_accuracy;
    [SerializeField] private PlayerReloadEvent m_playerReloadEvent;
    [SerializeField] private PlayerMagazineEvent m_playerMagazineEvent;
    
    public float RecoilForce => m_recoilForce;
    public Transform ShootingPivot => m_shootingPivot;
    public bool IsReloading => m_isReloading;

    private bool m_isReloading;
    private ReloadEventData m_reloadEventData = new ReloadEventData();
    private float m_reloadTimer;
    private const float k_DefaultDifferentAngle = 12;
    private const float k_DifferentAnglePerAccuracy = 2;

    private void Start()
    {
        m_currrentMagazine = m_magazineSize;
        m_playerMagazineEvent.Raise(m_currrentMagazine);
    }

    private Vector2 ApplyAccuracy(Vector2 inputDirection)
    {
        var differentAngle = k_DefaultDifferentAngle - ((m_accuracy > 5 ? 5 : m_accuracy) * k_DifferentAnglePerAccuracy);
        var randomAngle = Random.Range(-differentAngle, differentAngle);
        var finalDirection = Quaternion.Euler(0, 0, randomAngle) * inputDirection;
        return finalDirection;
    }

    #region Modification Methods
    public void ModifyAccuracy(float accuracy)
    {
        m_accuracy += accuracy;
        m_accuracy = Mathf.Clamp(m_accuracy, 1, 5);
    }
    
    public void ModifyRecoilForce(float recoilForce)
    {
        m_recoilForce += recoilForce;
        m_recoilForce = Mathf.Clamp(m_recoilForce, 0, 10);
    }
    
    #endregion

    public void ManualReload()
    {
        if (m_isReloading) return;
        StartCoroutine(OnReloading());
    }

    public override bool Shoot(Vector2 aimDirection)
    {
        aimDirection = ApplyAccuracy(aimDirection);

        if (base.Shoot(aimDirection))
        {
            m_currrentMagazine--;
            m_playerMagazineEvent.Raise(m_currrentMagazine);
            if (m_currrentMagazine <= 0)
            {
                m_currrentMagazine = 0;
                m_playerMagazineEvent.Raise(m_currrentMagazine);
                StartCoroutine(OnReloading());
            }
            return true;
        }
        return false;
    }

    private IEnumerator OnReloading()
    {
        m_isReloading = true;
        var timeStop = Time.time + m_reloadTime;
        while (timeStop > Time.time)
        {
            m_reloadTimer += Time.deltaTime;
            m_reloadEventData.CurrentReloadTime = m_reloadTimer;
            m_reloadEventData.MaxReloadTime = m_reloadTime;
            m_playerReloadEvent.Raise(m_reloadEventData);
            yield return null;
        }

        m_currrentMagazine = m_magazineSize;
        m_playerMagazineEvent.Raise(m_currrentMagazine);
        
        m_reloadTimer = 0;
        m_isReloading = false;
    }
}
