using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Spawner : MonoBehaviour
{
    #region Spawn Variables
    public GameObject[] MonObj;
    public Transform[] MonPos;
    public float spawnInterval = 8f; // 스폰 간격
    public float SpawnCheck = 10f; // 스폰 위치 확인 반경
    public float MinDistanceBetweenMonsters = 20f; // 몬스터 간 최소 거리

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

            // 모든 위치에 하나씩 스폰
            for (int i = 0; i < MonPos.Length; i++)
            {
                Vector3 a_SpawnPos = MonPos[i].position;

                if (!IsAliveCheck(a_SpawnPos) && !IsOverlapCheck(a_SpawnPos))
                {
                    int a_Rand = Random.Range(0, MonObj.Length);

                    // MonObj 배열의 3번 인덱스에 해당하는 오브젝트를 스폰할 때 y축으로 +5를 더해줌
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

    #region 스폰위치에 살아있는지 체크
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

    #region 스폰위치에 다른 몬스터가 있는지 체크
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
