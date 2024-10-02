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

    void Start()
    {
        LoadEquipments();
    }

    void Update()
    {
        //## 장비창 열기
        if (Input.GetKeyDown(KeyCode.E))
        {
            Equip_Panel.SetActive(!Equip_Panel.activeSelf);
            if (Equip_Panel.activeSelf)
            {
                UpdateEquipments();
            }
        }
    }

    public void EquipItem(Slot slot)
    {
        if (slot.itemID == 4)
        {
            Inven_Mgr.Inst.OnSlotClick(slot);
            return;
        }

        if (slot.itemID == 5)
        {
            return;
        }

        foreach (var eSlot in E_Slots)
        {
            if (eSlot.itemID == -1)
            {
                Item item = ItemDB.Inst.GetItemByID(slot.itemID);
                eSlot.EquipItem(slot.itemID, item.ItemImg);
                Inven.Inst.RemoveItem(slot.itemID);
                slot.ClearSlot();
                Inven_Mgr.Inst.UpdateSlots();
                SaveEquipments();
                break;
            }
        }
    }

    public void UnequipItem(E_Slot a_ESlot)
    {
        if (a_ESlot.itemID != -1)
        {
            Inven_Mgr.Inst.AddItemToInventory(a_ESlot.itemID);
            a_ESlot.UnequipItem();
            Inven_Mgr.Inst.UpdateSlots();
            SaveEquipments();
        }
    }

    void SaveEquipments()
    {
        for (int i = 0; i < E_Slots.Length; i++)
        {
            PlayerPrefs.SetInt("EquipItemID_" + i, E_Slots[i].itemID);
        }
        PlayerPrefs.Save();
    }

    void LoadEquipments()
    {
        for (int i = 0; i < E_Slots.Length; i++)
        {
            int itemID = PlayerPrefs.GetInt("EquipItemID_" + i, -1);
            if (itemID != -1)
            {
                Item item = ItemDB.Inst.GetItemByID(itemID);
                E_Slots[i].EquipItem(itemID, item.ItemImg);
            }
        }
    }

    void UpdateEquipments()
    {
        for (int i = 0; i < E_Slots.Length; i++)
        {
            int itemID = PlayerPrefs.GetInt("EquipItemID_" + i, -1);
            if (itemID != -1)
            {
                Item item = ItemDB.Inst.GetItemByID(itemID);
                E_Slots[i].EquipItem(itemID, item.ItemImg);
            }
            else
            {
                E_Slots[i].UnequipItem();
            }
        }
    }
}
