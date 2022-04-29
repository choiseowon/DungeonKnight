using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HeroScript : CharactorClass, IAttack, IDefence, IAllAttack, IMultiAtt, IAttDef, IAttUp, IDamage
{   // 모든 캐릭터가 공유하는 변수와 함수를 가진 클래스와 카드 종류에 따른 인터페이스 추가
    public static HeroScript Inst = null;
    public bool die_Bool = false;

    public float att_Point { get; set; }
    public float att_Up { get; set; }
    public float def_Point { get; set; }
    public float def_Save { get; set; }

    public float att_Down = 0.0f;
    public float def_Down = 0.0f;

    public float att_Item = 0.0f;
    public float def_Item = 0.0f;

    public Queue<string> patt_Pool = new Queue<string>();
    public Queue<GameObject> target_Pool = new Queue<GameObject>();
    GameObject target_Obj = null;

    public bool patt_Bool = false;

    void Awake()
    {
        Inst = this;
        att_Point = GlobalScript.g_AttState;    // static으로 저장된 변수 값을 받아옴
        def_Point = GlobalScript.g_DefState;
        def_Save = 0;
        att_Up = 1.0f;
        max_Hp = GlobalScript.g_HealthMax;
        now_Hp = GlobalScript.g_HealthNow;

        att_Item = GlobalScript.g_AttPuls;
        def_Item = GlobalScript.g_DefPuls;

        anim_Runtime = anim.runtimeAnimatorController;

        hp_Ui.HpSetting(max_Hp, now_Hp);
        patt_Bool = false;
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
    }

    public void PattAdd(string patt_Str)    // 패턴 추가 함수
    {
        patt_Pool.Enqueue(patt_Str);    // 패턴 Queue에 문자열을 추가

        if (patt_Bool == false)     // 패턴이 진행 중이 아나라면 패턴 호출
            PattCall();
    }

    public override void PattCall()     // 패턴 호출 함수
    {
        patt_Bool = true;   // 패턴이 실행 중이라고 bool 변수 변경
        patt_Str = patt_Pool.Dequeue();     // 패턴 Queue에서 가장 오래전에 추가된 패턴을 가져옴
        this.GetType().GetMethod(patt_Str).Invoke(this, null);  // 해당 문자열과 같은 이름의 함수 호출
    }

    public void PattEnd()   // 패턴이 끝날을 때 호출될 함수
    {
        target_Obj = null;      // 공격 타겟을 제거

        if (patt_Pool.Count > 0)    // 패턴이 남아 있는지 체크
            PattCall();     // 남은 패턴이 있으면 실행
        else
            patt_Bool = false;  // 없다면 패턴 체크용 bool 변수 변경

    }

    IEnumerator pattDelay_Co = null;    // 코루틴을 저장할 객체 생성

    IEnumerator PattDelayCo()   // 패턴의 딜레이 시간을 주기 위한 코루틴
    {
        yield return new WaitForSeconds(0.5f);  // 1초 대기

        pattDelay_Co = null;    // 저장한 코루틴 제거
        TurnEnd();      // 턴 종료 함수 호출
    }

    public void TurnEnd()   // 턴 종료 함수
    {
        patt_Bool = false;

        if (pattDelay_Co != null)
            return;

        for (int ii = 0; ii < StageScript.Inst.enemy_List.Count; ii++)
        {
            MonsterClass monClass = StageScript.Inst.enemy_List[ii].GetComponent<MonsterClass>();
            if (monClass.monState == MonState.Damage || monClass.monState == MonState.Death)
            {
                pattDelay_Co = PattDelayCo();
                StartCoroutine(pattDelay_Co);
                return;
            }
        }

        StageScript.Inst.EnemyPattCall();
    }

    public void Attack()    // 공격 패턴일 경우 호출될 함수
    {
        if (target_Pool.Count <= 0)     // 공격 타겟이 있는지 체크
            return;

        anim.SetTrigger("Attack");      // 공격 애니메이션 재생
        SoundScript.Inst.SfSoundPlay("Attack");        // 사운드 재생

        float att_Damage = Mathf.Floor((att_Point + att_Item) * (att_Up - att_Down));      // 최종 대미지는 공격력과 버프를 계산하여 수치를 설정
        float time = GetTime(anim_Runtime, "Attack");   // 공격 애니메이션의 길이를 구함
        target_Obj = target_Pool.Dequeue();     // 가장 처음 공격 타겟으로 설정된 몬스터를 타겟으로 지정

        StartCoroutine(AttackCo(time, att_Damage, target_Obj));     // 특정 타이밍에 함수를 호출하기 위해 코루틴 호출
    }

    public void AllAttack()
    {
        anim.SetTrigger("Attack");
        SoundScript.Inst.SfSoundPlay("Attack");        // 사운드 재생

        float att_Damage = Mathf.Floor(((att_Point * 0.5f) + att_Item) * (att_Up - att_Down));      // 최종 대미지는 공격력과 버프를 계산하여 수치를 설정
        float time = GetTime(anim_Runtime, "Attack");

        foreach(GameObject obj in StageScript.Inst.enemy_List)
        {
            if(obj.GetComponent<MonsterClass>().monState != MonState.Death)
                StartCoroutine(AttackCo(time, att_Damage, obj));
        }
    }

    public void MultiAtt()
    {
        if (target_Pool.Count <= 0)
            return;

        anim.SetTrigger("Attack");
        SoundScript.Inst.SfSoundPlay("Attack");        // 사운드 재생

        float att_Damage = Mathf.Floor(((att_Point * 0.3f) + att_Item) * (att_Up - att_Down));      // 최종 대미지는 공격력과 버프를 계산하여 수치를 설정
        float time = GetTime(anim_Runtime, "Attack"); 
        target_Obj = target_Pool.Dequeue();

        StartCoroutine(MultiAttCo(time, att_Damage, target_Obj));
    }

    public void AttDef()
    {
        if (target_Pool.Count <= 0)
            return;

        anim.SetTrigger("Attack");
        SoundScript.Inst.SfSoundPlay("Attack");        // 사운드 재생

        float att_Damage = Mathf.Floor(((att_Point * 0.7f) + att_Item) * (att_Up - att_Down));      // 최종 대미지는 공격력과 버프를 계산하여 수치를 설정
        float time = GetTime(anim_Runtime, "Attack");
        target_Obj = target_Pool.Dequeue();

        StartCoroutine(AttDefCo(time, att_Damage, target_Obj));

        def_Save += Mathf.Floor((def_Point + def_Item) * 0.7f * (1.0f - def_Down));
        eff_Root.EffectSetting(EffectType.Defence, this.gameObject);
        SoundScript.Inst.SfSoundPlay("Defence");
        eff_Runtime = eff_Root.eff_Runtime;
        BuffCheck(BuffType.Defence, def_Save);

        if (eff_Runtime != null)
        {
            time = GetTime(eff_Runtime, "Defence");
            StartCoroutine(EffectCo(time));
        }
    }

    IEnumerator AttackCo(float time, float a_Damage, GameObject target)     // 시간과 피해량, 공격 대상을 매개변수로 받아옴
    {
        yield return new WaitForSeconds(time * 0.65f);      // 받아온 애니메이션 시간의 일정 위치까지 대기
        target.GetComponent<IDamage>().Damage(a_Damage);    // 타겟의 피해량을 매개변수로 대미지를 받은 함수 호출
        yield return new WaitForSeconds(time * 0.35f);      // 일정 시간 대기
        PattEnd();      // 패턴 종료 함수 호출
    }

    IEnumerator MultiAttCo(float time, float a_Damage, GameObject target)
    {
        yield return new WaitForSeconds(time * 0.65f);
        for(int ii = 0; ii < 3; ii++)
        {
            target.GetComponent<IDamage>().Damage(a_Damage);
            yield return new WaitForSeconds(time * 0.15f);
        }
        yield return new WaitForSeconds(time * 0.35f);
        PattEnd();
    }

    IEnumerator AttDefCo(float time, float a_Damage, GameObject target)
    {
        yield return new WaitForSeconds(time * 0.65f);
        target.GetComponent<IDamage>().Damage(a_Damage);
        yield return new WaitForSeconds(time * 0.35f);
    }

    public void Defence()   // 방어 패턴일 경우 호출될 함수
    {
        def_Save += Mathf.Floor((def_Point + def_Item) * (1.0f - def_Down));    // 방어막을 저장
        eff_Root.EffectSetting(EffectType.Defence, this.gameObject);    // 방어 이펙트를 생성
        SoundScript.Inst.SfSoundPlay("Defence");    // 사운드 호출
        eff_Runtime = eff_Root.eff_Runtime;     // 이펙트의 런타임을 받아옴
        BuffCheck(BuffType.Defence, def_Save);      // 버프를 체크하는 함수 호출

        if (eff_Runtime != null)
        {
            float time = GetTime(eff_Runtime, "Defence");   // 해당 이펙트의 애니메이션 길이를 받아옴
            StartCoroutine(EffectCo(time));     // 받아온 길이를 이용하여 코루틴 호출
        }
    }

    public void AttUp()
    {
        att_Up = 1.2f;
        eff_Root.EffectSetting(EffectType.AttUp, this.gameObject);
        SoundScript.Inst.SfSoundPlay("PowerUp");
        eff_Runtime = eff_Root.eff_Runtime;
        BuffCheck(BuffType.AttUp, 2.0f);

        if (eff_Runtime != null)
        {
            float time = GetTime(eff_Runtime, "AttUp");
            StartCoroutine(EffectCo(time));
        }
    }

    IEnumerator EffectCo(float time)    // 이펙트가 끝나는 걸 확인하기 위한 코루틴
    {
        yield return new WaitForSeconds(time);      // 매개변수로 받아온 시간 만큼 대기
        PattEnd();      // 패턴 종료 함수 호출
    }

    public void BuffCheck(BuffType buff_Type, float buff_Value)     // 버프를 체크하는 함수
    {
        if(buff_ValueDict.ContainsKey(buff_Type) == true)   // 버프 딕셔너리에 해당 타입이 키값으로 저장되어 있는지 확인
        {
            if (buff_Value <= 0.0f)     // 매개변수로 넘어온 float 값이 0보다 작거나 같다면
                buff_ValueDict.Remove(buff_Type);   // 해당 버프 삭제
            else
                buff_ValueDict[buff_Type] = buff_Value;     // 해당 버프의 밸류값 수정

            BuffView();     // 버프를 표시해주는 함수 호출

        }
        else
        {
            buff_ValueDict.Add(buff_Type, buff_Value);      // 해당 버프를 딕셔너리에 새로 추가
            GameObject obj = InstantiateFunc(buff_Root.transform, buff_Obj);    // 새로 추가된 버프를 버프창에 표시하기 위한 오브젝트 생성
            obj.GetComponent<BuffScript>().BuffSetting(buff_Type, (int)buff_Value);     // 해당 오브젝트를 셋팅하는 함수 호출
        }

        CardCtrlScript.Inst.CardReSetting();    // 카드의 텍스트를 버프가 계산된 값으로 수정하기 위해 갱신
    }

    GameObject InstantiateFunc(Transform a_Parent, GameObject a_Prefab)     // 인스턴스할 경우 호출하는 함수
    {
        GameObject obj = Instantiate(a_Prefab);     // 오브젝트 인스턴스
        obj.transform.SetParent(a_Parent);      // 매개변수로 넘어온 부모 설정
        obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);   // 사이즈를 변경

        return obj;
    }

    public void BuffCount()     // 한 턴이 지나 버프를 감소하기 위한 함수
    {
        foreach (BuffType m_Type in buff_ValueDict.Keys.ToList())   // 딕셔너리에 저장된 키를 리스트로 하여 반복
        {
            buff_ValueDict[m_Type] = buff_ValueDict[m_Type] - 1;    // 모든 키값의 밸류 값을 1 감소 시킴
        }

        buff_ValueDict.Remove(BuffType.Defence);    // 방어막의 경우 한 턴이 지나면 모두 삭제

        BuffView();     // 버프를 표시해주는 함수 호출
        CardCtrlScript.Inst.CardReSetting();    // 카드의 텍스트를 버프가 계산된 값으로 수정하기 위해 갱신
    }

    void BuffView()
    {
        BuffScript[] buff_Array = buff_Root.GetComponentsInChildren<BuffScript>();  // 버프 표시창에 있는 모든 버프의 스크립트를 배열로 저장

        foreach(BuffScript buff in buff_Array)  // 저장된 배열 만큼 반복
        {
            if(buff.buffType == BuffType.Defence && buff_ValueDict.ContainsKey(BuffType.Defence) != true)
            {   // 방어막의 경우
                def_Save = 0;   // 저장된 방어 수치 제거
                Destroy(buff.gameObject);   // 방어막 표시 제거
            }

            buff_ValueDict.TryGetValue(buff.buffType, out float count);     // 버프의 밸류값을 해당 변수에 저장
            buff.buff_Count = (int)count;   // 딕셔너리에서 가져온 밸류값을 해당 버프의 카운트 변수에 저장

            if (buff.buff_Count <= 0)   // 해당 변수의 값이 0보다 작거나 같을 경우
            {
                switch (buff.buffType)  // 버프 타입에 따라 다른 내용 실행
                {
                    case BuffType.AttUp:    // 공격력 증가 버프일 경우
                        att_Up = 1.0f;      // 공격력 증가 수치 기존 값으로 변경
                        break;
                    case BuffType.DefDown:  // 방어력 감소 디버프일 경우
                        def_Down = 0.0f;    // 방어력 감소 수치 기존 값으로 변경
                        break;
                    case BuffType.AttDown:  // 공격력 감소 디버프일 경우
                        att_Down = 0.0f;    // 공격력 감소 수치 기존 값으로 변경
                        break;
                }

                buff_ValueDict.Remove(buff.buffType);   // 딕셔너리에서 해당 버프 삭제
                Destroy(buff.gameObject);       // 버프 표시 오브젝트 제거
            }
            else
            {
                buff.BuffSetting(buff.buffType, buff.buff_Count);   // 버프 표시 오브젝트를 새로 셋팅하는 함수 호출
            }
        }
    }

    float GetTime(RuntimeAnimatorController a_RunTime, string ani_Str)  // 애니메이션의 길이를 반환하는 함수
    {
        float time = 0.0f;

        for (int ii = 0; ii < a_RunTime.animationClips.Length; ii++)    // 런타임에 있는 모든 애니메이션 클립들 만큼 반복
            if (a_RunTime.animationClips[ii].name.Contains(ani_Str))    // 특정 문자열의 애니메이션인지 확인
            {
                time = a_RunTime.animationClips[ii].length;     // 그 애니메이션의 길이를 변수에 저장
            }

        return time;    // 저장된 길이 반환
    }

    IEnumerator damage_Co = null;

    public void Damage(float a_Damage)
    {
        float m_Damage = a_Damage;

        if (def_Save > 0)       // 저장된 방어력이 있는지 체크
        {
            m_Damage = a_Damage - def_Save;     // 저장된 방어력만큼 피해 감소
            def_Save -= a_Damage;       // 저장된 방어력 감소

            if (m_Damage <= 0)      // 피해가 0 보다 작을 경우
                m_Damage = 0;       // 0 으로 변경

            if (def_Save <= 0)      // 저장된 방어력이 0 이하가 될 경우
                def_Save = 0;       // 0 으로 변경

            BuffCheck(BuffType.Defence, def_Save);     // 방어력 초기화
        }

        now_Hp -= m_Damage;     // 대미지 수치 만큼 현재 체력 감소
        if (now_Hp <= 0)        // 현재 체력이 0보다 작거나 같을 경우
            now_Hp = 0;         // 0으로 설정

        hp_Ui.HpSetting(max_Hp, now_Hp);
        string m_Str = "";

        if (m_Damage > 0)       // 피해량이 0 보다 크다면
        {
            m_Str = "- " + Mathf.Floor(m_Damage).ToString();     // 피해량(내림)을 문자열로 저장
            eff_Root.TextSetting(m_Str, new Color32(255, 50, 50, 255), Color.white);
            if (damage_Co == null)
                anim.SetTrigger("Damage");
            // 이펙트를 텍스트로 설정. 매개변수로 문자열과 기본색상, 테두리 색상 값을 넘겨줌
            SoundScript.Inst.SfSoundPlay("Damage");        // 사운드 재생
        }
        else
        {
            m_Str = "방어함";       // 피해량이 0 이면 해당 문자열 저장
            eff_Root.TextSetting(m_Str, new Color32(255, 255, 255, 255), Color.black);
            // 이펙트를 텍스트로 설정. 매개변수로 문자열과 기본색상, 테두리 색상 값을 넘겨줌 
            SoundScript.Inst.SfSoundPlay("Guard");     // 사운드 재생
        }

        if(now_Hp <= 0)
            die_Bool = true;

        float time = GetTime(anim_Runtime, "Damage");

        if (damage_Co != null)
            return;

        GlobalScript.g_HealthNow = now_Hp;
        damage_Co = DamageCo(time);
        StartCoroutine(damage_Co);
    }

    IEnumerator DamageCo(float time)
    {
        yield return new WaitForSeconds(time);

        if (now_Hp <= 0.0f)     // 현제 체력이 0 이거나 그보다 작을 경우
        {
            float death_time = GetTime(anim_Runtime, "Death");
            StartCoroutine(DeathCo(death_time));
        }

        damage_Co = null;
    }

    IEnumerator DeathCo(float time)
    {
        anim.SetTrigger("Death");
        SoundScript.Inst.SfSoundPlay("Death");

        yield return new WaitForSeconds(time * 0.95f);

        StageScript.Inst.dead_Obj.SetActive(true);
        SoundScript.Inst.BgmSoundPlay("GameOver_Bgm");
        SoundScript.Inst.bgm_Audio.loop = false;

        Destroy(this.gameObject);
    }
}