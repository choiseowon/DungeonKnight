using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GlobalScript
{
    #region
    [Header("----- Game Size -----")]
    public static float g_Width = Screen.width;
    public static float g_Height = Screen.height;

    [Header("----- Hero State -----")]
    public static int g_AttState = 20;
    public static int g_AttPuls = 0;
    public static int g_DefState = 10;
    public static int g_DefPuls = 0;
    public static float g_HealthMax = 200;
    public static float g_HealthNow = g_HealthMax;
    public static int g_ManaMax = 3;
    public static int g_Gold = 300;

    [Header("----- Shop -----")]
    public static int g_ShopWeapon = 100;
    public static int g_ShopShield = 100;
    public static int g_ShopArmor = 100;

    [Header("----- Map Create -----")]
    public static bool g_MapBool = false;
    public static Canvas g_SelecCanvas = null;
    public static int g_EnemyLev = 1;

    public static StageType g_StageType = StageType.Count;

    [Header("----- Sound -----")]
    public static float bgm_Volume = 1.0f;
    public static float sf_Volume = 1.0f;
    #endregion

    [Header("----- Card -----")]
    // 카드 종류 리스트
    public static List<CardType> g_CardValue = new List<CardType>
        { CardType.Attack, CardType.Defence, CardType.AllAttack, CardType.MultiAtt, CardType.AttDef, CardType.AttUp }; 

    // 플레이어 카드 리스트
    public static List<CardType> g_CardDec = new List<CardType>
        { CardType.Attack, CardType.Attack, CardType.Attack, CardType.Defence,
        CardType.Defence, CardType.Defence, CardType.AllAttack, CardType.MultiAtt, CardType.AttDef, CardType.AttUp };

    #region
    public static void ValueReset()
    {
        #region
        g_Width = Screen.width;
        g_Height = Screen.height;

        g_AttState = 20;
        g_AttPuls = 0;
        g_DefState = 10;
        g_DefPuls = 0;
        g_HealthMax = 200;
        g_HealthNow = g_HealthMax;
        g_ManaMax = 3;
        g_Gold = 300;

        g_ShopWeapon = 100;
        g_ShopShield = 100;
        g_ShopArmor = 100;
        g_MapBool = false;
        g_SelecCanvas = null;

        g_EnemyLev = 1;
        #endregion

        // 메인화면으로 돌아가면 호출되는 함수 카드 리스트를 초기화함
        g_CardValue.Clear();
        g_CardDec.Clear();

        g_CardValue = new List<CardType>
            { CardType.Attack, CardType.Defence, CardType.AllAttack, CardType.MultiAtt, CardType.AttDef, CardType.AttUp };

        g_CardDec = new List<CardType>
             { CardType.Attack, CardType.Attack, CardType.Attack, CardType.Defence,
                CardType.Defence, CardType.Defence, CardType.AllAttack, CardType.MultiAtt, CardType.AttDef, CardType.AttUp };


    }
    #endregion
}
