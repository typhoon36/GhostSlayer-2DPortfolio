using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nependas_Ctrl : MonoBehaviour
{
    #region #enum
    public enum State { Roar, Move, Bullet, Bite, Dead }

    public State m_State = State.Roar;
    #endregion

    #region #HP
    [Header("HP")]
    public GameObject m_HPGroup;
    public Image m_HPBar;
    float m_MaxHP = 1000.0f;
    float m_CurHP = 1000.0f;
    #endregion

    #region Roar
    [Header("RoarEff")]
    public GameObject m_RoarEff;
    private bool IsRoar = false;
    #endregion

    #region  ������ ����
    private float m_Speed = 4.0f;
    Vector3 m_CurPos;
    Vector3 m_DirPos;
    #endregion


    #region Bullet
    [Header("Bullet")]
    public GameObject m_Bullet;
    public Transform m_BulletPos;
    private float Shot_Time = 0.0f;
    private float BulletSpeed = 10.0f;
    private int BulletCount = 0;
    private float bulletPatternInterval = 5.0f; // �Ѿ� ���� ���� ����
    private float bulletPatternTimer = 0.0f;
    #endregion




    #region #Global 
    [Header("Global")]
    public RectTransform m_EffCanvas;
    public Animator m_Anim;
    public PlayerCtrl m_Player;
    public GameObject m_NextPortal;
    public CinemachineVirtualCamera m_Cinema;
    private CinemachineBasicMultiChannelPerlin m_ChanelPerlin;
    private SpriteRenderer m_Sprite;
    [HideInInspector] public bool IsDead = false;
    #endregion


    private void Start()
    {
        m_HPGroup.SetActive(false);

        m_MaxHP = m_CurHP;
        m_Sprite = GetComponentInChildren<SpriteRenderer>();
        m_Anim = GetComponentInChildren<Animator>();

        // ������ �� ���¸� Roar�� ����
        m_State = State.Roar;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, m_Player.transform.position);

        if (distanceToPlayer < 50.0f) // �÷��̾ ������ ������ ���� ��
        {
            m_HPGroup.gameObject.SetActive(true);
            Move();
        }
        else // �÷��̾ ������ �ָ� ���� ��
        {
            m_HPGroup.gameObject.SetActive(false);
            m_State = State.Roar; // ���� �ൿ�� ���߰� Roar ���·� ����
        }

        // ���� �ð����� Bullet ���� ����
        bulletPatternTimer += Time.deltaTime;
        if (bulletPatternTimer >= bulletPatternInterval)
        {
            bulletPatternTimer = 0.0f;
            m_State = State.Bullet;
        }
    }

    void FixedUpdate()
    {
        if (IsDead) return;

        switch (m_State)
        {
            case State.Roar:
                Roar();
                break;
            case State.Move:
                Move();
                break;
            case State.Bullet:
                Bullet();
                break;
            case State.Bite:
                Bite();
                break;
            case State.Dead:
                Dead();
                break;
        }

    }

    #region �⺻ �̵�
    void Move()
    {
        m_CurPos = transform.position;
        m_DirPos = Vector3.left;

        if (m_Player != null)
        {
            Vector3 a_CacVec = m_Player.transform.position - transform.position;
            m_DirPos = a_CacVec;

            // �÷��̾��� ��ġ�� ���� flipX ����
            if (m_Player.transform.position.x > transform.position.x)
                m_Sprite.flipX = true; // �÷��̾ �����ʿ� ���� ��
            else
                m_Sprite.flipX = false; // �÷��̾ ���ʿ� ���� ��
        }

        Vector3 a_CacPos = m_CurPos + m_DirPos.normalized * m_Speed * Time.deltaTime;
        a_CacPos.y = -21.86f; // y ���� ����
        transform.position = a_CacPos;
        m_Anim.SetBool("IsWalk", true);
    }
    #endregion

    #region ��ȿ -- ��Ʈ��
    void Roar()
    {
        if (!IsRoar && m_RoarEff != null && m_EffCanvas != null)
        {
            m_EffCanvas.transform.localPosition = Vector3.zero; // ĵ������ ��ġ�� ������ �߾����� ����

            m_RoarEff.SetActive(true); // ��ȿ ȿ�� ������Ʈ Ȱ��ȭ
            m_RoarEff.transform.localPosition = new Vector3(-243, 135, 0);

            IsRoar = true;
            m_Anim.SetBool("IsWalk", false);

            // ��鸲 ȿ�� ����
            StartCoroutine(ShakeCamera(0.5f, 2.0f));

        }
    }

    IEnumerator ShakeCamera(float duration, float amplitude)
    {
        if (m_ChanelPerlin != null)
        {
            m_ChanelPerlin.m_AmplitudeGain = amplitude;
            yield return new WaitForSeconds(duration);
            m_ChanelPerlin.m_AmplitudeGain = 0f;
        }
        yield return new WaitForSeconds(2.0f);

        IsRoar = false;
        m_RoarEff.SetActive(false); // ��ȿ ȿ�� ��Ȱ��ȭ

        // ��ȿ�� ������ Move ���·� ����
        m_State = State.Move;
    }
    #endregion

    #region �Ϲ� �Ѿ�
    void Bullet()
    {
        Shot_Time -= Time.deltaTime;
        if (Shot_Time < 0)
        {
            Vector3 a_Target = m_Player.transform.position - m_BulletPos.position;
            a_Target.z = 0;
            a_Target.Normalize();

            Bullet_Ctrl a_Bulletsc = BulletPool_Mgr.Inst.GetEnBulletPool();
            if (a_Bulletsc != null)
            {
                a_Bulletsc.gameObject.SetActive(true);
                a_Bulletsc.BulletSpawn(m_BulletPos.transform.position, a_Target, BulletSpeed);

                // ȸ��
                a_Bulletsc.transform.right = new Vector3(-a_Target.x, -a_Target.y, 0);

                BulletCount++;
                if (BulletCount < 7)
                {
                    m_Anim.SetBool("IsBullet", true);
                    Shot_Time = 0.7f;
                }
                else
                {
                    BulletCount = 0;
                    Shot_Time = 2.0f;
                    m_Anim.SetBool("IsBullet", false);
                    // �Ѿ� ������ ������ Move ���·� ����
                    m_State = State.Move;
                }
            }
        }
    }
    #endregion


    void Bite()
    {
        float a_Dist = Vector3.Distance(transform.position, m_Player.transform.position);

        if (a_Dist < 2.0f)
        {
            m_Anim.SetBool("IsBite", true);
        }
        else
        {
            m_State = State.Move;
        }
    }

    void Dead()
    {
        if (m_CurHP < 0)
        {
            IsDead = true;
            m_Anim.SetTrigger("IsDead");
            m_State = State.Dead;
            m_NextPortal.SetActive(true);
        }

    }


    #region �浹ó�� & �ǰ�
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Obstacle"))
        {
            if (m_Sprite != null)
            {
                // �浹�� ��ü�� �̸��� ���� flipX ����
                if (coll.gameObject.name == "Obstacle-1")
                    m_Sprite.flipX = false;
                else if (coll.gameObject.name == "Obstacle-2")
                    m_Sprite.flipX = true;
            }
        }
        else if (coll.gameObject.tag == "Punch" && coll.gameObject.activeSelf == true)
        {
            m_Anim.SetTrigger("IsHit");
            TakeDamage(10);
        }

        else if (coll.gameObject.tag == "Ally_Bullet")
        {
            m_Anim.SetTrigger("IsHit");
            TakeDamage(20);
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
            IsDead = true;
            m_Anim.SetTrigger("IsDead");
            Destroy(this.gameObject);
            m_HPGroup.SetActive(false);
            m_NextPortal.gameObject.SetActive(true);
        }

    }
    #endregion
}

