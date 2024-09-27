using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool_Mgr : MonoBehaviour
{
    [Header("------- Bullet Pool -------")]
    public GameObject AL_BulletPrefab;  //아군 총알 프리팹을 연결할 변수
    public GameObject En_BulletPrefab;  //적군 총알 프리팹을 연결할 변수
    public GameObject En_BulletPrefab2;  //2번째 총알
    //아군 총알을 미리 생성해 저장할 리스트 자료형
    [HideInInspector] public List<Bullet_Ctrl> m_AllyBulletPool = new List<Bullet_Ctrl>();
    //적군 총알을 미리 생성해 저장할 리스트 자료형
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
        //총알을 생성해 오브젝트 풀에 저장
        for (int i = 0; i < 200; i++)
        {
            //총알 프리팹을 생성
            GameObject a_Bullet = (GameObject)Instantiate(AL_BulletPrefab);
            //생성한 총알을 Bullet_Mgr 밑으로 차일드화 하기
            a_Bullet.transform.SetParent(this.transform);
            //생성한 총알을 비활성화
            a_Bullet.SetActive(false);
            //생성한 총알을 오브젝트 풀에 추가
            m_AllyBulletPool.Add(a_Bullet.GetComponent<Bullet_Ctrl>());
        }
        //--- Ally Bullet Pool

        //--- Enemy Bullet Pool
        //총알을 생성해 오브젝트 풀에 저장
        for (int i = 0; i < 120; i++)
        {
            //총알 프리팹을 생성
            GameObject a_Bullet = (GameObject)Instantiate(En_BulletPrefab);
            //생성한 총알을 Bullet_Mgr 밑으로 차일드화 하기
            a_Bullet.transform.SetParent(this.transform);
            //생성한 총알을 비활성화
            a_Bullet.SetActive(false);
            //생성한 총알을 오브젝트 풀에 추가
            m_EnBulletPool.Add(a_Bullet.GetComponent<Bullet_Ctrl>());
        }
        //--- Enemy Bullet Pool

        //--- Enemy2 Bullet Pool
        //총알을 생성해 오브젝트 풀에 저장
        for (int i = 0; i < 120; i++)
        {
            //총알 프리팹을 생성
            GameObject a_Bullet = (GameObject)Instantiate(En_BulletPrefab2);
            //생성한 총알을 Bullet_Mgr 밑으로 차일드화 하기
            a_Bullet.transform.SetParent(this.transform);
            //생성한 총알을 비활성화
            a_Bullet.SetActive(false);
            //생성한 총알을 오브젝트 풀에 추가
            m_EnBulletPool2.Add(a_Bullet.GetComponent<Bullet_Ctrl>());
        }

    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public Bullet_Ctrl GetALBulletPool()
    {
        //오브젝트 풀의 처음부터 끝까지 순회
        foreach (Bullet_Ctrl a_BNode in m_AllyBulletPool)
        {
            //비활성화 여부로 사용 가능한 Bullet을 판단
            if (a_BNode.gameObject.activeSelf == false)
            {
                return a_BNode;
            }
        }

        //대기하고 있는 총알이 하나도 없으면 이쪽으로 넘어오게 된다.
        //그럴 경우 총알을 새로 하나 더 추가로 만들어 준다.

        //총알 프리팹을 생성
        GameObject a_Bullet = (GameObject)Instantiate(AL_BulletPrefab);
        //생성한 총알을 this.transform 밑으로 차일드화 하기
        a_Bullet.transform.SetParent(this.transform);
        //생성한 총알을 비활성화
        a_Bullet.SetActive(false);
        //생성한 총알의 BulletCtrl 컴포넌트 찾아오기
        Bullet_Ctrl a_BCtrl = a_Bullet.GetComponent<Bullet_Ctrl>();
        //생성한 총알을 오브젝트 풀에 추가
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
