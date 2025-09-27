using System.Collections;
using UnityEngine;

public class PlayerBullet : Bullet
{
    [SerializeField] private Animator m_animator;
    
    private readonly int m_ExplodeAnimParam = Animator.StringToHash("Explode Trigger");
    private readonly float m_explodeAnimDuration = 0.45f;

    private void OnEnable()
    {
        m_animator.Play("Idle");
    }

    protected override void DestroyBullet()
    {
        StartCoroutine(OnBeforeDestroyingBullet());
    }

    private IEnumerator OnBeforeDestroyingBullet()
    {
        m_isActivated = false;
        m_animator.SetTrigger(m_ExplodeAnimParam);
        yield return new WaitForSeconds(m_explodeAnimDuration);
        base.DestroyBullet();
    }
}
