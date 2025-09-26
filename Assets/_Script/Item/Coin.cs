using System.Collections;
using SGGames.Script.Core;
using UnityEngine;

public class Coin : PickupBehavior, IPickup
{
    [SerializeField] private SpriteRenderer m_model;
    [SerializeField] private float m_delayBeforeSelfDestruction;

    private Coroutine m_flickerCoroutine;
    private Coroutine m_selfDestructionCoroutine;
    
    private void Start()
    {
        m_selfDestructionCoroutine = StartCoroutine(SelfDestruction());
    }

    private IEnumerator SelfDestruction()
    {
        var timeStart = Time.time;
        var timeStop = timeStart + m_delayBeforeSelfDestruction;
        bool triggerFlickerFx = false;
        while(Time.time < timeStop)
        {
            if (!triggerFlickerFx)
            {
                var timeElapsed = Time.time - timeStart;
                if (timeElapsed >= m_delayBeforeSelfDestruction * 0.7f)
                {
                    m_flickerCoroutine = StartCoroutine(FlickerFx());
                    triggerFlickerFx = true;
                }
            }
            yield return null;
        }
        this.gameObject.SetActive(false);
    }

    private IEnumerator FlickerFx()
    {
        var originalColor = m_model.color;
        var transparentColor = m_model.color;
        transparentColor.a = 0f;
        while (true)
        {
            m_model.color = transparentColor;
            yield return new WaitForSeconds(0.08f);
            m_model.color = originalColor;
            yield return new WaitForSeconds(0.08f);
            yield return null;
        }
    }

    public void Picking(Transform player)
    {
        if (m_flickerCoroutine != null)
        {
            StopCoroutine(m_flickerCoroutine);
            m_flickerCoroutine = null;
        }

        if (m_selfDestructionCoroutine != null)
        {
            StopCoroutine(m_selfDestructionCoroutine);
            m_selfDestructionCoroutine = null;
        }
        
        m_model.color = Color.white;
        MoveToPlayer(player, Pickup);
    }

    public void Pickup()
    {
        Debug.Log("Picked up 1 coin");
        ServiceLocator.GetService<CurrencyManager>().AddCoin(1);
        this.gameObject.SetActive(false);
    }
}
