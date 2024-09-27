using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int itemID = -1;
    public Image m_Icon;

    public void SetItem(int itemID)
    {
        this.itemID = itemID;
        Item item = ItemDB.Inst.GetItemByID(itemID);
        if (item != null)
        {
            m_Icon.sprite = item.ItemImg;
            m_Icon.gameObject.SetActive(true);
        }
    }

    public void ClearSlot()
    {
        itemID = -1;
        m_Icon.sprite = null;
        m_Icon.gameObject.SetActive(false);
    }
}
