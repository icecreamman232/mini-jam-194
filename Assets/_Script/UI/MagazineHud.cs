using System;
using TMPro;
using UnityEngine;

public class MagazineHud : MonoBehaviour
{
    [SerializeField] private PlayerMagazineEvent m_playerMagazineEvent;
    [SerializeField] private TextMeshProUGUI m_magazineText;

    private void Awake()
    {
        m_playerMagazineEvent.AddListener(OnUpdateMagazineHud);
    }

    private void OnDestroy()
    {
        m_playerMagazineEvent.RemoveListener(OnUpdateMagazineHud);
    }

    private void OnUpdateMagazineHud(int currentMagazine)
    {
        m_magazineText.text = $"x {currentMagazine}";
    }
}
