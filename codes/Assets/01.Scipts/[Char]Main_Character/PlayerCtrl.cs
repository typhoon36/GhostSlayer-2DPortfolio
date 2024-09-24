using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCtrl : MonoBehaviour
{
    #region HP
    [HideInInspector] public float m_MaxHP = 100.0f;
    [HideInInspector] public float m_CurHP = 100.0f;
    #endregion

    #region �̵� & ����
    Rigidbody2D m_Rd;
    float m_Speed = 10.0f;
    float JumpForce = 600.0f;

    bool IsDJump = false;
    int m_ReserveJump = 0;
    #endregion

    #region ���� -- �нú�
    [Header("Dash")]
    public Ghost_Ctrl m_GhostEff;
    #endregion

    #region Attack --- �⺻����
    [Header("Attack")]
    public GameObject attackObj;
    #endregion

    #region Skill_1
    [Header("Skill_1")]
    public GameObject m_BulletObj;
    public Transform m_BulletPos;
    float m_ShotTime = 0.0f;
    public float BulletSpeed = 10.0f;
    #endregion

    #region Global
    Animator m_Anim;
    SpriteRenderer m_Sprite;
    Portal m_Portal = null;
    public GameObject m_Inven;
    bool IsInven = true;
    GameObject m_Seller = null;
    GameObject m_Chair = null;
    GameObject m_Witch = null;
    GameObject m_Crow = null;
    #endregion

    #region Singleton
    public static PlayerCtrl Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        m_Anim = GetComponent<Animator>();
        this.m_Rd = GetComponent<Rigidbody2D>();
        m_Sprite = GetComponent<SpriteRenderer>();

        // ���� ���� �� ����� ���� ��ġ�� �̵�
        GlobalValue.LoadGameData();
        transform.position = GlobalValue.g_SpawnPosition;
    }

    void Update()
    {
        //#�⺻ �̵� �� �������
        Move();
        Jump();
        Dash();

        //#�⺻ ����
        if (Input.GetMouseButton(0))
            StartCoroutine(Attack());

        if (Input.GetKeyDown(KeyCode.E))
            FireUpdate();

        #region ��ų2 ȸ��(���� ü�� ȸ�� �� 30%)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            float healAmount = 30.0f;
            m_CurHP += healAmount;
            if (m_MaxHP < m_CurHP)
                m_CurHP = m_MaxHP;

            Game_Mgr.Inst.UpdateHP(m_CurHP, m_MaxHP);

            // MpIcon�� ȸ���� ��ŭ ���̱�
            Game_Mgr.Inst.m_MPIcon.fillAmount -= healAmount / m_MaxHP;
        }
        #endregion

        #region ��Ż �̵�
        if (m_Portal != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Portal_Mgr.Inst.TpPlayer(m_Portal.portalID, transform, m_Portal.m_tpCool, true);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                Portal_Mgr.Inst.TpPlayer(m_Portal.portalID, transform, m_Portal.m_tpCool, false);
            }
        }
        #endregion

        #region ���� ����
        if (m_Seller != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Market_Mgr.Inst.Market_Panel.SetActive(true);
            }
        }
        #endregion

        #region �κ�����
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (m_Inven != null)
            {
                if (IsInven)
                {
                    m_Inven.SetActive(true);
                    IsInven = false;
                }
                else
                {
                    m_Inven.SetActive(false);
                    IsInven = true;
                }
            }
        }
        #endregion

        #region Chair�� ��ȣ�ۿ�
        if (m_Chair != null && Input.GetKeyDown(KeyCode.F))
        {
            AHeal();
        }
        #endregion

        #region Witch�� ��ȣ�ۿ�
        if (m_Witch != null && Input.GetKeyDown(KeyCode.F))
        {
            Game_Mgr.Inst.m_DialogPanel.SetActive(true);
        }
        #endregion

        #region Crow�� ��ȣ�ۿ�
        if (m_Crow != null && Input.GetKeyDown(KeyCode.F))
        {
            Game_Mgr.Inst.m_CDialoguePanel.SetActive(true);
        }
        #endregion

        #region # ����
        if (-35 > this.transform.position.y)
        {
            gameObject.SetActive(false);
            Game_Mgr.Inst.Death();
        }
        #endregion
    }

    void AHeal()
    {
        m_CurHP = m_MaxHP;

        Game_Mgr.Inst.UpdateHP(m_CurHP, m_MaxHP);
        Game_Mgr.Inst.m_MPIcon.fillAmount = 1.0f;
    }

    void OnApplicationQuit()
    {
        // ���� ���� �� ���� ��ġ�� ����
        GlobalValue.g_SpawnPosition = transform.position;
        GlobalValue.SaveGameData();
    }

    void OnDisable()
    {
        // �÷��̾ ��Ȱ��ȭ�� �� ���� ��ġ�� ����
        GlobalValue.g_SpawnPosition = transform.position;
        GlobalValue.SaveGameData();
    }

    #region Movement && Jump
    void Move()
    {
        float key = Input.GetAxisRaw("Horizontal");

        m_Rd.velocity = new Vector2(key * m_Speed, m_Rd.velocity.y);

        if (key == 0)
        {
            m_Anim.SetBool("IsWalk", false);
        }
        else
        {
            m_Anim.SetBool("IsWalk", true);

            if (key < 0)
            {
                m_Sprite.flipX = true;
            }
            else
            {
                m_Sprite.flipX = false;
            }
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_ReserveJump = 2;
        }

        if (0 < m_ReserveJump)
        {
            this.m_Rd.velocity = new Vector2(this.m_Rd.velocity.x, 0);
            this.m_Rd.AddForce(Vector2.up * JumpForce);
            m_ReserveJump = 0;
            IsDJump = true;
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && IsDJump)
            {
                this.m_Rd.velocity = new Vector2(this.m_Rd.velocity.x, 0);
                this.m_Rd.AddForce(Vector2.up * JumpForce);
                IsDJump = false;
            }
        }

        if (0 < m_ReserveJump)
            m_ReserveJump--;
    }
    #endregion

    #region Dash
    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            m_GhostEff.IsGhosting = true;
            m_GhostEff.FlipX = m_Sprite.flipX;
            float DashDir = m_Sprite.flipX ? -1 : 1;
            m_Rd.velocity = new Vector2(DashDir * 20.0f, m_Rd.velocity.y);
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            m_GhostEff.IsGhosting = false;
        }
    }
    #endregion

    //------ ���� ����
    #region �Ϲ� ����
    IEnumerator Attack()
    {
        if (attackObj != null)
        {
            // ���� ������Ʈ�� ����� ��ġ�� �÷��̾��� ���⿡ �°� ����
            SpriteRenderer a_Sprite = attackObj.GetComponent<SpriteRenderer>();
            if (a_Sprite != null)
            {
                a_Sprite.flipX = m_Sprite.flipX;
            }

            Vector3 attackPos = attackObj.transform.localPosition;
            attackPos.x = m_Sprite.flipX ? -0.15f : 0.15f;
            attackObj.transform.localPosition = attackPos;

            attackObj.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            attackObj.SetActive(false);
        }
    }
    #endregion


    void FireUpdate()
    {
        // m_MPIcon�� 0�̸� ���� ���� ����
        if (Game_Mgr.Inst.m_MPIcon.fillAmount <= 0) return;


        // m_ShotTime�� 0���� ũ�� ���� ���� ����
        if (m_ShotTime > 0) return;


        Vector3 a_Target = m_Sprite.flipX ? Vector3.left : Vector3.right;
        a_Target.Normalize();

        Bullet_Ctrl a_BulletSc = BulletPool_Mgr.Inst.GetALBulletPool();

        a_BulletSc.gameObject.SetActive(true);
        a_BulletSc.BulletSpawn(m_BulletPos.position, a_Target, BulletSpeed);

        a_BulletSc.transform.right = a_Target;

        Game_Mgr.Inst.m_MPIcon.fillAmount -= 0.5f;

        // ��Ÿ�� �ʱ�ȭ
        m_ShotTime = 2.0f;
    }

    void FixedUpdate()
    {
        // m_ShotTime ����
        if (m_ShotTime > 0)
        {
            m_ShotTime -= Time.fixedDeltaTime;
        }
    }


    //------ �浹 �� ������ ����
    #region �浹 & ������
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Forest" ||
            coll.gameObject.tag == "Lab" ||
            coll.gameObject.tag == "Fort")
        {
            IsDJump = false;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Boss")
        {
            OnDamaged(coll.transform.position);
        }

        else if (coll.gameObject.tag == "Monster")
        {
            OnDamaged(coll.transform.position);
        }
        else if(coll.gameObject.tag == "Enermy_Bullet")
        {
            OnDamaged(coll.transform.position);
        }

        else if (coll.gameObject.tag == "Trap")
        {
            OnDamaged(coll.transform.position);
        }

        else if (coll.gameObject.tag == "Portal")
        {
            Portal portal = coll.GetComponent<Portal>();
            if (portal != null)
            {
                m_Portal = portal;
            }
        }
        else if (coll.gameObject.tag == "Seller")
        {
            m_Seller = coll.gameObject;
        }
        else if (coll.gameObject.tag  == "Witch")
        {
            m_Witch = coll.gameObject;
        }

        else if (coll.gameObject.tag == "Crow")
        {
            m_Crow = coll.gameObject;
        }

        else if (coll.gameObject.tag == "Chair")
        {
            m_Chair = coll.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Portal")
        {
            m_Portal = null;
        }
        else if (coll.gameObject.tag == "Seller")
        {
            m_Seller = null;
        }
        else if (coll.gameObject.tag == "Chair")
        {
            m_Chair = null; // Chair ������Ʈ ����
        }
        else if (coll.gameObject.tag == "Witch")
        {
            m_Witch = null; // Witch ������Ʈ ����
        }

        else if (coll.gameObject.tag == "Crow")
        {
            m_Crow = null; // Crow ������Ʈ ����
        }

    }

    //# ������ ó��
    void OnDamaged(Vector2 a_TargetPos)
    {
        if (m_GhostEff.IsGhosting) return;

        m_Sprite.color = new Color(1, 1, 1, 0.4f);

        int dir = transform.position.x - a_TargetPos.x > 0 ? 1 : -1;
        m_Rd.AddForce(new Vector2(dir * 2, 2), ForceMode2D.Impulse); // �з����� ����� �� ����

        m_CurHP -= 10.0f;
        Game_Mgr.Inst.UpdateHP(m_CurHP, m_MaxHP);

        if (m_CurHP <= 0)
        {
            Game_Mgr.Inst.Death();
        }

        Invoke("OffDamaged", 1.0f);
    }

    void OffDamaged()
    {
        m_Sprite.color = new Color(1, 1, 1, 1);
    }
    #endregion
}
