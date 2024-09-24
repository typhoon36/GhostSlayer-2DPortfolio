using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    Vector3 m_DirVec = Vector3.right;       //날아가야 할 방향 벡터
    float m_MoveSpeed = 15.0f;              //이동속도

    float m_LifeTime = 3.0f;

    void OnEnable()  // Active가 활성화 될 때마다 호출되는 함수
    {
        m_LifeTime = 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_LifeTime)
        {
            m_LifeTime -= Time.deltaTime;
            if (m_LifeTime <= 0.0f)
                gameObject.SetActive(false);
        }

        transform.position += m_DirVec * Time.deltaTime * m_MoveSpeed;

        // 카메라 뷰포트 좌표를 사용하여 총알이 화면 밖으로 나갔는지 확인
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            gameObject.SetActive(false);
        }
    }

    public void BulletSpawn(Vector3 a_StPos, Vector3 a_DirVec,
                            float a_MvSpeed = 15.0f, float Att = 20.0f)
    {
        m_DirVec = a_DirVec;
        transform.position = new Vector3(a_StPos.x, a_StPos.y, 0.0f);
        m_MoveSpeed = a_MvSpeed;
    }
}
