using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bush_Ctrl : MonoBehaviour
{
    #region Enum
    public enum State { Idle, Trace, Attack }
    public State m_State = State.Idle;
    #endregion

    #region HP
    [Header("HP")]
    public Image m_HPBar;
    float m_MaxHP = 50.0f;
    float m_CurHP = 50.0f;
    #endregion

    #region #Trace
  
    private Transform monsterTr;
    private Transform playerTr;

    public float TraceDist = 10.0f;
    
    public float AttackDist = 1.5f;
    #endregion

    #region Global
    public bool IsDead = false; // 접근 수준을 public으로 변경
    Rigidbody2D m_Rd;
    Animator m_Anim;
    SpriteRenderer m_Sprite;
    #endregion

    void Awake()
    {
        
        TraceDist = 10.0f;
        AttackDist = 5.0f;

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

                   
                    if (playerTr.position.x > transform.position.x)
                        m_Sprite.flipX = false;
                    else
                        m_Sprite.flipX = true;

                }
                break;
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
        if (m_CurHP < 0.0f)
            return;

        float a_CacDmg = Dmg;
        if (m_CurHP < Dmg)
            a_CacDmg = m_CurHP;

        m_CurHP -= Dmg;

        if (m_HPBar != null)
            m_HPBar.fillAmount = m_CurHP / m_MaxHP;

        if (m_CurHP <= 0)
        {
            m_Anim.SetTrigger("IsDie");
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
