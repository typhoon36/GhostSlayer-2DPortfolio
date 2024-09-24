using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  <#아이템클래스>
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

    public bool Use()
    {
        return false;
    }

    public bool Equip()
    {
        return false;
    }
}