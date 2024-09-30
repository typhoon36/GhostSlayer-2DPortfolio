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

        //## 아이템 리스트
        List<Item> itemsForSale = ItemDB.Inst.m_ItemDB;

        //## 상품 생성
        foreach (var item in itemsForSale)
        {
            GameObject productObj = Instantiate(Product_Prefab, Product_Prant.transform);
            ProductNd product = productObj.GetComponent<ProductNd>();
            product.SetImg(item);
            product.m_BuyButton.onClick.AddListener(() => BuyProduct(product));
        }

        //## 상품 리스트에 상품 추가
        m_Products = Product_Prant.GetComponentsInChildren<ProductNd>();
    }

    //## 구매
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

            m_ContentTxt.text = "고블린 상인 : 좋은 선택이다.유령.";
            StartCoroutine(ResetContentTxt());
        }
        else
        {
            m_ContentTxt.text = "고블린 상인 : 너,돈이 부족하다..거래 안한다..";
        }
    }

    IEnumerator ResetContentTxt()
    {
        yield return new WaitForSeconds(4f);
        m_ContentTxt.text = "고블린 상인 : 히히..없는거 없다.만물상이다.";
    }
}
