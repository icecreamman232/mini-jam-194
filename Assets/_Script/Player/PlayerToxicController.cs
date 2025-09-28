using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ToxicModifierID
{
    SpeedReduce,
    RandomTeleport,
    COUNT
}

public class PlayerToxicController : MonoBehaviour
{
    [SerializeField] private ToxicDebuffEvent m_debuffEvent;
    [SerializeField] private ToxicDebuffContainer m_debuffContainer;
    [SerializeField] private PlayerCollectToxicEvent m_collectToxicEvent;
    [SerializeField] private int m_toxicLevel = 1;
    [SerializeField] private float m_currentToxic;
    [SerializeField] private float m_maxToxic;

    private PlayerController m_controller;
    private ToxicEventData m_toxicEventData = new ToxicEventData();

    private Dictionary<ToxicModifierID, ToxicModifier> m_toxicModifier =
        new Dictionary<ToxicModifierID, ToxicModifier>();

    private void Start()
    {
        m_controller = GetComponent<PlayerController>();
        m_toxicModifier.Add(ToxicModifierID.SpeedReduce, new MovespeedToxicModifier());
        m_toxicModifier.Add(ToxicModifierID.RandomTeleport, new RandomTeleportToxicModifier(GetComponent<PlayerTeleport>()));
    }

    public void AddToxic(float amount)
    {
        m_currentToxic += amount;
        UpdateUI();
        if (m_currentToxic >= m_maxToxic)
        {
            UpgradeToxicLevel();
            UpdateUI();
        }
    }

    private void UpgradeToxicLevel()
    {
        m_toxicLevel++;
        if (m_toxicLevel > (int)ToxicModifierID.COUNT)
        {
            m_toxicLevel = (int)ToxicModifierID.COUNT;
            return;
        }
        m_maxToxic += 3;
        m_currentToxic = 0;

        var modifier = GetRandomModifier(out var modifierID);
        modifier.Apply(m_controller);
        m_debuffEvent.Raise(m_debuffContainer.ToxicDebuffDataList.FirstOrDefault(data=>data.ID == modifierID));
    }

    private ToxicModifier GetRandomModifier(out ToxicModifierID modifierID)
    {
        modifierID = (ToxicModifierID)Random.Range(0, (int)ToxicModifierID.COUNT);
        return m_toxicModifier[modifierID];
    }

    private void UpdateUI()
    {
        m_toxicEventData.ToxicLevel = m_toxicLevel;
        m_toxicEventData.CurrentToxic = m_currentToxic;
        m_toxicEventData.MaxToxic = m_maxToxic;
        m_collectToxicEvent.Raise(m_toxicEventData);
    }
}
