using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inven_Mgr : MonoBehaviour
{
    //# �κ��丮 â
    public GameObject m_InvenPanel;
    bool IsOpen = false;

    //# ����
    public Slot[] Slots;
    //# ������ �θ� ��ü
    public Transform m_SlotRoot;

    //# �κ��丮 �ʱ�ȭ
    public Button AllRemove_Btn;

    //# �κ��丮 â �ݱ�
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

        //# �κ��丮 �ʱ�ȭ ��ư
        AllRemove_Btn.onClick.AddListener(ClearAllSlots);

        //## �κ��丮 �ݱ�
        if (Close_Btn != null)
            Close_Btn.onClick.AddListener(() => m_InvenPanel.SetActive(false));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            IsOpen = !IsOpen;
            m_InvenPanel.SetActive(IsOpen);
            if (IsOpen)
            {
                UpdateSlots(); // �κ��丮 â�� ���� �� ���� ������Ʈ
            }
        }
    }

    void SlotCntChange(int Val)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (i < Val)
            {
                Slots[i].GetComponent<Button>().interactable = true;
                int idx = i; // �͸� �޼��忡�� �ε����� ĸó�ϴ� ���� �ذ�
                Slots[i].GetComponent<Button>().onClick.RemoveAllListeners(); // ���� ������ ����
                Slots[i].GetComponent<Button>().onClick.AddListener(() => Equip_Mgr.Inst.EquipItem(Slots[idx]));
            }
            else if (i == 4)
            {
                int idx = i; // �͸� �޼��忡�� �ε����� ĸó�ϴ� ���� �ذ�
                Slots[i].GetComponent<Button>().onClick.RemoveAllListeners(); // ���� ������ ����
                Slots[i].GetComponent<Button>().onClick.AddListener(() => OnSlotClick(Slots[idx]));
            }
            else
            {
                Slots[i].GetComponent<Button>().interactable = false;
                Slots[i].ClearSlot(); // ���� ��Ȱ��ȭ �� �ʱ�ȭ
            }
        }
    }

    public void UpdateSlots()
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
                m_Inven.AddItem(itemID); // �κ��丮�� ������ �߰�
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
        PlayerPrefs.Save(); // ����� ������ �����մϴ�.
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
        int totalGold = 0;

        foreach (var itemID in m_Inven.m_ItemIDs)
        {
            Item item = ItemDB.Inst.GetItemByID(itemID);
            if (item != null)
            {
                totalGold += item.Price / 2; // ������ �Ǹ� �� ���� �������� ���
            }
        }

        m_Inven.m_ItemIDs.Clear();
        foreach (var slot in Slots)
        {
            slot.ClearSlot();
        }
        SaveInventory();

        // ��� �߰�
        Game_Mgr.Inst.AddGold(totalGold);
    }

    public void OnSlotClick(Slot slot)
    {
        if (slot.itemID == 4)
        {
            // Ư�� ������ ���
            Item item = ItemDB.Inst.GetItemByID(slot.itemID);
            if (item != null && item.Use())
            {
                // �κ��丮���� ������ ����
                m_Inven.RemoveItem(slot.itemID);
                slot.ClearSlot();

                // Ư�� ��ġ�� �̵�
                Transform a_PlyTransform = GameObject.FindWithTag("Player").transform;
                a_PlyTransform.position = new Vector3(-70, -23.1f, 0); // ���ϴ� ��ġ�� �̵�
            }
        }
    }
}
