using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest_Ctrl : MonoBehaviour
{
    public int goldAmount = 100;
    Animator m_Anim;
    bool isOpened = false;

    void Start()
    {
        m_Anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 상자가 열렸는지 확인
        if (isOpened && m_Anim != null)
        {
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Open") &&
                m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                Open();
                isOpened = false; // 상자가 열렸음을 표시
            }
        }
    }

    public void Open()
    {
        // 골드 증가 로직
        Game_Mgr.Inst.AddGold(goldAmount);
        // 상자 비활성화 또는 다른 로직 추가
        gameObject.SetActive(false);
    }

    public void TriggerOpen()
    {
        if (m_Anim != null && !isOpened)
        {
            m_Anim.SetTrigger("IsOpen");
            isOpened = true; // 상자가 열리도록 설정
        }
    }
}
