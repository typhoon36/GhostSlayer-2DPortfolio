using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal_Mgr : MonoBehaviour
{
    //# 포탈을 관리해줄 딕셔너리 변수
    Dictionary<int, Transform> portals = new Dictionary<int, Transform>();
    bool isTeleporting = false;//텔레포트 중인지 확인

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

        //## 포탈이 등록되어있는지 확인
        if (portals.ContainsKey(fromPortal_ID))
        {
            // 등록된 포탈이 17번이거나 18번이면
            if (fromPortal_ID == 17 || fromPortal_ID == 18)
            {
                Portal portal = portals[fromPortal_ID].GetComponent<Portal>();
                if (portal != null)
                {
                    portal.RoomEnter(playerTransform);
                }
                return;
            }

            //## 포탈 이동 처리
            int targetPortalID;
            if (a_NextPortal)
                targetPortalID = (fromPortal_ID + 1) % portals.Count;//다음 포탈로 이동
            else
                targetPortalID = (fromPortal_ID - 1 + portals.Count) % portals.Count;//이전 포탈로 이동

            if (portals.ContainsKey(targetPortalID))//포탈이 등록되어있으면
            {
                playerTransform.position = portals[targetPortalID].position;//플레이어 위치를 다음 포탈 위치 이동
                StartCoroutine(Tp_Cool(a_tpCool));
            }
        }
    }

    //포탈 쿨타임
    IEnumerator Tp_Cool(float a_cool)
    {
        isTeleporting = true;
        yield return new WaitForSeconds(a_cool);
        isTeleporting = false;
    }
}
