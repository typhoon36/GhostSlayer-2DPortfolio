using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public int portalID;
    public int connectedPortalID; // 연결된 포탈의 ID 추가
    [HideInInspector] public float m_tpCool = 1.0f;

    void Start()
    {
        Portal_Mgr.Inst.RegistPortal(portalID, transform);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Portal_Mgr.Inst.TpPlayer(portalID, connectedPortalID, other.transform, m_tpCool);
        }
    }
}
