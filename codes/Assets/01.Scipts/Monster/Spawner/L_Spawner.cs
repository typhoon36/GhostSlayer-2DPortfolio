using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_Spawner : MonoBehaviour
{
    #region Spawn Variables
    public GameObject[] MonObj;
    public Transform[] MonPos;
    public float spawnInterval = 3f; // 스폰 간격
    public float SpawnCheck = 10f; // 스폰 위치 확인 반경

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

            // 모든 위치에 하나씩 스폰
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

    #region 스폰위치에 살아있는지 체크
    bool IsAliveCheck(Vector3 position)
    {
        //Foreach로 모든 몬스터를 순회
        foreach (GameObject monster in MonList)
        {
            //몬스터가 있고,거리를 계산하여 스폰체크 반경내에 살아있는 몬스터가 있는지
            if (monster != null &&
                Vector3.Distance(monster.transform.position, position) < SpawnCheck)
            {
                //있다면 컴포넌트 가져오고 죽었다면 true 반환
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
