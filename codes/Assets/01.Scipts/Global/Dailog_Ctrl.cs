using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//# ������ ��ȭ ������ ������ �� �ִ� Ŭ����//Custom Class ����
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
    [SerializeField] private Button Next_Btn; // ���� ��� ��ư �߰�

    bool IsDialogue = false; // ��ȭ ������ üũ
    int Cnt = 0; // ��簡 �󸶳� ����Ǿ����� �˷��� ����

    [SerializeField] private Dialogue[] m_Dialog;

    // ��ȭ ������ ��ư���� ����
    public void ShowDialogue()
    {
        if (IsDialogue) return; // ��ȭ�� �̹� ���� ���̸� �޼��� ����

        OnOff(true);
        Cnt = 0;
        NextDialogue(); // ù ��° ��� ǥ��

        // ���� ��� ��ư Ŭ�� �̺�Ʈ �߰�
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
            OnOff(false); // ��� ��
            Next_Btn.gameObject.SetActive(false); // ���� ��� ��ư �����
            Exit_Btn.gameObject.SetActive(true);
            Exit_Btn.onClick.RemoveAllListeners();
            Exit_Btn.onClick.AddListener(() =>
            {
                m_DialogPanel.gameObject.SetActive(false);
                Init(); // �г��� ���� �� �ʱ�ȭ
            });
            IsDialogue = false;
        }
    }

    void Init()
    {
        OnOff(false);
        Next_Btn.gameObject.SetActive(true);
        Exit_Btn.gameObject.SetActive(false);
        IsDialogue = false; // �ʱ�ȭ �� ��ȭ ���µ� �ʱ�ȭ
        Cnt = 0; // �ʱ�ȭ �� ��� ���൵ �ʱ�ȭ
    }
}
