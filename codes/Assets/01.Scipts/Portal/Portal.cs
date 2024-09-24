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


    //# 포탈을 타고 들어갔을때 위치변경
    public void RoomEnter(Transform playerTransform)
    {
        if (portalID == 18)
        {
            SceneManager.sceneLoaded += (scene, mode) =>
            OnSceneLoaded(scene, mode, playerTransform,
            new Vector3(-132.1f, -23.1f, 0));
            SceneManager.LoadScene("Lab");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode,
        Transform playerTransform,
        Vector3 position)
    {
        if (playerTransform != null)
            playerTransform.position = position;


        SceneManager.sceneLoaded -= (s, m) =>
        OnSceneLoaded(s, m, playerTransform, position);
    }
}
