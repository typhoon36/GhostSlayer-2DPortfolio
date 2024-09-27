using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool_Mgr : MonoBehaviour
{
    [Header("------- Bullet Pool -------")]
    public GameObject AL_BulletPrefab;  //�Ʊ� �Ѿ� �������� ������ ����
    public GameObject En_BulletPrefab;  //���� �Ѿ� �������� ������ ����
    public GameObject En_BulletPrefab2;  //2��° �Ѿ�
    //�Ʊ� �Ѿ��� �̸� ������ ������ ����Ʈ �ڷ���
    [HideInInspector] public List<Bullet_Ctrl> m_AllyBulletPool = new List<Bullet_Ctrl>();
    //���� �Ѿ��� �̸� ������ ������ ����Ʈ �ڷ���
    [HideInInspector] public List<Bullet_Ctrl> m_EnBulletPool = new List<Bullet_Ctrl>();
    [HideInInspector] public List<Bullet_Ctrl> m_EnBulletPool2 = new List<Bullet_Ctrl>();

    #region Singleton
    public static BulletPool_Mgr Inst = null;

    private void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        //--- Ally Bullet Pool
        //�Ѿ��� ������ ������Ʈ Ǯ�� ����
        for (int i = 0; i < 200; i++)
        {
            //�Ѿ� �������� ����
            GameObject a_Bullet = (GameObject)Instantiate(AL_BulletPrefab);
            //������ �Ѿ��� Bullet_Mgr ������ ���ϵ�ȭ �ϱ�
            a_Bullet.transform.SetParent(this.transform);
            //������ �Ѿ��� ��Ȱ��ȭ
            a_Bullet.SetActive(false);
            //������ �Ѿ��� ������Ʈ Ǯ�� �߰�
            m_AllyBulletPool.Add(a_Bullet.GetComponent<Bullet_Ctrl>());
        }
        //--- Ally Bullet Pool

        //--- Enemy Bullet Pool
        //�Ѿ��� ������ ������Ʈ Ǯ�� ����
        for (int i = 0; i < 120; i++)
        {
            //�Ѿ� �������� ����
            GameObject a_Bullet = (GameObject)Instantiate(En_BulletPrefab);
            //������ �Ѿ��� Bullet_Mgr ������ ���ϵ�ȭ �ϱ�
            a_Bullet.transform.SetParent(this.transform);
            //������ �Ѿ��� ��Ȱ��ȭ
            a_Bullet.SetActive(false);
            //������ �Ѿ��� ������Ʈ Ǯ�� �߰�
            m_EnBulletPool.Add(a_Bullet.GetComponent<Bullet_Ctrl>());
        }
        //--- Enemy Bullet Pool

        //--- Enemy2 Bullet Pool
        //�Ѿ��� ������ ������Ʈ Ǯ�� ����
        for (int i = 0; i < 120; i++)
        {
            //�Ѿ� �������� ����
            GameObject a_Bullet = (GameObject)Instantiate(En_BulletPrefab2);
            //������ �Ѿ��� Bullet_Mgr ������ ���ϵ�ȭ �ϱ�
            a_Bullet.transform.SetParent(this.transform);
            //������ �Ѿ��� ��Ȱ��ȭ
            a_Bullet.SetActive(false);
            //������ �Ѿ��� ������Ʈ Ǯ�� �߰�
            m_EnBulletPool2.Add(a_Bullet.GetComponent<Bullet_Ctrl>());
        }

    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public Bullet_Ctrl GetALBulletPool()
    {
        //������Ʈ Ǯ�� ó������ ������ ��ȸ
        foreach (Bullet_Ctrl a_BNode in m_AllyBulletPool)
        {
            //��Ȱ��ȭ ���η� ��� ������ Bullet�� �Ǵ�
            if (a_BNode.gameObject.activeSelf == false)
            {
                return a_BNode;
            }
        }

        //����ϰ� �ִ� �Ѿ��� �ϳ��� ������ �������� �Ѿ���� �ȴ�.
        //�׷� ��� �Ѿ��� ���� �ϳ� �� �߰��� ����� �ش�.

        //�Ѿ� �������� ����
        GameObject a_Bullet = (GameObject)Instantiate(AL_BulletPrefab);
        //������ �Ѿ��� this.transform ������ ���ϵ�ȭ �ϱ�
        a_Bullet.transform.SetParent(this.transform);
        //������ �Ѿ��� ��Ȱ��ȭ
        a_Bullet.SetActive(false);
        //������ �Ѿ��� BulletCtrl ������Ʈ ã�ƿ���
        Bullet_Ctrl a_BCtrl = a_Bullet.GetComponent<Bullet_Ctrl>();
        //������ �Ѿ��� ������Ʈ Ǯ�� �߰�
        m_AllyBulletPool.Add(a_BCtrl);

        return a_BCtrl;
    }

    public Bullet_Ctrl GetEnBulletPool()
    {
        if (En_BulletPrefab == null)
        {
            return null;
        }

        foreach (Bullet_Ctrl a_BNode in m_EnBulletPool)
        {
            if (a_BNode != null && !a_BNode.gameObject.activeSelf)
            {
                return a_BNode;
            }
        }

        GameObject a_Bullet = Instantiate(En_BulletPrefab);
        a_Bullet.transform.SetParent(this.transform);
        a_Bullet.SetActive(false);
        Bullet_Ctrl a_BCtrl = a_Bullet.GetComponent<Bullet_Ctrl>();
        m_EnBulletPool.Add(a_BCtrl);

        return a_BCtrl;
    }

    public Bullet_Ctrl GetEn2BulletPool()
    {
        if (En_BulletPrefab2 == null)
        {
            return null;
        }

        foreach (Bullet_Ctrl a_BNode in m_EnBulletPool2)
        {
            if (a_BNode != null && !a_BNode.gameObject.activeSelf)
            {
                return a_BNode;
            }
        }

        GameObject a_Bullet = Instantiate(En_BulletPrefab2);
        a_Bullet.transform.SetParent(this.transform);
        a_Bullet.SetActive(false);
        Bullet_Ctrl a_BCtrl = a_Bullet.GetComponent<Bullet_Ctrl>();
        m_EnBulletPool2.Add(a_BCtrl);

        return a_BCtrl;
    }


}
