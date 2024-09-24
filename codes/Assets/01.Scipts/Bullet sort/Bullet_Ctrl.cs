using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    Vector3 m_DirVec = Vector3.right;       //���ư��� �� ���� ����
    float m_MoveSpeed = 15.0f;              //�̵��ӵ�

    float m_LifeTime = 3.0f;

    void OnEnable()  // Active�� Ȱ��ȭ �� ������ ȣ��Ǵ� �Լ�
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

        // ī�޶� ����Ʈ ��ǥ�� ����Ͽ� �Ѿ��� ȭ�� ������ �������� Ȯ��
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
