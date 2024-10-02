using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest_Ctrl : MonoBehaviour
{
    [HideInInspector] public int goldAmount = 700;
    Animator m_Anim;
    bool isOpened = false;

    void Start()
    {
        m_Anim = GetComponent<Animator>();
    }

    void Update()
    {
        // ���ڰ� ���ȴ��� Ȯ��
        if (isOpened && m_Anim != null)
        {
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Open") &&
                m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                Open();
                isOpened = false; // ���ڰ� �������� ǥ��
            }
        }
    }

    public void Open()
    {
        Game_Mgr.Inst.AddGold(goldAmount);

    

        gameObject.SetActive(false);
    }

    public void TriggerOpen()
    {
        if (m_Anim != null && !isOpened)
        {
            m_Anim.SetTrigger("IsOpen");
            isOpened = true; // ���ڰ� �������� ����
        }
    }
}
