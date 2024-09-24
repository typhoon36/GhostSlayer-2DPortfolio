using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title_Mgr : MonoBehaviour
{
    float time = 0;

    public Button Start_Btn;

    public Button Exit_Btn;


    // Start is called before the first frame update
    void Start()
    {
        if (Start_Btn != null)
        {
            Start_Btn.onClick.AddListener(() =>
            {
                Fade_Mgr.Inst.IsFadeIn = true;
                SceneManager.LoadScene("LoadScene");
            });
        }

        if (Exit_Btn != null)
        {
            Exit_Btn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;

#else
                Application.Quit();
#endif

            });
        }


    }


    //# ȿ�� 
    void Update()
    {

        //## ��ư �����̴� ȿ��
        time += Time.deltaTime;
        if (time > 1.0f)
            time = 0;

        float alpha = Mathf.Abs(time - 0.5f) * 2;
        Color color = new Color(alpha, alpha, alpha, 1);// R ���� �׻� 1 (������), G�� B ���� alpha�� ���� ����.
        //R���� ���ķ� ���ϰ������� �������� �ƴ� �ٸ������� ���ϰԵ�

        // ��ư�� �ؽ�Ʈ ���� ����
        Text buttonText = Start_Btn.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.color = color;
        }
        //~��ư �����̴� ȿ��




    }



}
