using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_Spawner : MonoBehaviour
{
    public GameObject[] MonObj;
    public Transform[] MonPos;
    public float spawnInterval = 3f; // ���� ����
    public float spawnCheckRadius = 10f; // ���� ��ġ Ȯ�� �ݰ�

    private float m_SpDelta = 0.0f;
    private List<GameObject> spawnedMonsters = new List<GameObject>();

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
                Vector3 spawnPosition = MonPos[i].position;

                if (!IsPositionOccupiedOrAlive(spawnPosition))
                {
                    int RandMon = Random.Range(0, MonObj.Length);
                    GameObject Mon = Instantiate(MonObj[RandMon], spawnPosition, Quaternion.identity);
                    spawnedMonsters.Add(Mon);
                }
            }
        }

        // Clean up the list by removing destroyed monsters
        spawnedMonsters.RemoveAll(monster => monster == null ||
        (monster.GetComponent<Drone_Ctrl>()?.IsDead ?? false));
    }

    bool IsPositionOccupiedOrAlive(Vector3 position)
    {
        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster != null && Vector3.Distance(monster.transform.position, position) < spawnCheckRadius)
            {
                Drone_Ctrl droneCtrl = monster.GetComponent<Drone_Ctrl>();
                if (droneCtrl != null && !droneCtrl.IsDead)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
