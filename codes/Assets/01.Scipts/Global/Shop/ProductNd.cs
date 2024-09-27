using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductNd : MonoBehaviour
{
    [HideInInspector] public Item m_Item;

    public Image m_ItemIcon;
    public Text m_ItemPrice;
    public Button m_BuyButton;

    public void SetImg(Item _item)
    {
        m_Item = _item;
        m_ItemIcon.sprite = m_Item.ItemImg;
        m_ItemPrice.text = m_Item.Price.ToString();
    }

    public Item BuyItem()
    {
        return m_Item;
    }
}
