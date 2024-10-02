using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//# 복수의 대화 내용을 저장할 수 있는 클래스//Custom Class 접근
[System.Serializable]
public class Dialogue
{
    [TextArea]
    public string dialogue;
}

public class Dailog_Ctrl : MonoBehaviour
{
    [SerializeField] Image m_Box;
    [SerializeField] GameObject m_DialogPanel;
    [SerializeField] private Text Txt_Dialogue;
    [SerializeField] private Button Exit_Btn;
    [SerializeField] private Button Next_Btn;

    bool IsDialogue = false;
    int Cnt = 0;
    [SerializeField] private Dialogue[] m_Dialog;

    public void ShowDialogue()
    {
        if (IsDialogue) return;

        OnOff(true);
        Cnt = 0;
        NextDialogue();

        Next_Btn.gameObject.SetActive(true);
        Next_Btn.onClick.RemoveAllListeners();
        Next_Btn.onClick.AddListener(NextDialogue);
    }

    void OnOff(bool a_flag)
    {
        Txt_Dialogue.gameObject.SetActive(a_flag);
        IsDialogue = a_flag;
    }

    public void NextDialogue()
    {
        if (Cnt < m_Dialog.Length)
        {
            Txt_Dialogue.text = m_Dialog[Cnt].dialogue;
            Cnt++;
        }
        else
        {
            OnOff(false);
            Next_Btn.gameObject.SetActive(false);
            Exit_Btn.gameObject.SetActive(true);
            Exit_Btn.onClick.RemoveAllListeners();
            Exit_Btn.onClick.AddListener(() =>
            {
                m_DialogPanel.gameObject.SetActive(false);
                Init();
            });
            IsDialogue = false;
        }
    }

    void Init()
    {
        OnOff(false);
        Next_Btn.gameObject.SetActive(true);
        Exit_Btn.gameObject.SetActive(false);
        IsDialogue = false;
        Cnt = 0;
        ShowDialogue(); // 초기화 후 첫 번째 다이얼로그 표시
    }
}
