using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class Market_Mgr : MonoBehaviour
{
    [Header("Market")]
    public GameObject Market_Panel;
    public GameObject Product_Prant;
    public GameObject Product_Prefab;
    public Text m_ContentTxt;

    public Button m_ExitBtn;

    ProductNd[] m_Products;

    #region Singleton
    public static Market_Mgr Inst;
    private void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        m_ExitBtn.onClick.AddListener(() => Market_Panel.SetActive(false));

        //## ������ ����Ʈ
        List<Item> itemsForSale = ItemDB.Inst.m_ItemDB;

        //## ��ǰ ����
        foreach (var item in itemsForSale)
        {
            GameObject productObj = Instantiate(Product_Prefab, Product_Prant.transform);
            ProductNd product = productObj.GetComponent<ProductNd>();
            product.SetImg(item);
            product.m_BuyButton.onClick.AddListener(() => BuyProduct(product));
        }

        //## ��ǰ ����Ʈ�� ��ǰ �߰�
        m_Products = Product_Prant.GetComponentsInChildren<ProductNd>();
    }

    //## ����
    void BuyProduct(ProductNd product)
    {
        int itemID = product.BuyItem().ItemID;
        int price = product.BuyItem().Price;

        if (GlobalValue.HasEnoughGold(price))
        {
            if (Inven.Inst.AddItem(itemID))
            {
                GlobalValue.g_UserGold -= price;
            }

            m_ContentTxt.text = "��� ���� : ���� �����̴�.����.";
            StartCoroutine(ResetContentTxt());
        }
        else
        {
            m_ContentTxt.text = "��� ���� : ��,���� �����ϴ�..�ŷ� ���Ѵ�..";
        }
    }

    IEnumerator ResetContentTxt()
    {
        yield return new WaitForSeconds(4f);
        m_ContentTxt.text = "��� ���� : ����..���°� ����.�������̴�.";
    }
}
