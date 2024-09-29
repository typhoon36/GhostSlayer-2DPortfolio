using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//아이템추가와 저장 클래스
public class Inven : MonoBehaviour
{
    #region Singleton
    public static Inven Inst;

    void Awake()
    {
        if (Inst != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Inst = this;
    }
    #endregion

    public delegate void OnChangeSlotCnt(int Val);
    public OnChangeSlotCnt D_OnSlotCntChange;

    public delegate void OnChangeItem();
    public OnChangeItem D_OnChangeItem;

    private int slotCnt;

    public int m_SlotCnt
    {
        get => slotCnt;
        set
        {
            slotCnt = value;
            D_OnSlotCntChange?.Invoke(slotCnt);
        }
    }

    public List<int> m_ItemIDs = new List<int>();

    void Start()
    {
        m_SlotCnt = 9;
        LoadInventory();
    }

    public bool AddItem(int itemID)
    {
        if (m_ItemIDs.Count < m_SlotCnt)
        {
            m_ItemIDs.Add(itemID);
            D_OnChangeItem?.Invoke();
            SaveInventory();
            return true;
        }
        return false;
    }

    public void RemoveItem(int itemID)
    {
        if (m_ItemIDs.Contains(itemID))
        {
            m_ItemIDs.Remove(itemID);
            D_OnChangeItem?.Invoke();
            SaveInventory();
        }
    }

    void SaveInventory()
    {
        PlayerPrefs.SetInt("SlotCount", m_SlotCnt);
        for (int i = 0; i < m_ItemIDs.Count; i++)
        {
            PlayerPrefs.SetInt("ItemID_" + i, m_ItemIDs[i]);
        }
        PlayerPrefs.SetInt("ItemCount", m_ItemIDs.Count);
        PlayerPrefs.Save(); // 데이터 저장을 강제합니다.
    }

    void LoadInventory()
    {
        m_SlotCnt = PlayerPrefs.GetInt("SlotCount", 9);
        int itemCount = PlayerPrefs.GetInt("ItemCount", 0);
        m_ItemIDs.Clear();
        for (int i = 0; i < itemCount; i++)
        {
            int itemID = PlayerPrefs.GetInt("ItemID_" + i, -1);
            if (itemID != -1)
            {
                m_ItemIDs.Add(itemID);
            }
        }
        D_OnChangeItem?.Invoke();
    }
}
