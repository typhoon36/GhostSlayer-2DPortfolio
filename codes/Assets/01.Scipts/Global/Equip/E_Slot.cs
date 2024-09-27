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

    public void EquipItem(int itemID, Sprite itemSprite)
    {
        this.itemID = itemID;
        m_Icon.sprite = itemSprite;
        m_Icon.gameObject.SetActive(true);
        UpdateInfoText();
    }

    public void UnequipItem()
    {
        itemID = -1;
        m_Icon.sprite = null;
        m_Icon.gameObject.SetActive(false);
        UpdateInfoText();
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
}
