using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public int portalID;
    [HideInInspector] public float m_tpCool = 1.0f;

    void Start()
    {
        Portal_Mgr.Inst.RegistPortal(portalID, transform);
    }

}
