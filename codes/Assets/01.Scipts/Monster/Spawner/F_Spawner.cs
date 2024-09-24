using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Spawner : MonoBehaviour
{
    public GameObject[] MonObj;
    public Transform[] MonPos;

    float m_SpDelta = 0.0f;
    private List<Vector3> usedPositions = new List<Vector3>();

    #region #Singleton Pattern

    public static F_Spawner Inst = null;
    private void Awake()
    {
        Inst = this;
    }
    #endregion

    void Update()
    {
        m_SpDelta -= Time.deltaTime;

        if (m_SpDelta < 0.0f)
        {
            m_SpDelta = 0.2f; // ���� �ֱ⸦ 1�ʷ� ����

            GameObject Mon = null;
            int RandMon = Random.Range(0, MonObj.Length);
            int RandPos = Random.Range(0, MonPos.Length);
            Vector3 spawnPosition = MonPos[RandPos].position;

            Mon = Instantiate(MonObj[RandMon]);

            // MonObj[2]�� ��� y ���� MonPos�� y ���� 2�� ���� ������ ����
            if (RandMon == 2)
            {
                spawnPosition.y += 5.0f;
            }

            // ���Ͱ� ��ġ�� �ʵ��� ��ġ Ȯ��
            if (!usedPositions.Contains(spawnPosition))
            {
                usedPositions.Add(spawnPosition);
                Mon.transform.position = spawnPosition;
            }
            else
            {
                Destroy(Mon);
            }
        }
    }
}
