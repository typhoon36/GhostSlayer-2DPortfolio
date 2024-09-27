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

    void BulletHoming()  //Ÿ���� ���� ���� �̵��ϴ� �ൿ ���� �Լ�
    {
        if (m_HomingEnabled)
        {
            m_DesireDir = Target_Obj.transform.position - transform.position;
            m_DesireDir.z = 0.0f;
            m_DesireDir.Normalize();

            // Ÿ���� ���� ����ź ȸ����Ű��
            float angle = Mathf.Atan2(m_DesireDir.y, m_DesireDir.x) * Mathf.Rad2Deg - 90; // ������ �⺻ �������� ����
            Quaternion targetRot = Quaternion.Euler(0, 0, angle); // ������ ���ʹϿ�����
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                            targetRot, m_RotSpeed * Time.deltaTime);
        }

        // ����ź�� �ٶ󺸴� ���������� �����̰� �ϱ�
        transform.Translate(Vector3.up * m_MoveSpeed * Time.deltaTime, Space.Self);
    }

    void Start()
    {
        Destroy(gameObject, 5.0f);
        StartCoroutine(DisableHomingAfterDelay(2.0f)); // 2�� �� ����ź ��Ȱ��ȭ
    }

    IEnumerator DisableHomingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_HomingEnabled = false;
    }
}
