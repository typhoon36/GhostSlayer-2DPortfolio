using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  <#������Ŭ����>
/// </summary>
public enum ItemType
{
    Equip,
    Use,
    Consumeable,
    ETC
}

[System.Serializable]
public class Item
{
    public int ItemID;
    public ItemType m_ItType;
    public string ItemName;
    public Sprite ItemImg;
    public int Price;
    public float DamageModifier;

    public bool Use()
    {
        if (ItemID == 4)
        {
            return true;
        }
        return false;
    }

    public bool Equip()
    {
        if (ItemID == 5)
        {
            Debug.Log("������ ID�� 5�� ��� ������ �� �����ϴ�.");
            return false;
        }
        else if (ItemID == 6)
        {
            Game_Mgr.Inst.RecoverHP(0.2f);
        }
        return true;
    }
}
