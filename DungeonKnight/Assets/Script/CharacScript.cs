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
    protected float max_Hp = 0.0f;      // 캐릭터의 최대 체력
    protected float now_Hp = 0.0f;      // 캐릭터의 현재 체력
    protected float att_Point = 0.0f;   // 캐릭터의 공격력 수치
    protected float def_Point = 0.0f;   // 캐릭터의 방어력 수치
    protected float multi_Damage = 0;   // 멀티 공격을 받을 때 사용하는 변수
    protected float def_Save = 0;       // 방어 패턴 시 피해를 막을 수치 저장 변수
    protected List<EnemyPatt> e_Patten = new List<EnemyPatt>();     // 몬스터의 패턴을 저장하는 리스트
    public int multi_Count = 0;     // 멀티 공격 카운트를 체크하는 변수
    public float att_Up = 1.0f;     // 공격력 증가 수치
    public float def_Up = 1.0f;     // 방어력 증가 수치
    public float att_Down = 0.0f;   // 공격력 감소 수치
    public float def_Down = 0.0f;   // 방어력 감소 수치
    public bool patt_Bool = true;   // 패턴 진행 여부 변수

    protected void HeroSetting()    // 주인공 캐릭터 변수 값 설정 함수
    {
        att_Point = GlobalScript.g_AttState;    // static으로 저장된 변수 값을 받아옴
        def_Point = GlobalScript.g_DefState;
        max_Hp = GlobalScript.g_HealthMax;
        now_Hp = GlobalScript.g_HealthNow;
    }

    protected void EnemyPatten(EnemyType a_Type)    // 몬스터 패턴 설정 함수
    {
        List<EnemyPatt> m_Patten = null;

        switch (a_Type)     // 매개변수로 받아온 몬스터 타입에 따른 능력치 및 패턴 설정
        {
            case EnemyType.N_Ghost:     // 유령 몬스터의 경우
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.Attack, EnemyPatt.AttUp };   // 공격, 공격력 증가 패턴 추가
                    max_Hp = 100.0f;        // 최대 체력 설정
                    att_Point = 10.0f;      // 공격력 설정
                }
                break;
            case EnemyType.N_Thief:     // 도적 몬스터의 경우
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.Attack, EnemyPatt.Defence, EnemyPatt.AttUp };    // 공격, 방어, 공격력 증가 패턴 추가
                    max_Hp = 100.0f;        // 최대 체력 설정
                    att_Point = 10.0f;      // 공격력 설정
                    def_Point = 5.0f;       // 방어력 설정
                }
                break;
            case EnemyType.N_Knight:    // 기사 몬스터의 경우
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.Attack, EnemyPatt.Defence, EnemyPatt.AttUp };    // 공격, 방어, 공격력 증가 패턴 추가
                    max_Hp = 150.0f;        // 최대 체력 설정
                    att_Point = 10.0f;      // 공격력 설정
                    def_Point = 10.0f;       // 방어력 설정
                }
                break;
            case EnemyType.E_Thief:     // 엘리트 도적 몬스터의 경우
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.Attack, EnemyPatt.Defence, EnemyPatt.AttUp,
                                            EnemyPatt.MultiAtt, EnemyPatt.DefDown };    // 공격, 방어, 공격력 증가, 멀티 공격, 방어 디버프 패턴 추가
                    max_Hp = 400.0f;        // 최대 체력 설정
                    att_Point = 30.0f;      // 공격력 설정
                    def_Point = 15.0f;       // 방어력 설정
                }
                break;
            case EnemyType.E_Knight:    // 엘리트 기사 몬스터의 경우
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.Attack, EnemyPatt.Defence, EnemyPatt.ADUp,
                                            EnemyPatt.AttDown, EnemyPatt.Perfect };     // 공격, 방어, 공&방 증가, 공격 디버프, 무적 패턴 추가
                    max_Hp = 500.0f;        // 최대 체력 설정
                    att_Point = 25.0f;      // 공격력 설정
                    def_Point = 20.0f;       // 방어력 설정
                }
                break;
            case EnemyType.B_Monster:   // 보스 몬스터의 경우
                {
                    m_Patten = new List<EnemyPatt> { EnemyPatt.AttDrain, EnemyPatt.Defence, EnemyPatt.ADUp,
                                            EnemyPatt.MultiAtt };       // 흡혈 공격, 방어, 공&방 증가, 멀티 공격 패턴 추가
                    max_Hp = 800.0f;        // 최대 체력 설정
                    att_Point = 40.0f;      // 공격력 설정
                    def_Point = 25.0f;       // 방어력 설정
                }
                break;
        }

        e_Patten = m_Patten;
    }

}