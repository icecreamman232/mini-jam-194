using System.Collections;
using UnityEngine;

public class BounceLaserAI : EnemyAI
{
    [SerializeField] private LaserLine m_laserLine;
    [SerializeField] private int m_numberBounces;

    private bool m_canShowNext;
    private bool m_hasShowLasers;
    [ContextMenu("Test")]
    private void Test()
    {
        m_laserLine.SetLaserLineTo(Random.insideUnitCircle.normalized * 3f, null);
    }

    private void Update()
    {
        if (m_canThink && !m_hasShowLasers)
        {
            StartCoroutine(ShowLaserCoroutine());
            m_hasShowLasers = true;
        }
    }

    private IEnumerator ShowLaserCoroutine()
    {
        for (int i = 0; i < m_numberBounces; i++)
        {
            m_laserLine.SetLaserLineTo(Random.insideUnitCircle.normalized * 3f, () =>
            {
                m_canShowNext = true;
            });
            yield return new WaitUntil(()=> m_canShowNext);
            yield return new WaitForSeconds(0.65f);
            m_canShowNext = false;
        }
    }
}
