using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal_Mgr : MonoBehaviour
{
    Dictionary<int, Transform> portals = new Dictionary<int, Transform>();
    bool isTeleporting = false;
    Transform playerTransform;

    #region Singleton
    public static Portal_Mgr Inst;

    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
        }
    }
    #endregion

    public void RegistPortal(int id, Transform portalTransform)
    {
        if (!portals.ContainsKey(id))
        {
            portals.Add(id, portalTransform);
        }
    }

    public void TpPlayer(int fromPortalID, int toPortalID, Transform playerTransform, float tpCool)
    {
        if (isTeleporting) return;

        this.playerTransform = playerTransform;

        if (portals.ContainsKey(toPortalID))
        {
            playerTransform.position = portals[toPortalID].position;
            GlobalValue.g_SpawnPosition = playerTransform.position;
            GlobalValue.SaveGameData();
            StartCoroutine(Tp_Cool(tpCool));
        }
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    IEnumerator Tp_Cool(float a_cool)
    {
        isTeleporting = true;
        yield return new WaitForSeconds(a_cool);
        isTeleporting = false;
    }
}
