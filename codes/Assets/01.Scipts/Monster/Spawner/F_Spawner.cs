using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Spawner : MonoBehaviour
{
    #region Spawn Variables
    public GameObject[] MonObj;
    public Transform[] MonPos;
    public float spawnInterval = 8f; // ���� ����
    public float SpawnCheck = 10f; // ���� ��ġ Ȯ�� �ݰ�
    public float MinDistanceBetweenMonsters = 20f; // ���� �� �ּ� �Ÿ�

    float m_SpDelta = 0.0f;
    List<GameObject> MonList = new List<GameObject>();
    #endregion
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
            m_SpDelta = spawnInterval;

            // ��� ��ġ�� �ϳ��� ����
            for (int i = 0; i < MonPos.Length; i++)
            {
                Vector3 a_SpawnPos = MonPos[i].position;

                if (!IsAliveCheck(a_SpawnPos) && !IsOverlapCheck(a_SpawnPos))
                {
                    int a_Rand = Random.Range(0, MonObj.Length);

                    // MonObj �迭�� 3�� �ε����� �ش��ϴ� ������Ʈ�� ������ �� y������ +5�� ������
                    if (a_Rand == 3)
                    {
                        a_SpawnPos += new Vector3(0, 5, 0);
                    }

                    GameObject a_Mon = Instantiate(MonObj[a_Rand], a_SpawnPos, Quaternion.identity);
                    MonList.Add(a_Mon);
                }
            }
        }

        MonList.RemoveAll(monster => monster == null ||
        (monster.GetComponent<Tree_Ctrl>()?.IsDead ?? false) ||
        (monster.GetComponent<Slime_Ctrl>()?.IsDead ?? false) ||
        (monster.GetComponent<Bush_Ctrl>()?.IsDead ?? false));
    }

    #region ������ġ�� ����ִ��� üũ
    bool IsAliveCheck(Vector3 position)
    {
        foreach (GameObject monster in MonList)
        {
            if (monster != null &&
                Vector3.Distance(monster.transform.position, position) < SpawnCheck)
            {
                if (!(monster.GetComponent<Tree_Ctrl>()?.IsDead ?? true) &&
                    !(monster.GetComponent<Slime_Ctrl>()?.IsDead ?? true) &&
                    !(monster.GetComponent<Bush_Ctrl>()?.IsDead ?? true))
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region ������ġ�� �ٸ� ���Ͱ� �ִ��� üũ
    bool IsOverlapCheck(Vector3 position)
    {
        foreach (GameObject monster in MonList)
        {
            if (monster != null &&
                Vector3.Distance(monster.transform.position, position) < MinDistanceBetweenMonsters)
            {
                return true;
            }
        }
        return false;
    }
    #endregion
}
