using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inven_Mgr : MonoBehaviour
{
    //# 인벤토리 창
    public GameObject m_InvenPanel;
    bool IsOpen = false;

    //# 슬롯
    public Slot[] Slots;
    //# 슬롯의 부모객체
    public Transform m_SlotRoot;

    //# 인벤토리 초기화
    public Button AllRemove_Btn;

    //# 키보드없이 인벤토리 창 닫기
    public Button Close_Btn;

    Inven m_Inven;

    #region Singleton
    public static Inven_Mgr Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        m_Inven = Inven.Inst;
        Slots = m_SlotRoot.GetComponentsInChildren<Slot>();
        m_Inven.D_OnSlotCntChange += SlotCntChange;
        m_Inven.D_OnChangeItem += UpdateSlots;
        m_InvenPanel.SetActive(IsOpen);
        LoadInventory();

        //# 인벤토리 초기화 버튼
        AllRemove_Btn.onClick.AddListener(ClearAllSlots);

        //## 인벤토리 꺼주기
        if (Close_Btn != null)
            Close_Btn.onClick.AddListener(() => m_InvenPanel.SetActive(false));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            IsOpen = !IsOpen;
            m_InvenPanel.SetActive(IsOpen);
        }
    }

    void SlotCntChange(int Val)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (i < Val)
            {
                Slots[i].GetComponent<Button>().interactable = true;
                int idx = i; // 현재 인덱스를 저장하는 별도의 변수
                Slots[i].GetComponent<Button>().onClick.RemoveAllListeners(); // 기존 리스너 제거
                Slots[i].GetComponent<Button>().onClick.AddListener(() => Equip_Mgr.Inst.EquipItem(Slots[idx]));
            }
            else
            {
                Slots[i].GetComponent<Button>().interactable = false;
                Slots[i].ClearSlot(); // 슬롯 비활성화 시 슬롯 초기화
            }
        }
    }

    void UpdateSlots()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (i < m_Inven.m_ItemIDs.Count)
            {
                Slots[i].SetItem(m_Inven.m_ItemIDs[i]);
            }
            else
            {
                Slots[i].ClearSlot();
            }
        }
        SaveInventory();
    }

    public void AddSlot()
    {
        m_Inven.m_SlotCnt++;
    }

    public void AddItemToInventory(int itemID)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].itemID == -1)
            {
                Slots[i].SetItem(itemID);
                break;
            }
        }
    }

    void SaveInventory()
    {
        PlayerPrefs.SetInt("SlotCount", m_Inven.m_SlotCnt);
        for (int i = 0; i < m_Inven.m_ItemIDs.Count; i++)
        {
            PlayerPrefs.SetInt("ItemID_" + i, m_Inven.m_ItemIDs[i]);
        }
        PlayerPrefs.SetInt("ItemCount", m_Inven.m_ItemIDs.Count);
        PlayerPrefs.Save(); // 데이터 저장을 강제합니다.
    }

    void LoadInventory()
    {
        m_Inven.m_SlotCnt = PlayerPrefs.GetInt("SlotCount", 9);
        int itemCount = PlayerPrefs.GetInt("ItemCount", 0);
        m_Inven.m_ItemIDs.Clear(); 
        for (int i = 0; i < itemCount; i++)
        {
            int itemID = PlayerPrefs.GetInt("ItemID_" + i, -1);
            if (itemID != -1)
            {
                m_Inven.m_ItemIDs.Add(itemID);
            }
        }
        UpdateSlots();
    }

    void ClearAllSlots()
    {
        m_Inven.m_ItemIDs.Clear();
        foreach (var slot in Slots)
        {
            slot.ClearSlot();
        }
        SaveInventory();
    }
}
