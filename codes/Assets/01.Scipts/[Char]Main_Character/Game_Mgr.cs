using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Game_Mgr : MonoBehaviour
{
    //HUD
    [Header("HUD")]
    public Image m_HPBar;
    public Text m_HPText;
    public Image m_MPIcon;
    float m_InCrease = 0.01f;
    public Text m_ItemTxt;

    //Menu
    [Header("Menu")]
    public GameObject m_MenuPanel;
    public Button m_PauseBtn;
    public Button m_ResumeBtn;
    public Button m_ResetBtn;
    public Button m_ExitBtn;

    //Reinforce
    [Header("Reinforce")]
    public GameObject m_ReinPanel;
    public Button m_ReinforceBtn;
    public Text m_HelpTxt;
    public Button m_RCloseBtn;

    //Skill
    [Header("Skill")]
    public GameObject m_SkillPanel;

    //Death
    [Header("Death")]
    public GameObject m_DeathPanel;
    public Button m_ConfirmBtn;
    public Button m_CancelBtn;

    //Info
    [Header("Info")]
    public Text Info_Txt;

    #region MiniMap
    [Header("MiniMap")]
    public GameObject m_MiniMap;
    public Button m_CloseMapBtn;
    #endregion


    //gold
    int m_CurGold = 0;

    #region Dialog
    [Header("Dialog")]
    public GameObject m_DialogPanel;
    public GameObject m_CDialoguePanel;
    public GameObject m_RDialoguePanel;
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
        GlobalValue.LoadGameData(); // 게임 데이터 로드

        // 플레이어의 체력 초기화
        PlayerCtrl.Inst.m_MaxHP = GlobalValue.g_MaxHP;
        PlayerCtrl.Inst.m_CurHP = GlobalValue.g_CurHP;

        if (m_HPText != null)
        {
            m_HPText.text = PlayerCtrl.Inst.m_CurHP + " / " + PlayerCtrl.Inst.m_MaxHP;
        }

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
                    PlayerCtrl.Inst.m_MaxHP += 10;
                    PlayerCtrl.Inst.m_CurHP += 10;
                    GlobalValue.g_UserGold -= 100;

                    // 증가된 체력 저장
                    GlobalValue.g_MaxHP = PlayerCtrl.Inst.m_MaxHP;
                    GlobalValue.g_CurHP = PlayerCtrl.Inst.m_CurHP;
                    GlobalValue.SaveGameData();

                    if (m_HelpTxt != null)
                    {
                        m_HelpTxt.gameObject.SetActive(true);
                        m_HelpTxt.text = "체력이 10 증가하였습니다.";
                    }

                    // HUD 업데이트
                    UpdateHP(PlayerCtrl.Inst.m_CurHP, PlayerCtrl.Inst.m_MaxHP);
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
        // 골드값
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
                // 모든 패널 비활성화
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

    #region HP 업데이트
    public void UpdateHP(float a_CurHP, float a_MaxHP)
    {
        if (m_HPBar != null)
        {
            m_HPBar.fillAmount = a_CurHP / a_MaxHP;
        }
        if (m_HPText != null)
        {
            m_HPText.text = a_CurHP + " / " + a_MaxHP;
        }
    }
    public void RecoverHP(float amount)
    {
        if (m_HPBar != null)
        {
            m_HPBar.fillAmount += amount;
            if (m_HPBar.fillAmount > 1.0f)
            {
                m_HPBar.fillAmount = 1.0f;
                m_HPText.text = PlayerCtrl.Inst.m_CurHP + " / " + PlayerCtrl.Inst.m_MaxHP;
            }
        }
    }
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
                    // 현재 씬을 다시 로드
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    Time.timeScale = 1;

                    // 플레이어의 위치를 마지막 저장된 스폰 위치로 설정
                    GameObject player = GameObject.FindWithTag("Player");
                    if (player != null)
                    {
                        player.transform.position = new Vector3(-70f, -23.1f, 0);
                        GlobalValue.g_SpawnPosition = player.transform.position;
                        GlobalValue.SaveGameData();
                    }

                    // HP 텍스트 업데이트
                    UpdateHP(PlayerCtrl.Inst.m_CurHP, PlayerCtrl.Inst.m_MaxHP);
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

        //로컬 저장 유저 보유 골드값
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
            }

            if (hasItem3)
            {
                Info_Txt.text = "공격력 : 30\n방어력 : 10";
            }
            else if (hasItem0)
            {
                Info_Txt.text = "공격력 : 10\n방어력 : 10";
            }
            else
            {
                Info_Txt.text = "공격력 : 10\n방어력 : 10";
            }
        }
    }
    #endregion

    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
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

