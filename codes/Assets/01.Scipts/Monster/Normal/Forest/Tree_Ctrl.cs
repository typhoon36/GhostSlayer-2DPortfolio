using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tree_Ctrl : MonoBehaviour
{
    #region Enum
    public enum State { Idle, Bullet }
    public State m_State = State.Idle;
    #endregion

    #region HP
    [Header("HP")]
    public Image m_HPBar;
    float m_MaxHP = 40.0f;
    float m_CurHP = 40.0f;
    #endregion

    #region Attack
    [Header("Attack")]
    public float m_AttackDist = 8.0f;
    Transform m_PlayerTr;
    Transform m_MonsterTr;
    #endregion

    #region Bullet
    [Header("Bullet")]
    public GameObject m_Bullet;
    public Transform m_BulletPos;
    private float Shot_Time = 0.0f;
    private float BulletSpeed = 10.0f;
    private int BulletCount = 0;
    #endregion

    #region Global
    public bool IsDead = false; // 접근 수준을 public으로 변경
    SpriteRenderer m_Sprite;
    Animator m_Anim;
    Rigidbody2D m_Rd;
    #endregion

    void Awake()
    {
        m_AttackDist = 12.0f;
        m_CurHP = m_MaxHP;
        m_MonsterTr = this.gameObject.GetComponent<Transform>();
        m_PlayerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        m_Rd = GetComponent<Rigidbody2D>();
        m_Anim = GetComponent<Animator>();
        m_Sprite = GetComponent<SpriteRenderer>();
    }

    #region AI
    void FixedUpdate()
    {
        CheckMon();
        MonAction();
    }

    //#AI üũ
    float m_AI_Delay = 0.0f;
    void CheckMon()
    {
        if (IsDead) return;

        m_AI_Delay -= Time.deltaTime;

        if (0 < m_AI_Delay) return;

        m_AI_Delay = 0.1f;

        if (m_PlayerTr == null) return;

        float a_Dist = Vector3.Distance(m_PlayerTr.position, m_MonsterTr.position);

        if (a_Dist <= m_AttackDist)
        {
            m_State = State.Bullet;
        }
        else
        {
            m_State = State.Idle;
        }

        if (m_PlayerTr.position.x < m_MonsterTr.position.x)
            m_Sprite.flipX = true;
        else
            m_Sprite.flipX = false;
    }

    void MonAction()
    {
        if (IsDead) return;

        switch (m_State)
        {
            case State.Idle:
                m_Anim.SetBool("IsAttack", false);
                break;
            case State.Bullet:
                BulletUpdate();
                break;
        }
    }
    #endregion

    void BulletUpdate()
    {
        Shot_Time -= Time.deltaTime;

        if (Shot_Time <= 0)
        {
            Vector3 a_Target = m_PlayerTr.transform.position - m_BulletPos.transform.position;
            a_Target.z = 0.0f;
            a_Target.Normalize();

            Bullet_Ctrl a_BulletSc = BulletPool_Mgr.Inst.GetEnBulletPool();
            a_BulletSc.gameObject.SetActive(true);
            a_BulletSc.BulletSpawn(m_BulletPos.transform.position, a_Target, BulletSpeed);
            a_BulletSc.transform.right = new Vector3(-a_Target.x, -a_Target.y, 0.0f);

            BulletCount++;
            if (BulletCount < 7)
            {
                Shot_Time = 0.7f;
            }
            else
            {
                BulletCount = 0;
                Shot_Time = 2.0f;
                m_State = State.Idle;
            }
        }
    }

    #region Damage
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Punch" && coll.gameObject.activeSelf == true)
        {
            m_Anim.SetTrigger("IsHit");
            TakeDamage(10);
            Debug.Log(m_CurHP.ToString());
        }
        else if (coll.gameObject.tag == "Ally_Bullet")
        {
            m_Anim.SetTrigger("IsHit");
            TakeDamage(10);
        }
    }

    public void TakeDamage(float Dmg)
    {
        if (m_CurHP < 0.0f)
            return;

        float a_CacDmg = Dmg;
        if (m_CurHP < Dmg)
            a_CacDmg = m_CurHP;

        // 장착된 아이템의 데미지 수정자 반영
        if (Equip_Mgr.Inst != null)
        {
            foreach (var eSlot in Equip_Mgr.Inst.E_Slots)
            {
                if (eSlot.itemID != -1)
                {
                    if (eSlot.itemID == 0)
                    {
                        a_CacDmg = 10;
                    }
                    else if (eSlot.itemID == 3)
                    {
                        a_CacDmg = 20;
                    }
                    else
                    {
                        Item item = ItemDB.Inst?.GetItemByID(eSlot.itemID);
                        if (item != null)
                        {
                            a_CacDmg *= item.DamageModifier;
                        }
                    }
                }
            }
        }

        m_CurHP -= a_CacDmg;

        if (m_HPBar != null)
            m_HPBar.fillAmount = m_CurHP / m_MaxHP;

        if (m_CurHP <= 0)
        {
            m_Anim.SetTrigger("IsDie");
            this.gameObject.SetActive(false);
            Game_Mgr.Inst.AddGold(10);
        }
    }
    #endregion
}