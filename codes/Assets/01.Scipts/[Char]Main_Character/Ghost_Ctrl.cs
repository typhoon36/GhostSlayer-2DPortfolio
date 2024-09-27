using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ghost_Ctrl : MonoBehaviour
{
    public float G_Delay;
    float G_DelayTime;

    [Header("Ghost")]
    [HideInInspector] public GameObject Ghost_Eff;
    [HideInInspector] public bool IsGhosting = false;
    [HideInInspector] public bool FlipX = false; //flipX 상태를 저장

    // Start is called before the first frame update
    void Start()
    {
        G_DelayTime = G_Delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGhosting == true)
        {
            if (G_DelayTime > 0)
            {
                G_DelayTime -= Time.deltaTime;
            }
            else
            {
                //## 이펙트
                GameObject a_Eff = Instantiate(Ghost_Eff, transform.position, Quaternion.identity);
                Sprite a_Sprite = GetComponent<SpriteRenderer>().sprite;
                a_Eff.transform.localScale = this.transform.localScale;
                a_Eff.GetComponent<SpriteRenderer>().sprite = a_Sprite;
                a_Eff.GetComponent<SpriteRenderer>().flipX = FlipX; // flipX 상태 설정
                G_DelayTime = G_Delay;
                Destroy(a_Eff, 1f);
            }
        }
        else
        {
            return;
        }
    }
}
