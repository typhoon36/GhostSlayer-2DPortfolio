using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCtrl : MonoBehaviour
{

    #region 이동 & 점프
    Rigidbody2D m_Rd;
    float m_Speed = 10.0f;
    float JumpForce = 400.0f;

    bool IsDJump = false;
    int m_ReserveJump = 0;
    float m_DJumpCool = 3f;
    float m_DJCooldown = 0.0f;
    #endregion

    #region 돌진 -- 패시브
    [Header("Dash")]
    public Ghost_Ctrl m_GhostEff;
    #endregion

    #region Attack --- 기본공격
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

        // 게임 시작 시 저장된 스폰 위치로 이동
        GlobalValue.LoadGameData();
        transform.position = GlobalValue.g_SpawnPosition;
    }

    void Update()
    {
        //#기본 이동 및 점프 처리
        Move();
        Jump();
        Dash();

        //# 더블 점프 쿨타임
        if (m_DJCooldown > 0)
            m_DJCooldown -= Time.deltaTime;

        //#기본 공격
        if (!Game_Mgr.IsPointerOverUIObject() && Input.GetMouseButton(0))
            StartCoroutine(Attack());

        if (!Game_Mgr.IsPointerOverUIObject() && Input.GetKeyDown(KeyCode.E))
            FireUpdate();

        #region 스킬2 회복(현재 체력 회복량 30%)
        if (Input.GetKeyDown(KeyCode.Q) && Game_Mgr.Inst.m_MPIcon.fillAmount > 0)
        {
            Game_Mgr.Inst.RecoverHP(3.0f);
        }
        #endregion

        #region 포탈 이동
        if (m_Portal != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Portal_Mgr.Inst.TpPlayer(m_Portal.portalID, m_Portal.connectedPortalID,
                    transform, m_Portal.m_tpCool);
            }
        }
        #endregion

        #region 상점 상호작용
        if (m_Seller != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Market_Mgr.Inst.Market_Panel.SetActive(true);
            }
        }
        #endregion

        #region 인벤토리
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

        #region 상자 상호작용
        if (m_Chest != null && Input.GetKeyDown(KeyCode.F))
        {
            OpenChest();
        }
        #endregion

        #region 표지판 상호작용
        if (m_Post != null && Input.GetKeyDown(KeyCode.F))
        {
            Game_Mgr.Inst.m_PostDialoguePanel.SetActive(true);
        }
        #endregion

        #region Chair와 상호작용
        if (m_Chair != null && Input.GetKeyDown(KeyCode.F))
        {
            AHeal();
        }
        #endregion

        #region Witch와 상호작용
        if (m_Witch != null && Input.GetKeyDown(KeyCode.F))
        {
            Game_Mgr.Inst.m_DialogPanel.SetActive(true);
        }
        #endregion

        #region Crow와 상호작용
        if (m_Crow != null && Input.GetKeyDown(KeyCode.F))
        {
            Game_Mgr.Inst.m_CDialoguePanel.SetActive(true);
        }
        #endregion

        #region Robot와 상호작용
        if (m_Robot != null && Input.GetKeyDown(KeyCode.F))
        {
            Game_Mgr.Inst.m_RDialoguePanel.SetActive(true);
        }
        #endregion

    }

    void FixedUpdate()
    {
        // m_ShotTime 감소
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
        // 게임 종료 시 현재 위치를 저장
        GlobalValue.g_SpawnPosition = transform.position;
        //게임 종료시 체력 저장
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

    //------ 공격 관련
    #region 일반 공격
    IEnumerator Attack()
    {
        if (attackObj != null)
        {
            // 공격 오브젝트의 방향과 위치를 플레이어의 방향에 맞게 설정
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
        // m_MPIcon이 0이면 총을 쏘지 않음
        if (Game_Mgr.Inst.m_MPIcon.fillAmount <= 0) return;

        // m_ShotTime이 0보다 크면 총을 쏘지 않음
        if (m_ShotTime > 0) return;

        Vector3 a_Target = m_Sprite.flipX ? Vector3.left : Vector3.right;
        a_Target.Normalize();

        Bullet_Ctrl a_BulletSc = BulletPool_Mgr.Inst.GetALBulletPool();

        a_BulletSc.gameObject.SetActive(true);
        a_BulletSc.BulletSpawn(m_BulletPos.position, a_Target, BulletSpeed);

        a_BulletSc.transform.right = a_Target;

        Game_Mgr.Inst.m_MPIcon.fillAmount -= 0.5f;

        // 쿨타임 초기화
        m_ShotTime = 2.0f;
    }
    #endregion

    #region 상자 상호작용
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

    //------ 충돌 및 데미지 관련
    #region 충돌 & 데미지
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

    //# 데미지 처리
    void OnDamaged(Vector2 a_TargetPos)
    {
        if (m_GhostEff.IsGhosting) return;

        m_Sprite.color = new Color(1, 1, 1, 0.4f);

        int dir = transform.position.x - a_TargetPos.x > 0 ? 1 : -1;
        m_Rd.AddForce(new Vector2(dir * 2, 2), ForceMode2D.Impulse); // 밀려나는 방향과 힘 조정

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
