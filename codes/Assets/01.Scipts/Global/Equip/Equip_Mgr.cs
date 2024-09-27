using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equip_Mgr : MonoBehaviour
{
    public GameObject Equip_Panel;
    public E_Slot[] E_Slots;

    #region Singleton
    public static Equip_Mgr Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    public void EquipItem(Slot slot)
    {
        if (slot.itemID == 4)
        {
            Inven_Mgr.Inst.OnSlotClick(slot);
            return;
        }

        foreach (var eSlot in E_Slots)
        {
            if (eSlot.itemID == -1)
            {
                eSlot.EquipItem(slot.itemID, slot.m_Icon.sprite);
                slot.ClearSlot();
                break;
            }
        }
    }

    public void UnequipItem(E_Slot eSlot)
    {
        if (eSlot.itemID != -1)
        {
            Inven_Mgr.Inst.AddItemToInventory(eSlot.itemID);
            eSlot.UnequipItem();
        }
    }
}