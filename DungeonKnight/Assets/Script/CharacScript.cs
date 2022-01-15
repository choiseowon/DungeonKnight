using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyPatt
{
    Attack,
    Defence,
    AttUp,
    MultiAtt,
    DefDown,
    AttDown,
    ADUp,
    Perfect,
    AttDrain,
    Summon,
    Count
}

public enum EnemyClip
{
    Idle,
    Attack,
    Damage,
    Death,
    Count
}

public enum EnemyType
{
    N_Ghost,
    N_Thief,
    N_Knight,
    E_Thief,
    E_Knight,
    B_Monster,
    Count
}

public class CharacScript : MonoBehaviour
{
    [Header("All Setting Value")]
    protected float max_Hp = 0.0f;
    protected float now_Hp = 0.0f;
    protected float att_Point = 0.0f;
    protected float def_Point = 0.0f;
    protected float multi_Damage = 0;
    protected float def_Save = 0;
    protected List<EnemyPatt> e_Patten = new List<EnemyPatt>();
    public int multi_Count = 0;
    public float att_Up = 1.0f;
    public float def_Up = 1.0f;
    public float att_Down = 0.0f;
    public float def_Down = 0.0f;
    public bool patt_Bool = true;

    protected void HeroSetting()
    {
        att_Point = GlobalScript.g_AttState;
        def_Point = GlobalScript.g_DefState;
        max_Hp = GlobalScript.g_HealthMax;
        now_Hp = GlobalScript.g_HealthNow;
    }

    protected void EnemyPatten(EnemyType a_Type)
    {
        List<EnemyPatt> m_Patten = null;

        switch (a_Type)
        {
            case EnemyType.N_Ghost:
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.Attack, EnemyPatt.AttUp };
                    max_Hp = 100.0f;
                    att_Point = 10.0f;
                }
                break;
            case EnemyType.N_Thief:
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.Attack, EnemyPatt.Defence, EnemyPatt.AttUp };
                    max_Hp = 100.0f;
                    att_Point = 10.0f;
                    def_Point = 5.0f;
                }
                break;
            case EnemyType.N_Knight:
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.Attack, EnemyPatt.Defence, EnemyPatt.AttUp };
                    max_Hp = 150.0f;
                    att_Point = 10.0f;
                    def_Point = 10.0f;
                }
                break;
            case EnemyType.E_Thief:
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.Attack, EnemyPatt.Defence, EnemyPatt.AttUp,
                                            EnemyPatt.MultiAtt, EnemyPatt.DefDown };
                    max_Hp = 400.0f;
                    att_Point = 30.0f;
                    def_Point = 15.0f;
                }
                break;
            case EnemyType.E_Knight:
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.Attack, EnemyPatt.Defence, EnemyPatt.ADUp,
                                            EnemyPatt.AttDown, EnemyPatt.Perfect };
                    max_Hp = 500.0f;
                    att_Point = 25.0f;
                    def_Point = 20.0f;
                }
                break;
            case EnemyType.B_Monster:
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.AttDrain, EnemyPatt.Defence, EnemyPatt.ADUp,
                                            EnemyPatt.MultiAtt };
                    max_Hp = 800.0f;
                    att_Point = 40.0f;
                    def_Point = 25.0f;
                }
                break;
        }

        e_Patten = m_Patten;
    }

}