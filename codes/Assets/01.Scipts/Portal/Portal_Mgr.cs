using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal_Mgr : MonoBehaviour
{
    //# 포탈을 관리해줄 딕셔너리 변수
    Dictionary<int, Transform> portals = new Dictionary<int, Transform>();
    bool isTeleporting = false;//텔레포트 중인지 확인
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

    //# 포탈 등록
    public void RegistPortal(int id, Transform portalTransform)
    {
        if (!portals.ContainsKey(id))
        {
            portals.Add(id, portalTransform);
        }
    }

    //#포탈 이동
    public void TpPlayer(int fromPortal_ID, Transform playerTransform,
        float a_tpCool, bool a_NextPortal = true)
    {
        if (isTeleporting) return;
        //텔레포트 중이면 리턴

        this.playerTransform = playerTransform; // 플레이어의 Transform 저장

        //## 포탈이 등록되어있는지 확인
        if (portals.ContainsKey(fromPortal_ID))
        {
            //## 포탈 이동 처리
            int targetPortalID;
            if (a_NextPortal)
                targetPortalID = (fromPortal_ID + 1) % portals.Count;//다음 포탈로 이동
            else
                targetPortalID = (fromPortal_ID - 1 + portals.Count) % portals.Count;//이전 포탈로 이동

            if (portals.ContainsKey(targetPortalID))//포탈이 등록되어있으면
            {
                playerTransform.position = portals[targetPortalID].position;//플레이어 위치를 다음 포탈 위치 이동
                GlobalValue.g_SpawnPosition = playerTransform.position;
                GlobalValue.SaveGameData();
                StartCoroutine(Tp_Cool(a_tpCool));
            }
        }
    }

    // 플레이어의 Transform 반환
    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    //포탈 쿨타임
    IEnumerator Tp_Cool(float a_cool)
    {
        isTeleporting = true;
        yield return new WaitForSeconds(a_cool);
        isTeleporting = false;
    }
}
