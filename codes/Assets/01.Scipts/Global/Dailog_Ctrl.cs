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
    [SerializeField] private Button Next_Btn; // 다음 대사 버튼 추가

    bool IsDialogue = false; // 대화 중인지 체크
    int Cnt = 0; // 대사가 얼마나 진행되었는지 알려줄 변수

    [SerializeField] private Dialogue[] m_Dialog;

    // 대화 시작은 버튼으로 시작
    public void ShowDialogue()
    {
        if (IsDialogue) return; // 대화가 이미 진행 중이면 메서드 종료

        OnOff(true);
        Cnt = 0;
        NextDialogue(); // 첫 번째 대사 표시

        // 다음 대사 버튼 클릭 이벤트 추가
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
            OnOff(false); // 대사 끝
            Next_Btn.gameObject.SetActive(false); // 다음 대사 버튼 숨기기
            Exit_Btn.gameObject.SetActive(true);
            Exit_Btn.onClick.RemoveAllListeners();
            Exit_Btn.onClick.AddListener(() =>
            {
                m_DialogPanel.gameObject.SetActive(false);
                Init(); // 패널이 닫힐 때 초기화
            });
            IsDialogue = false;
        }
    }

    void Init()
    {
        OnOff(false);
        Next_Btn.gameObject.SetActive(true);
        Exit_Btn.gameObject.SetActive(false);
        IsDialogue = false; // 초기화 시 대화 상태도 초기화
        Cnt = 0; // 초기화 시 대사 진행도 초기화
    }
}
