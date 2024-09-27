using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Drone_Ctrl : MonoBehaviour
{
    public enum State { Idle, Trace, Attack }
    public State m_State = State.Idle;

    #region HP
    public Image m_HPBar;
    float m_MaxHP = 50.0f;
    float m_CurHP = 50.0f;
    #endregion

    #region #Trace
    private Transform monsterTr;
    private Transform playerTr;

    public float TraceDist = 10.0f;
    public float AttackDist = 2f;
    #endregion

    #region Bullet
    public GameObject m_Bullet;
    public Transform m_BulletPos;
    private float Shot_Time = 0.0f;
    private float BulletSpeed = 10.0f;
    private int BulletCount = 0;
    #endregion

    #region Global
    public bool IsDead = false;
    Rigidbody2D m_Rd;
    Animator m_Anim;
    SpriteRenderer m_Sprite;
    #endregion

    void Awake()
    {
        TraceDist = 10.0f;
        AttackDist = 5f;
        m_CurHP = m_MaxHP;

        monsterTr = this.gameObject.GetComponent<Transform>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) playerTr = playerObj.GetComponent<Transform>();

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
    float m_AI_Delay = 0.0f;
    void CheckMon()
    {
        if (IsDead) return;

        m_AI_Delay -= Time.deltaTime;

        if (0 < m_AI_Delay) return;

        m_AI_Delay = 0.1f;

        if (playerTr == null) return;

        float Dist = Vector2.Distance(transform.position,
            playerTr.position);

        if (Dist <= AttackDist)
        {
            m_State = State.Attack;
        }
        else if (Dist <= TraceDist)
        {
            m_State = State.Trace;
        }
        else
        {
            m_State = State.Idle;
        }
    }

    void MonAction()
    {
        if (IsDead) return;

        switch (m_State)
        {
            case State.Idle:
                m_Anim.SetBool("IsWalk", false);
                break;
            case State.Trace:
                {
                    float a_MoveVel = 2.0f;
                    Vector3 a_MoveDir = playerTr.position - transform.position;
                    a_MoveDir.y = 0.0f;

                    if (0 < a_MoveDir.magnitude)
                    {
                        Vector3 a_Vec = a_MoveDir.normalized * a_MoveVel * Time.deltaTime;
                        transform.Translate(a_Vec, Space.World);

                        // 방향 전환
                        if (a_MoveDir.x > 0)
                            m_Sprite.flipX = false;
                        else
                            m_Sprite.flipX = true;
                    }

                    m_Anim.SetBool("IsAttack", false);
                    m_Anim.SetBool("IsWalk", true);

                }
                break;
            case State.Attack:
                {
                    m_Anim.SetBool("IsAttack", true);

                    // 플레이어를 향하도록 방향 전환
                    if (playerTr.position.x > transform.position.x)
                        m_Sprite.flipX = false;
                    else
                        m_Sprite.flipX = true;

                    BulletUpdate();
                }
                break;
        }
    }
    #endregion

    #region Bullet
    void BulletUpdate()
    {
        Shot_Time -= Time.deltaTime;

        if (Shot_Time <= 0)
        {
            Vector3 a_Target =
                playerTr.transform.position - m_BulletPos.transform.position;

            a_Target.z = 0.0f;
            a_Target.Normalize();

            Bullet_Ctrl a_BulletSc = BulletPool_Mgr.Inst.GetEn2BulletPool();

            a_BulletSc.gameObject.SetActive(true);
            a_BulletSc.BulletSpawn(m_BulletPos.transform.position, a_Target, BulletSpeed);

            a_BulletSc.transform.right = new Vector3(-a_Target.x, -a_Target.y, 0.0f);

            BulletCount++;
            if (BulletCount < 2)
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
    #endregion

    #region Damage

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Punch" && coll.gameObject.activeSelf == true)
        {
            m_Anim.SetTrigger("IsHurt");
            TakeDamage(10);
        }
        else if (coll.gameObject.tag == "Ally_Bullet")
        {
            m_Anim.SetTrigger("IsHurt");
            TakeDamage(10);
        }
    }

    public void TakeDamage(float Dmg)
    {
        if (m_CurHP <= 0.0f)
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
                    Item item = ItemDB.Inst?.GetItemByID(eSlot.itemID);
                    if (item != null)
                    {
                        a_CacDmg *= item.DamageModifier;
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
            IsDead = true; // 드론이 죽었음을 표시
            StartCoroutine(DeathDel(1.2f));
        }
    }

    IEnumerator DeathDel(float a_Val)
    {
        yield return new WaitForSeconds(a_Val);
        gameObject.SetActive(false);
        Game_Mgr.Inst.AddGold(10);
    }
    #endregion
}
