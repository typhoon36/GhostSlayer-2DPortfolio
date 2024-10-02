using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCtrl : MonoBehaviour
{

    #region �̵� & ����
    Rigidbody2D m_Rd;
    float m_Speed = 10.0f;
    float JumpForce = 400.0f;

    bool IsDJump = false;
    int m_ReserveJump = 0;
    float m_DJumpCool = 3f;
    float m_DJCooldown = 0.0f;
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
    GameObject m_Robot = null;
    GameObject m_Chest = null;
    GameObject m_Post = null;
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
        //#�⺻ �̵� �� ���� ó��
        Move();
        Jump();
        Dash();

        //# ���� ���� ��Ÿ��
        if (m_DJCooldown > 0)
            m_DJCooldown -= Time.deltaTime;

        //#�⺻ ����
        if (!Game_Mgr.IsPointerOverUIObject() && Input.GetMouseButton(0))
            StartCoroutine(Attack());

        if (!Game_Mgr.IsPointerOverUIObject() && Input.GetKeyDown(KeyCode.E))
            FireUpdate();

        #region ��ų2 ȸ��(���� ü�� ȸ���� 30%)
        if (Input.GetKeyDown(KeyCode.Q) && Game_Mgr.Inst.m_MPIcon.fillAmount > 0)
        {
            Game_Mgr.Inst.RecoverHP(3.0f);
        }
        #endregion

        #region ��Ż �̵�
        if (m_Portal != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Portal_Mgr.Inst.TpPlayer(m_Portal.portalID, m_Portal.connectedPortalID,
                    transform, m_Portal.m_tpCool);
            }
        }
        #endregion

        #region ���� ��ȣ�ۿ�
        if (m_Seller != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Market_Mgr.Inst.Market_Panel.SetActive(true);
            }
        }
        #endregion

        #region �κ��丮
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

        #region ���� ��ȣ�ۿ�
        if (m_Chest != null && Input.GetKeyDown(KeyCode.F))
        {
            OpenChest();
        }
        #endregion

        #region ǥ���� ��ȣ�ۿ�
        if (m_Post != null && Input.GetKeyDown(KeyCode.F))
        {
            Game_Mgr.Inst.m_PostDialoguePanel.SetActive(true);
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

        #region Robot�� ��ȣ�ۿ�
        if (m_Robot != null && Input.GetKeyDown(KeyCode.F))
        {
            Game_Mgr.Inst.m_RDialoguePanel.SetActive(true);
        }
        #endregion

    }

    void FixedUpdate()
    {
        // m_ShotTime ����
        if (m_ShotTime > 0)
        {
            m_ShotTime -= Time.fixedDeltaTime;
        }
    }

    void AHeal()
    {
        Game_Mgr.Inst.m_HPBar.fillAmount = 1.0f;
        Game_Mgr.Inst.m_CurHP = Game_Mgr.Inst.m_MaxHP;
        Game_Mgr.Inst.m_HPText.text = GlobalValue.g_CurHP.ToString() + " / " + Game_Mgr.Inst.m_MaxHP.ToString();

        Game_Mgr.Inst.m_MPIcon.fillAmount = 1.0f;
    }

    void OnApplicationQuit()
    {
        // ���� ���� �� ���� ��ġ�� ����
        GlobalValue.g_SpawnPosition = transform.position;
        //���� ����� ü�� ����
        GlobalValue.g_CurHP = Game_Mgr.Inst.m_CurHP;

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
            if (m_ReserveJump == 0 && m_DJCooldown <= 0)
            {
                m_ReserveJump = 2;
            }
            else if (m_ReserveJump == 1 && IsDJump && m_DJCooldown <= 0)
            {
                m_ReserveJump = 2;
            }
        }

        if (m_ReserveJump > 0)
        {
            this.m_Rd.velocity = new Vector2(this.m_Rd.velocity.x, 0);
            this.m_Rd.AddForce(Vector2.up * JumpForce);
            m_ReserveJump--;
            m_Anim.SetBool("IsJump", true);
            if (m_ReserveJump == 1)
            {
                IsDJump = true;
            }
            else
            {
                IsDJump = false;
                m_DJCooldown = m_DJumpCool;
            }
        }
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
            m_Rd.velocity = new Vector2(DashDir * 40.0f, m_Rd.velocity.y);
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

    #region skill_1
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
    #endregion

    #region ���� ��ȣ�ۿ�
    void OpenChest()
    {
        if (m_Chest != null)
        {
            Chest_Ctrl chestCtrl = m_Chest.GetComponent<Chest_Ctrl>();
            if (chestCtrl != null)
            {
                chestCtrl.TriggerOpen();
            }
        }
    }
    #endregion

    //------ �浹 �� ������ ����
    #region �浹 & ������
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Forest") ||
            coll.gameObject.layer == LayerMask.NameToLayer("Lab"))
        {
            IsDJump = false;
            m_Anim.SetBool("IsJump", false);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        #region Damage
        if (coll.gameObject.tag == "Boss")
            OnDamaged(coll.transform.position);

        else if (coll.gameObject.tag == "Monster")
            OnDamaged(coll.transform.position);

        else if (coll.gameObject.tag == "Enermy_Bullet")
            OnDamaged(coll.transform.position);

        else if (coll.gameObject.tag == "Trap")
            OnDamaged(coll.transform.position);
        #endregion

        else if (coll.gameObject.tag == "Portal")
        {
            Portal portal = coll.GetComponent<Portal>();
            if (portal != null)
            {
                m_Portal = portal;
            }
        }

        #region NPCS
        else if (coll.gameObject.tag == "Seller")
            m_Seller = coll.gameObject;

        else if (coll.gameObject.tag  == "Witch")
            m_Witch = coll.gameObject;

        else if (coll.gameObject.tag == "Crow")
            m_Crow = coll.gameObject;

        else if (coll.gameObject.tag == "Robot")
            m_Robot = coll.gameObject;

        else if (coll.gameObject.tag == "Chair")
            m_Chair = coll.gameObject;

        

        else if (coll.gameObject.tag == "Post")
            m_Post = coll.gameObject;
        #endregion

        else if (coll.gameObject.tag == "Chest")
            m_Chest = coll.gameObject;
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        #region NPCS
        if (coll.gameObject.tag == "Portal")
            m_Portal = null;

        else if (coll.gameObject.tag == "Seller")
            m_Seller = null;

        else if (coll.gameObject.tag == "Chair")
            m_Chair= null;

        else if (coll.gameObject.tag == "Witch")
            m_Witch = null;

        else if (coll.gameObject.tag == "Crow")
            m_Crow = null;

        else if (coll.gameObject.tag == "Post")
            m_Post = null;

        else if (coll.gameObject.tag == "Robot")
            m_Robot = null;
        #endregion

        #region Chest
        else if (coll.gameObject.tag == "Chest")
            m_Chest = null;
        #endregion
    }

    //# ������ ó��
    void OnDamaged(Vector2 a_TargetPos)
    {
        if (m_GhostEff.IsGhosting) return;

        m_Sprite.color = new Color(1, 1, 1, 0.4f);

        int dir = transform.position.x - a_TargetPos.x > 0 ? 1 : -1;
        m_Rd.AddForce(new Vector2(dir * 2, 2), ForceMode2D.Impulse); // �з����� ����� �� ����

        Game_Mgr.Inst.m_HPBar.fillAmount -= 0.2f;

        Game_Mgr.Inst.m_CurHP -= 20.0f;

        Game_Mgr.Inst.m_HPText.text = Game_Mgr.Inst.m_CurHP.ToString() + " / " + Game_Mgr.Inst.m_MaxHP.ToString();

        if (Game_Mgr.Inst.m_CurHP <= 0)
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
