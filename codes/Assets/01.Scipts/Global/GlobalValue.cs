using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GlobalValue
{
    public static int g_UserGold = 10000;
    public static float g_MaxHP = 100f; // 최대 체력
    public static float g_CurHP = 100f; // 현재 체력

    public static Vector2 g_SpawnPosition = Vector2.zero; // 스폰 위치 초기화

    // 게임 데이터 로드
    public static void LoadGameData()
    {
        g_UserGold = PlayerPrefs.GetInt("UserGold", g_UserGold);
        g_MaxHP = PlayerPrefs.GetFloat("MaxHP", g_MaxHP);
        g_CurHP = PlayerPrefs.GetFloat("CurHP", g_CurHP);

        if (g_CurHP > g_MaxHP)
        {
            g_CurHP = g_MaxHP;
        }

        if (g_CurHP < 0)
        {
            g_CurHP = 0;
        }

        g_SpawnPosition = new Vector2(PlayerPrefs.GetFloat("SpawnPosX", 0),
            PlayerPrefs.GetFloat("SpawnPosY", 0));
    }

    public static void SaveGameData()
    {
        PlayerPrefs.SetInt("UserGold", g_UserGold);
        PlayerPrefs.SetFloat("MaxHP", g_MaxHP);
        PlayerPrefs.SetFloat("CurHP", g_CurHP);

        PlayerPrefs.SetFloat("SpawnPosX", g_SpawnPosition.x);
        PlayerPrefs.SetFloat("SpawnPosY", g_SpawnPosition.y);
        PlayerPrefs.Save();
    }

    // 게임 데이터 초기화
    public static void ResetGameData()
    {
        PlayerPrefs.DeleteAll();
        g_UserGold = 10000;
        g_MaxHP = 100f;
        g_CurHP = 100f;
        g_SpawnPosition = Vector2.zero;
        SaveGameData();
    }

    // 충분한 골드가 있는지 확인
    public static bool HasEnoughGold(int price)
    {
        return g_UserGold >= price;
    }
}
