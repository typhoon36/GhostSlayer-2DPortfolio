using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HormingMs_Ctrl : MonoBehaviour
{
    public GameObject Target_Obj;
    public float m_MoveSpeed = 5.0f;
    public float m_RotSpeed = 200.0f;
    private Vector3 m_DesireDir;
    private bool m_HomingEnabled = true;

    void Update()
    {
        BulletHoming();
    }

    void BulletHoming()  //타겟을 향해 추적 이동하는 행동 패턴 함수
    {
        if (m_HomingEnabled)
        {
            m_DesireDir = Target_Obj.transform.position - transform.position;
            m_DesireDir.z = 0.0f;
            m_DesireDir.Normalize();

            // 타겟을 향해 유도탄 회전시키기
            float angle = Mathf.Atan2(m_DesireDir.y, m_DesireDir.x) * Mathf.Rad2Deg - 90; // 왼쪽을 기본 방향으로 설정
            Quaternion targetRot = Quaternion.Euler(0, 0, angle); // 각도를 쿼터니온으로
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                            targetRot, m_RotSpeed * Time.deltaTime);
        }

        // 유도탄이 바라보는 방향쪽으로 움직이게 하기
        transform.Translate(Vector3.up * m_MoveSpeed * Time.deltaTime, Space.Self);
    }

    void Start()
    {
        Destroy(gameObject, 5.0f);
        StartCoroutine(DisableHomingAfterDelay(2.0f)); // 2초 후 유도탄 비활성화
    }

    IEnumerator DisableHomingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_HomingEnabled = false;
    }
}
