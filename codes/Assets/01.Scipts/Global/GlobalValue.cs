using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GlobalValue
{
    public static int g_UserGold = 10000;

    public static Vector2 g_SpawnPosition = Vector2.zero; // 스폰 위치 저장 변수 수정

    // 게임 데이터 로드
    public static void LoadGameData()
    {
        g_UserGold = PlayerPrefs.GetInt("UserGold", g_UserGold);

        // 스폰 위치 로드
        g_SpawnPosition = new Vector2(
            PlayerPrefs.GetFloat("SpawnPosX", 0),
            PlayerPrefs.GetFloat("SpawnPosY", 0)
        );
    }

    // 게임 데이터 저장
    public static void SaveGameData()
    {
        PlayerPrefs.SetInt("UserGold", g_UserGold);

        // 스폰 위치 저장
        PlayerPrefs.SetFloat("SpawnPosX", g_SpawnPosition.x);
        PlayerPrefs.SetFloat("SpawnPosY", g_SpawnPosition.y);
        PlayerPrefs.Save();
    }

    // 게임 데이터 초기화
    public static void ResetGameData()
    {
        PlayerPrefs.DeleteAll();
        g_UserGold = 10000;
        g_SpawnPosition = Vector2.zero;
        SaveGameData();
    }

    // 골드가 충분한지 확인
    public static bool HasEnoughGold(int price)
    {
        return g_UserGold >= price;
    }
}
