using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : MonoBehaviour
{
    #region Singleton
    public static ItemDB Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    public List<Item> m_ItemDB = new List<Item>();

    public Item GetItemByID(int itemID)
    {
        return m_ItemDB.Find(i => i.ItemID == itemID);
    }
}