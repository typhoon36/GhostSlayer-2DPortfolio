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
    }

    public bool AddItem(int itemID)
    {
        if (m_ItemIDs.Count < m_SlotCnt)
        {
            m_ItemIDs.Add(itemID);
            D_OnChangeItem?.Invoke();
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
        }
    }
}
