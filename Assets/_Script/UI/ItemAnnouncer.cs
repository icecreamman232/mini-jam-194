using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ItemAnnouncer : MonoBehaviour
{
    [SerializeField] private RectTransform m_itemAnnouncerGO;
    [SerializeField] private TextMeshProUGUI m_itemName;
    [SerializeField] private TextMeshProUGUI m_itemDesc;

    private void Awake()
    {
        m_itemAnnouncerGO.gameObject.SetActive(false);
    }

    public void Show(ItemData itemData)
    {
        var anchorPos = m_itemAnnouncerGO.anchoredPosition;
        anchorPos.x = -150;
        m_itemAnnouncerGO.anchoredPosition = anchorPos;
        
        m_itemName.text = itemData.Name;
        m_itemDesc.text = itemData.Description;
        m_itemAnnouncerGO.gameObject.SetActive(true);

        m_itemAnnouncerGO.DOAnchorPosX(0, 0.3f)
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                StartCoroutine(AfterShow());
            });
    }

    private IEnumerator AfterShow()
    {
        yield return new WaitForSeconds(1f);
        m_itemAnnouncerGO.gameObject.SetActive(false);
    }
}
