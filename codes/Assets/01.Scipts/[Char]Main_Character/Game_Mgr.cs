using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Game_Mgr : MonoBehaviour
{
    #region HUD
    [Header("HUD")]
    public Image m_HPBar;
    public Text m_HPText;
    public Image m_MPIcon;
    float m_InCrease = 0.01f;
    public Text m_ItemTxt;
    int m_CurGold = 0;
    [HideInInspector] public float m_CurHP = 100;
    [HideInInspector] public float m_MaxHP = 100;
    Coroutine m_recoverHPCo;
    #endregion

    // Menu
    [Header("Menu")]
    public GameObject m_MenuPanel;
    public Button m_PauseBtn;
    public Button m_ResumeBtn;
    public Button m_ResetBtn;
    public Button m_ExitBtn;

    // Reinforce
    [Header("Reinforce")]
    public GameObject m_ReinPanel;
    public Button m_ReinforceBtn;
    public Text m_HelpTxt;
    public Button m_RCloseBtn;

    // Skill
    [Header("Skill")]
    public GameObject m_SkillPanel;

    // Death
    [Header("Death")]
    public GameObject m_DeathPanel;
    public Button m_ConfirmBtn;
    public Button m_CancelBtn;

    // Info
    [Header("Info")]
    public Text Info_Txt;

    #region MiniMap
    [Header("MiniMap")]
    public GameObject m_MiniMap;
    public Button m_CloseMapBtn;
    #endregion

    #region Dialog
    [Header("Dialog")]
    public GameObject m_DialogPanel;
    public GameObject m_CDialoguePanel;
    public GameObject m_RDialoguePanel;
    public GameObject m_PostDialoguePanel;
    #endregion

    #region Singleton
    public static Game_Mgr Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        GlobalValue.LoadGameData();
        m_MaxHP = GlobalValue.g_MaxHP;
        m_CurHP = GlobalValue.g_CurHP;

        UpdateHP();

        #region MENU
        if (m_PauseBtn != null)
        {
            m_PauseBtn.onClick.AddListener(() =>
            {
                if (IsPointerOverUIObject())
                {
                    m_MenuPanel.SetActive(true);
                    Time.timeScale = 0;
                }
            });
        }

        if (m_ResumeBtn != null)
        {
            m_ResumeBtn.onClick.AddListener(() =>
            {
                if (IsPointerOverUIObject())
                {
                    m_MenuPanel.SetActive(false);
                    Time.timeScale = 1;
                }
            });
        }

        if (m_ResetBtn != null)
        {
            m_ResetBtn.onClick.AddListener(() =>
            {
                if (IsPointerOverUIObject())
                {
                    GlobalValue.ResetGameData();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    Time.timeScale = 1;
                }
            });
        }

        if (m_ExitBtn != null)
        {
            m_ExitBtn.onClick.AddListener(() =>
            {
                if (IsPointerOverUIObject())
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
            });
        }
        #endregion

        #region Reinforce
        if (m_ReinforceBtn != null)
        {
            m_ReinforceBtn.onClick.AddListener(() =>
            {
                if (IsPointerOverUIObject())
                {
                    Game_Mgr.Inst.m_MaxHP += 10;
                    Game_Mgr.Inst.m_CurHP += 10;
                    GlobalValue.g_UserGold -= 100;

                    GlobalValue.g_MaxHP = Game_Mgr.Inst.m_MaxHP;
                    GlobalValue.g_CurHP = Game_Mgr.Inst.m_CurHP;
                    GlobalValue.SaveGameData();

                    if (m_HelpTxt != null)
                    {
                        m_HelpTxt.gameObject.SetActive(true);
                        m_HelpTxt.text = "체력이 10 증가하였습니다.";
                    }

                    UpdateHP();
                }
            });
        }
        if (m_RCloseBtn != null)
        {
            m_RCloseBtn.onClick.AddListener(() =>
            {
                if (IsPointerOverUIObject())
                {
                    m_ReinPanel.SetActive(false);
                }
            });
        }
        #endregion

        #region MiniMap
        if (m_MiniMap != null)
        {
            m_MiniMap.SetActive(false);
        }

        if (m_CloseMapBtn != null)
        {
            m_CloseMapBtn.onClick.AddListener(() =>
            {
                if (IsPointerOverUIObject())
                {
                    m_MiniMap.SetActive(false);
                }
            });
        }
        #endregion
    }

    void Update()
    {
        #region HUD
        if (m_MPIcon != null)
        {
            m_MPIcon.fillAmount += m_InCrease * Time.deltaTime;
            if (m_MPIcon.fillAmount > 1.0f)
            {
                m_MPIcon.fillAmount = 1.0f;
            }
        }

        if (m_ItemTxt != null)
        {
            m_ItemTxt.text = GlobalValue.g_UserGold.ToString("N0");

            if (GlobalValue.g_UserGold < 0)
            {
                m_ItemTxt.text = 0.ToString();
            }
        }
        #endregion

        #region Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_MenuPanel.activeSelf)
            {
                m_MenuPanel.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                m_MenuPanel.SetActive(true);
                m_SkillPanel.SetActive(false);
                m_DeathPanel.SetActive(false);
                m_ReinPanel.SetActive(false);
                m_MiniMap.SetActive(false);
                m_DialogPanel.SetActive(false);
                m_CDialoguePanel.SetActive(false);
                m_RDialoguePanel.SetActive(false);
                Inven_Mgr.Inst.m_InvenPanel.SetActive(false);
            }
        }
        #endregion

        #region Skill
        if (Input.GetKeyDown(KeyCode.K))
        {
            m_SkillPanel.SetActive(!m_SkillPanel.activeSelf);
        }
        #endregion

        #region miniMap
        if (Input.GetKeyDown(KeyCode.M))
        {
            m_MiniMap.SetActive(!m_MiniMap.activeSelf);
            Time.timeScale = m_MiniMap.activeSelf ? 0 : 1;
        }
        #endregion
    }

    #region HP
    public void StartRecoverHP()
    {
        if (m_recoverHPCo != null)
        {
            StopCoroutine(m_recoverHPCo);
        }
        m_recoverHPCo = StartCoroutine(RecoverHPOverTime());
    }

    public void StopRecoverHP()
    {
        if (m_recoverHPCo != null)
        {
            StopCoroutine(m_recoverHPCo);
            m_recoverHPCo = null;
        }
    }

    private IEnumerator RecoverHPOverTime()
    {
        while (true)
        {
            RecoverHP(1f);
            yield return new WaitForSeconds(1f);
        }
    }

    public void RecoverHP(float amount)
    {
        m_CurHP = Mathf.Min(m_CurHP + amount, m_MaxHP);
        GlobalValue.g_CurHP = m_CurHP;
        UpdateHP();
    }


    public void UpdateHP()
    {
        m_HPBar.fillAmount = m_CurHP / m_MaxHP;
        m_HPText.text = $"{m_CurHP} / {m_MaxHP}";
    }
    #endregion

    #region Death
    public void Death()
    {
        m_DeathPanel.SetActive(true);
        Time.timeScale = 0;
        m_HPBar.fillAmount = 0;

        if (m_ConfirmBtn != null)
        {
            m_ConfirmBtn.onClick.AddListener(() =>
            {
                if (IsPointerOverUIObject())
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    Time.timeScale = 1;

                    GameObject player = GameObject.FindWithTag("Player");
                    if (player != null)
                    {
                        player.transform.position = new Vector3(-70f, -23.1f, 0);
                        GlobalValue.g_SpawnPosition = player.transform.position;
                        GlobalValue.SaveGameData();
                    }

                    if (0 > GlobalValue.g_CurHP)
                        m_HPText.text = "0" + " / " + Mathf.FloorToInt(GlobalValue.g_MaxHP);
                    UpdateHP();
                }
            });
        }

        if (m_CancelBtn != null)
        {
            m_CancelBtn.onClick.AddListener(() =>
            {
                if (IsPointerOverUIObject())
                {
                    SceneManager.LoadScene("TitleScene");
                    Time.timeScale = 1;
                }
            });
        }
    }
    #endregion

    #region 골드
    public void AddGold(int a_Val)
    {
        if (m_CurGold <= int.MaxValue - a_Val)
            m_CurGold += a_Val;
        else
            m_CurGold = int.MaxValue;

        if (GlobalValue.g_UserGold <= int.MaxValue - a_Val)
            GlobalValue.g_UserGold += a_Val;
        else
            GlobalValue.g_UserGold = int.MaxValue;

        m_ItemTxt.text = m_CurGold.ToString("N0");

        PlayerPrefs.SetInt("Gold", GlobalValue.g_UserGold);
    }
    #endregion

    #region 슬롯 아이템 정보 업데이트
    public void UpdateAllSlotsInfo()
    {
        if (Info_Txt != null)
        {
            bool hasItem0 = false;
            bool hasItem3 = false;
            float TotalAtk = 0;

            foreach (E_Slot slot in FindObjectsOfType<E_Slot>())
            {
                if (slot.itemID == 0)
                {
                    hasItem0 = true;
                }
                if (slot.itemID == 3)
                {
                    hasItem3 = true;
                }

                if (slot.itemID != -1)
                {
                    TotalAtk += ItemDB.Inst.m_ItemDB[slot.itemID].DamageModifier;
                }
            }

            if (hasItem3)
            {
                Info_Txt.text = "공격력 : " + TotalAtk + "\n방어력 : 10";
            }
            else if (hasItem0)
            {
                Info_Txt.text = "공격력 : " + TotalAtk + "\n방어력 : 10";
            }
            else
            {
                Info_Txt.text = "공격력 : 10\n방어력 : 10";
            }
        }
    }
    #endregion

    public static bool IsPointerOverUIObject()
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
        List<RaycastResult> results = new List<RaycastResult>();
        for (int i = 0; i < Input.touchCount; ++i)
        {
            a_EDCurPos.position = Input.GetTouch(i).position;
            results.Clear();
            EventSystem.current.RaycastAll(a_EDCurPos, results);
            if (0 < results.Count)
                return true;
        }
        return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }
}

