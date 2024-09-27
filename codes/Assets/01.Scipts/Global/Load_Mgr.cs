using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Load_Mgr : MonoBehaviour
{
    public Text LoadText;
    public Text Progress_Text;
    public Slider Progress_bar;


    void Start()
    {
        StartCoroutine(LoadingScene()); 
    }


    IEnumerator LoadingScene()
    {
        yield return null;
        AsyncOperation Oper = SceneManager.LoadSceneAsync("Tema");
        Oper.allowSceneActivation = false;


        while(!Oper.isDone)
        {
            yield return null;

            if (Progress_bar.value < 0.9f)
            {
                Progress_bar.value = Mathf.MoveTowards(Progress_bar.value, 0.9f, Time.deltaTime);
            }

            else if(Oper.progress >= 0.9f)
            {
                Progress_bar.value = Mathf.MoveTowards(Progress_bar.value, 1f, Time.deltaTime);
            }

            if (Progress_bar.value >= 1f)
            {
                LoadText.text = "로딩 완료";
                Progress_Text.text = "키를 눌러 계속하세요";
            }

            if(Input.anyKeyDown&& 1<= Progress_bar.value && 0.9f <=Oper.progress)
            {
                Oper.allowSceneActivation = true;
            }



        }

    }



 
}
