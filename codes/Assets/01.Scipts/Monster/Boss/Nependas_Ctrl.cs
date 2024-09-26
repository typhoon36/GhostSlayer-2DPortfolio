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

    #region  움직임 변수
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
    private float bulletPatternInterval = 5.0f; // 총알 패턴 실행 간격
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

        // 시작할 때 상태를 Roar로 설정
        m_State = State.Roar;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, m_Player.transform.position);

        if (distanceToPlayer < 50.0f) // 플레이어가 보스와 가까이 있을 때
        {
            m_HPGroup.gameObject.SetActive(true);
            Move();
        }
        else // 플레이어가 보스와 멀리 있을 때
        {
            m_HPGroup.gameObject.SetActive(false);
            m_State = State.Roar; // 보스 행동을 멈추고 Roar 상태로 설정
        }

        // 일정 시간마다 Bullet 패턴 실행
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

    #region 기본 이동
    void Move()
    {
        m_CurPos = transform.position;
        m_DirPos = Vector3.left;

        if (m_Player != null)
        {
            Vector3 a_CacVec = m_Player.transform.position - transform.position;
            m_DirPos = a_CacVec;

            // 플레이어의 위치에 따라 flipX 설정
            if (m_Player.transform.position.x > transform.position.x)
                m_Sprite.flipX = true; // 플레이어가 오른쪽에 있을 때
            else
                m_Sprite.flipX = false; // 플레이어가 왼쪽에 있을 때
        }

        Vector3 a_CacPos = m_CurPos + m_DirPos.normalized * m_Speed * Time.deltaTime;
        a_CacPos.y = -21.86f; // y 값을 고정
        transform.position = a_CacPos;
        m_Anim.SetBool("IsWalk", true);
    }
    #endregion

    #region 포효 -- 인트로
    void Roar()
    {
        if (!IsRoar && m_RoarEff != null && m_EffCanvas != null)
        {
            m_EffCanvas.transform.localPosition = Vector3.zero; // 캔버스의 위치를 보스의 중앙으로 설정

            m_RoarEff.SetActive(true); // 포효 효과 오브젝트 활성화
            m_RoarEff.transform.localPosition = new Vector3(-243, 135, 0);

            IsRoar = true;
            m_Anim.SetBool("IsWalk", false);

            // 흔들림 효과 시작
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
        m_RoarEff.SetActive(false); // 포효 효과 비활성화

        // 포효가 끝나면 Move 상태로 변경
        m_State = State.Move;
    }
    #endregion

    #region 일반 총알
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

                // 회전
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
                    // 총알 패턴이 끝나면 Move 상태로 변경
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


    #region 충돌처리 & 피격
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Obstacle"))
        {
            if (m_Sprite != null)
            {
                // 충돌한 객체의 이름에 따라 flipX 설정
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

