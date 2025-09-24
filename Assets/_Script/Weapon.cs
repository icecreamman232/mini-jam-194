using System.Collections;
using SGGames.Script.Core;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected Transform m_shootingPivot;
    [SerializeField] private float m_delayAfterShot;
    [SerializeField] private ObjectPooler<Bullet> m_bulletPooler;
    
    private bool m_canShoot = true;
    
    public virtual bool Shoot(Vector2 aimDirection)
    {
        if (!m_canShoot) return false;
        
        var newBullet = m_bulletPooler.GetPooledObject();
        newBullet.transform.position = m_shootingPivot.position;
        newBullet.transform.up = aimDirection;
        newBullet.Spawn();
        
        StartCoroutine(OnDelayAfterShot());
        return true;
    }

    private IEnumerator OnDelayAfterShot()
    {
        m_canShoot = false;
        yield return new WaitForSeconds(m_delayAfterShot);
        m_canShoot = true;
    }
}
