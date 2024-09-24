using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slime_Ctrl : MonoBehaviour
{
    #region Enum
    public enum State { Idle, Move, Patrol }
    public State m_State = State.Idle;
    #endregion

    #region HP
    [Header("HP")]
    public Image m_HPBar;
    float m_MaxHP = 20.0f;
    float m_CurHP = 20.0f;
    #endregion

    #region #Trace
    private Transform monsterTr;
    private Transform playerTr;

    public float TraceDist = 10.0f;
    public float AttackDist = 1.5f;
    #endregion

    #region Global
    bool IsDead = false;
    Rigidbody2D m_Rd;
    Animator m_Anim;
    SpriteRenderer m_Sprite;
    #endregion

    void Awake()
    {
        TraceDist = 10.0f;
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

        float Dist = Vector2.Distance(transform.position, playerTr.position);

        if (Dist <= TraceDist)
        {
            m_State = State.Move;
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
                m_Anim.SetBool("IsMove", false);
                m_Anim.SetBool("IsAgro", false);
                break;

            case State.Move:
                {
                    float a_Velocity = 2.0f;
                    Vector2 a_Dir = playerTr.position - transform.position;
                    a_Dir.y = 0;

                    if (0 < a_Dir.magnitude)
                    {
                        if (a_Dir.magnitude <= AttackDist)
                        {
                            a_Velocity = 0.0f; // 플레이어와 겹치면 속도를 0으로 설정
                        }

                        Vector3 a_Vec = a_Dir.normalized * a_Velocity * Time.deltaTime;
                        transform.position += a_Vec;

                        if (a_Dir.x < 0)
                        {
                            m_Sprite.flipX = true;
                        }
                        else
                        {
                            m_Sprite.flipX = false;
                        }
                    }

                    m_Anim.SetBool("IsAgro", true);
                    m_Anim.SetBool("IsMove", true);
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


