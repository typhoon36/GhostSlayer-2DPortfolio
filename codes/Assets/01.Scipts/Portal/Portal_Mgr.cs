using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal_Mgr : MonoBehaviour
{
    //# ��Ż�� �������� ��ųʸ� ����
    Dictionary<int, Transform> portals = new Dictionary<int, Transform>();
    bool isTeleporting = false;//�ڷ���Ʈ ������ Ȯ��
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

    //# ��Ż ���
    public void RegistPortal(int id, Transform portalTransform)
    {
        if (!portals.ContainsKey(id))
        {
            portals.Add(id, portalTransform);
        }
    }

    //#��Ż �̵�
    public void TpPlayer(int fromPortal_ID, Transform playerTransform,
        float a_tpCool, bool a_NextPortal = true)
    {
        if (isTeleporting) return;
        //�ڷ���Ʈ ���̸� ����

        this.playerTransform = playerTransform; // �÷��̾��� Transform ����

        //## ��Ż�� ��ϵǾ��ִ��� Ȯ��
        if (portals.ContainsKey(fromPortal_ID))
        {
            //## ��Ż �̵� ó��
            int targetPortalID;
            if (a_NextPortal)
                targetPortalID = (fromPortal_ID + 1) % portals.Count;//���� ��Ż�� �̵�
            else
                targetPortalID = (fromPortal_ID - 1 + portals.Count) % portals.Count;//���� ��Ż�� �̵�

            if (portals.ContainsKey(targetPortalID))//��Ż�� ��ϵǾ�������
            {
                playerTransform.position = portals[targetPortalID].position;//�÷��̾� ��ġ�� ���� ��Ż ��ġ �̵�
                GlobalValue.g_SpawnPosition = playerTransform.position;
                GlobalValue.SaveGameData();
                StartCoroutine(Tp_Cool(a_tpCool));
            }
        }
    }

    // �÷��̾��� Transform ��ȯ
    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    //��Ż ��Ÿ��
    IEnumerator Tp_Cool(float a_cool)
    {
        isTeleporting = true;
        yield return new WaitForSeconds(a_cool);
        isTeleporting = false;
    }
}
