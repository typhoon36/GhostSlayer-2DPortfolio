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


    //# 효과 
    void Update()
    {

        //## 버튼 깜빡이는 효과
        time += Time.deltaTime;
        if (time > 1.0f)
            time = 0;

        float alpha = Mathf.Abs(time - 0.5f) * 2;
        Color color = new Color(alpha, alpha, alpha, 1);// R 값은 항상 1 (빨간색), G와 B 값은 alpha에 따라 변함.
        //R값을 알파로 변하게했으니 빨간색이 아닌 다른색으로 변하게됨

        // 버튼의 텍스트 색상 변경
        Text buttonText = Start_Btn.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.color = color;
        }
        //~버튼 깜빡이는 효과




    }



}
