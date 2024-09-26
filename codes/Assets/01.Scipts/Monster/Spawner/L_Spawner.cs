using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_Spawner : MonoBehaviour
{
    #region Spawn Variables
    public GameObject[] MonObj;
    public Transform[] MonPos;
    public float spawnInterval = 3f; // ���� ����
    public float SpawnCheck = 10f; // ���� ��ġ Ȯ�� �ݰ�

    float m_SpDelta = 0.0f;
    List<GameObject> MonList = new List<GameObject>();
    #endregion

    #region #Singleton Pattern

    public static L_Spawner Inst = null;
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

                if (!IsAliveCheck(a_SpawnPos))
                {
                    int a_Rand = Random.Range(0, MonObj.Length);
                    GameObject a_Mon = Instantiate(MonObj[a_Rand], a_SpawnPos,
                        Quaternion.identity);
                    MonList.Add(a_Mon);
                }
            }
        }


        MonList.RemoveAll(monster => monster == null ||
        (monster.GetComponent<Drone_Ctrl>()?.IsDead ?? false));
    }

    #region ������ġ�� ����ִ��� üũ
    bool IsAliveCheck(Vector3 position)
    {
        //Foreach�� ��� ���͸� ��ȸ
        foreach (GameObject monster in MonList)
        {
            //���Ͱ� �ְ�,�Ÿ��� ����Ͽ� ����üũ �ݰ泻�� ����ִ� ���Ͱ� �ִ���
            if (monster != null &&
                Vector3.Distance(monster.transform.position, position) < SpawnCheck)
            {
                //�ִٸ� ������Ʈ �������� �׾��ٸ� true ��ȯ
                Drone_Ctrl a_Obj = monster.GetComponent<Drone_Ctrl>();
                if (a_Obj != null && !a_Obj.IsDead)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion
}
