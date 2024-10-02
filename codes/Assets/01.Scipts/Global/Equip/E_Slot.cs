using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class E_Slot : MonoBehaviour, IPointerClickHandler
{
    #region Singleton
    public static E_Slot Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    public Image m_Icon;
    public int itemID = -1;
    Coroutine m_recoverHPCo;

    public void EquipItem(int itemID, Sprite itemSprite)
    {
        this.itemID = itemID;
        m_Icon.sprite = itemSprite;
        m_Icon.gameObject.SetActive(true);
        UpdateInfoText();

        // 아이템 장착 시 HP 회복 시작
        if (itemID == 6)
        {
            Game_Mgr.Inst.StartRecoverHP();
        }
    }

    public void UnequipItem()
    {
        itemID = -1;
        m_Icon.sprite = null;
        m_Icon.gameObject.SetActive(false);
        UpdateInfoText();

        // 아이템 해제 시 HP 회복 중지
        Game_Mgr.Inst.StopRecoverHP();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Equip_Mgr.Inst.UnequipItem(this);
        }
    }

    public void UpdateInfoText()
    {
        Game_Mgr.Inst.UpdateAllSlotsInfo();
    }

    public void StartRecoverHP()
    {
        if (m_recoverHPCo != null)
        {
            StopCoroutine(m_recoverHPCo);
        }
        m_recoverHPCo = StartCoroutine(RecoverHPOverTime());
    }

    public void StopRecoverHP()
    {
        if (m_recoverHPCo != null)
        {
            StopCoroutine(m_recoverHPCo);
            m_recoverHPCo = null;
        }
    }

    private IEnumerator RecoverHPOverTime()
    {
        while (true)
        {
            Game_Mgr.Inst.RecoverHP(0.01f); // HP를 서서히 회복
            yield return new WaitForSeconds(1f); // 1초마다 회복
        }
    }
}
