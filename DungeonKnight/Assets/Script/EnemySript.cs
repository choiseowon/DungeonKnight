using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySript : CharacScript
{
    [Header("E_KNight Value")]
    public bool per_Bool = false;

    [Header("B_Monster Value")]
    protected GameObject[] summon_Obj = new GameObject[2];
    public int drain_Point = 0;

    [Header("Animation")]
    public Animator anim = null;
    public AnimationClip[] anim_Clip = null;

    [Header("Enemy UI")]
    public Image hp_Img = null;
    public Text hp_Text = null;
    public GameObject buff_Root = null;
    public GameObject patt_Obj = null;
    public Button target_Btn = null;

    [Header("Enemy Type")]
    public EnemyType enemyType = EnemyType.Count;
    EnemyPatt patt_Now = EnemyPatt.Count;

    void Start()
    {
        EnemyPatten(enemyType);     // 몬스터 타입이 따른 능력치 및 패턴 설정

        if (target_Btn != null)
        {
            target_Btn.onClick.AddListener(() =>
            {
                if (HeroScript.Inst.target_Check != true)
                    return;

                CardCtrlScript.Inst.CardRemove(CardCtrlScript.Inst.click_Card);
                HeroScript.Inst.m_Target = this.gameObject;
                HeroScript.Inst.AttackCall();
            });
        }

        now_Hp = max_Hp;        // 현재 체력과 최대 체력이 동일하도록 설정

        if (hp_Img != null)
            hp_Img.fillAmount = now_Hp / max_Hp;        // 체력 이미지를 해당 비율에 맞게 설정

        if (hp_Text != null)
            hp_Text.text = (int)now_Hp + " / " + (int)max_Hp;       // 체력 텍스트 표기

        PattObjSetting();       // 패턴 표시용 오브젝트 생성
    }

    public void PatternCall()       // 패턴 호출 함수
    {
        if (HeroScript.Inst == null)        // HeroScript가 존재하는 경우에만 함수 실행
            return;

        def_Save = 0;               // 남아있던 방어막을 0으로 만든
        DefenceReset();             // 방어막 표시 초기화
        patt_Obj.SetActive(false);  // 패턴 표시 끄기

        // patt_Now의 값에 따라 다른 함수 호출
        if(patt_Now == EnemyPatt.Attack || patt_Now == EnemyPatt.MultiAtt || patt_Now == EnemyPatt.AttDrain)    // 공격 종류의 패턴
        {
            AttackCall();
        }
        else if(patt_Now == EnemyPatt.Defence)  // 방어 패턴
        {
            DefenceCall();
        }
        else if(patt_Now == EnemyPatt.AttUp || patt_Now == EnemyPatt.ADUp || patt_Now == EnemyPatt.Perfect)     // 버프형 패턴
        {
            BuffCall();
        }
        else if(patt_Now == EnemyPatt.AttDown || patt_Now == EnemyPatt.DefDown)     // 디버프형 패턴
        {
            DeBuffCall();
        }
        else if(patt_Now == EnemyPatt.Summon)       // 소환 패턴
        {
            SummonCall();
        }
    }

    #region ---------- Attack Funcs

    public void AttackCall()        // 공격 애니메이션 호출
    {
        if (patt_Now != EnemyPatt.AttDrain)     // 공격이 일반 공격인지 흡혈 공격인지 체크
            anim.SetTrigger("Attack");
        else
            anim.SetTrigger("Drain");

        SoundScript.Inst.SoundControl("Attack");        // 사운드 재생
    }

    public void AttackFunc()        // 모든 공격 애니메이션 중간에 이벤트로 호출 될 함수
    {
        switch(patt_Now)        // 패턴 종류 구분
        {
            case EnemyPatt.Attack:      // 일반 공격일 경우
                {
                    float att_Damage = att_Point * att_Up;      // 최종 대미지는 공격력과 버프를 계산하여 수치를 설정
                    HeroScript.Inst.DamageCall(Mathf.Floor(att_Damage), this);      // 주인공 캐릭터의 대미지 함수 호출 매개변수로 최종 대미지(소수점 내림)와 공격한 유닛 정보를 넘겨줌
                }
                break;
            case EnemyPatt.MultiAtt:    // 멀티 공격일 경우
                {
                    float att_Damage = (att_Point * 0.5f) * att_Up;      // 최종 대미지는 공격력의 일부와 버프를 계산하여 수치를 설정
                    HeroScript.Inst.multi_Count = 3;
                    HeroScript.Inst.DamageCall(Mathf.Floor(att_Damage), this);      // 주인공 캐릭터의 대미지 함수 호출 매개변수로 최종 대미지(소수점 내림)와 공격한 유닛 정보를 넘겨줌
                }
                break;
            case EnemyPatt.AttDrain:    // 흡혈 공격일 경우
                {
                    float att_Damage = att_Point * att_Up;      // 최종 대미지는 공격력과 버프를 계산하여 수치를 설정
                    HeroScript.Inst.DamageCall(Mathf.Floor(att_Damage), this);      // 주인공 캐릭터의 대미지 함수 호출 매개변수로 최종 대미지(소수점 내림)와 공격한 유닛 정보를 넘겨줌
                }
                break;
        }
    }

    public void DrainHP()       // 흡혈 공격 애니메이션 중간에 이벤트로 호출 될 함수
    {
        now_Hp += drain_Point;      // 현재 체력에 주인공 캐릭터가 받은 피해만큼을 추가

        if (now_Hp >= max_Hp)       // 추가된 현재 체력이 최대 체력 보다 높으면 최대 체력으로 설정
            now_Hp = max_Hp;

        if (hp_Img != null)
            hp_Img.fillAmount = now_Hp / max_Hp;        // 체력 이미지를 해당 비율에 맞게 설정

        if (hp_Text != null)
            hp_Text.text = (int)now_Hp + " / " + (int)max_Hp;       // 체력 텍스트 표기

        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);     // 이펙트 오브젝트 생성
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);       // 이벤트 오브젝트를 모아두기 위한 부모 오브젝트 설정
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);       // 크기 조정
        string m_Str = "- " + Mathf.Floor(drain_Point).ToString();      // 흡혈된 수치를 표기하기 위한 텍스트
        eff.GetComponent<EffectScript>().TextSetting(m_Str, new Color32(0, 255, 0, 255), Color.white);      // 이펙트를 텍스트로 설정. 매개변수로 문자열과 기본색상, 테두리 색상 값을 넘겨줌

        eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);     // 이펙트 오브젝트 생성
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);       // 이벤트 오브젝트를 모아두기 위한 부모 오브젝트 설정
        eff.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);       // 크기 조정
        eff.GetComponent<EffectScript>().EffectSetting(EffectType.Potion, this.gameObject);     // 매개변수로 넘겨준 타입의 이펙트를 해당 오브젝트 위치에 생성
        SoundScript.Inst.SoundControl("Potion");        // 사운드 재생

    }

    public void AttackEnd()        // 모든 공격 애니메이션 끝에 이벤트로 호출 될 함수
    {
        EnemyPattEnd();     // 몬스터의 패턴 종료 함수 호출
    }
    #endregion

    #region ---------- Defence Funcs

    public void DefenceCall()       // 방어 애니메이션 호출 함수
    {
        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);     // 이펙트 오브젝트 생성
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);       // 이벤트 오브젝트를 모아두기 위한 부모 오브젝트 설정
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);       // 크기 조정
        eff.GetComponent<EffectScript>().EffectSetting(EffectType.Defence, this.gameObject);     // 매개변수로 넘겨준 타입의 이펙트를 해당 오브젝트 위치에 생성
        def_Save += (int)(def_Point * def_Up);      // 저장될 방어력 수치 설정 방어력과 버프량을 계산하여 설정
        SoundScript.Inst.SoundControl("Defence");   // 사운드 재생
    }

    #endregion

    #region ---------- Damage Funcs
    public void DamageCall(float a_Damage)      // 대미지를 받을 경우 호출되는 함수
    {
        if (per_Bool == true)       // 무적버프가 있다면 피해를 0으로 만듬
            a_Damage = 0;

        float m_Damage = a_Damage;
        multi_Damage = a_Damage;        // 멀티 대미지를 위한 대미지 값 저장

        if (def_Save > 0)       // 저장된 방어력이 있다면
        {
            m_Damage = a_Damage - def_Save;     // 피해량을 저장된 방어력 만큼 감소
            def_Save -= a_Damage;       // 저장된 방어력 감소

            if (m_Damage <= 0)      // 저장된 방어력이 피해량보다 높아서 피해량이 0 이하가 된다면
                m_Damage = 0;       // 피해량 0 으로 설정


            DefenceReset();     // 방어력 표시 초기화
        }

        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);     // 이펙트 오브젝트 생성
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);       // 이벤트 오브젝트를 모아두기 위한 부모 오브젝트 설정
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);       // 크기 조정

        if (m_Damage > 0)       // 피해량이 0 보다 크다면
        {
            string m_Str = "- " + Mathf.Floor(m_Damage).ToString();     // 피해량(내림)을 문자열로 저장
            eff.GetComponent<EffectScript>().TextSetting(m_Str, new Color32(255, 50, 50, 255), Color.white);      // 이펙트를 텍스트로 설정. 매개변수로 문자열과 기본색상, 테두리 색상 값을 넘겨줌
            SoundScript.Inst.SoundControl("Damage");        // 사운드 재생
        }
        else
        {
            string m_Str = "방어함";       // 피해량이 0 이면 해당 문자열 저장
            eff.GetComponent<EffectScript>().TextSetting(m_Str, new Color32(255, 255, 255, 255), Color.black);      // 이펙트를 텍스트로 설정. 매개변수로 문자열과 기본색상, 테두리 색상 값을 넘겨줌
            SoundScript.Inst.SoundControl("Guard");     // 사운드 재생
        }

        now_Hp -= m_Damage;     // 대미지 수치 만큼 현재 체력 감소
        if (now_Hp <= 0)        // 현재 체력이 0보다 작거나 같을 경우
            now_Hp = 0;         // 0으로 설정

        anim.Play(anim_Clip[(int)EnemyClip.Damage].name, -1, 0.0f);     // 대미지 애니메이션을 처음부터 재생(멀티 공격을 받으면 여러번 피격되는 효과를 위해 애니메이션 초기화가 필요)

        if (hp_Img != null)
            hp_Img.fillAmount = now_Hp / max_Hp;        // 체력 이미지를 해당 비율에 맞게 설정

        if (hp_Text != null)
            hp_Text.text = (int)now_Hp + " / " + (int)max_Hp;       // 체력 텍스트 표기

    }

    public void MultiCheck()        // 멀티 공격이 있을 경우 피격 애니메이션 중간에 호출
    {
        if (multi_Count > 1)    // 멀티 카운트가 남아 있는지 체크
        {
            multi_Count--;      // 횟수 감소
            DamageCall(multi_Damage);       // 대미지 호출 함수
        }
    }

    public void DamageEnd()     // 피격 애니메이션 끝에 호출
    {
        if (now_Hp <= 0.0f)     // 현제 체력이 0 이거나 그보다 작을 경우
        {
            StageScript.Inst.enemy_List.Remove(this);
            anim.SetTrigger("Death");
            SoundScript.Inst.SoundControl("Death");

            if(enemyType == EnemyType.B_Monster)
            {
                for (int ii = 0; ii < summon_Obj.Length; ii++)
                {
                    if (summon_Obj[ii] == null)
                        continue;

                    summon_Obj[ii].GetComponent<EnemySript>().DamageCall(100);
                    SoundScript.Inst.SoundControl("Death");
                }
            }

                    
        }
        else
        {
            HeroScript.Inst.patt_Bool = true;
            StageScript.Inst.turn_Btn.gameObject.SetActive(true);
        }
            

    }

    public void DeathEnd()
    {
        if(StageScript.Inst.enemy_List.Count <= 0)
        {
            this.transform.position = new Vector3(GlobalScript.g_Width, 0, 0);

            if (StageScript.Inst.stageType == StageType.Boss)
            {
                if(enemyType == EnemyType.B_Monster)
                    StageScript.Inst.RewardCall();
            }
            else
            {
                StageScript.Inst.RewardCall();
            }
                

        }

        HeroScript.Inst.patt_Bool = true;
        StageScript.Inst.turn_Btn.gameObject.SetActive(true);
        Destroy(this.gameObject);
    }
    #endregion

    #region ---------- Buff Funcs

    void BuffCall()
    {
        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        e_Patten.Remove(patt_Now);

        switch (patt_Now)
        {
            case EnemyPatt.AttUp:
                {
                    SoundScript.Inst.SoundControl("AttackUp");
                    eff.GetComponent<EffectScript>().EffectSetting(EffectType.AttUp, this.gameObject);
                    att_Up = 1.2f;
                }
                break;
            case EnemyPatt.ADUp:
                {
                    SoundScript.Inst.SoundControl("AttackUp");
                    eff.GetComponent<EffectScript>().EffectSetting(EffectType.ADUp, this.gameObject);
                    att_Up = 1.2f;
                    def_Up = 1.2f;
                }
                break;
            case EnemyPatt.Perfect:
                {
                    SoundScript.Inst.SoundControl("Perfect");
                    eff.GetComponent<EffectScript>().EffectSetting(EffectType.Perfect, this.gameObject);
                    per_Bool = true;
                }
                break;
        }
    }


    void DeBuffCall()
    {
        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, HeroScript.Inst.transform.position, transform.rotation);
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        switch (patt_Now)
        {
            case EnemyPatt.DefDown:
                {
                    SoundScript.Inst.SoundControl("AttDown");
                    eff.GetComponent<EffectScript>().EffectSetting(EffectType.DefDown, this.gameObject);
                    HeroScript.Inst.def_Down = 0.2f;
                }
                break;
            case EnemyPatt.AttDown:
                {
                    SoundScript.Inst.SoundControl("DefDown");
                    eff.GetComponent<EffectScript>().EffectSetting(EffectType.AttDown, this.gameObject);
                    HeroScript.Inst.att_Down = 0.2f;
                }
                break;
        }

        CardCtrlScript.Inst.CardReSetting();
    }

    void BuffCount(EnemyPatt a_Patt)
    {
        BuffScript[] Ref_List = null;
        Ref_List = buff_Root.GetComponentsInChildren<BuffScript>();

        for (int ii = 0; ii < Ref_List.Length; ii++)
        {
            switch (Ref_List[ii].buffType)
            {
                case BuffType.AttUp:
                    {
                        if (a_Patt == EnemyPatt.AttUp)
                            return;
                    }
                    break;
                case BuffType.ADUp:
                    {
                        if (a_Patt == EnemyPatt.ADUp)
                            return;
                    }
                    break;
                case BuffType.Perfect:
                    {
                        if (a_Patt == EnemyPatt.Perfect)
                            return;
                    }
                    break;
            }


            if (Ref_List[ii].buffType == BuffType.Defence)
                return;

            Ref_List[ii].buff_Count--;

            if (Ref_List[ii].buff_Count <= 0)
            {
                if (Ref_List[ii].buffType == BuffType.Perfect)
                    per_Bool = false;

                switch (Ref_List[ii].buffType)
                {
                    case BuffType.AttUp:
                        {
                            e_Patten.Add(EnemyPatt.AttUp);
                            att_Up = 1.0f;
                        }
                        break;
                    case BuffType.ADUp:
                        {
                            e_Patten.Add(EnemyPatt.ADUp);
                            att_Up = 1.0f;
                            def_Up = 1.0f;
                        }
                        break;
                    case BuffType.Perfect:
                        {
                            e_Patten.Add(EnemyPatt.Perfect);
                        }
                        break;
                }

                Destroy(Ref_List[ii].gameObject);
            }
                
            else
                Ref_List[ii].buff_Txt.text = Ref_List[ii].buff_Count.ToString();

        }
    }

    #endregion

    #region ---------- Summon Funcs

    void SummonCall()
    {
        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        eff.GetComponent<EffectScript>().EffectSetting(EffectType.Summon, this.gameObject);
        SoundScript.Inst.SoundControl("Summon");
    }

    #endregion

    #region ---------- Effect Funcs

    public void EffEndCall(EffectType a_Type)
    {
        BuffScript[] Ref_List = null;
        Ref_List = buff_Root.GetComponentsInChildren<BuffScript>();

        BuffScript[] HeroRef_List = null;
        HeroRef_List = HeroScript.Inst.buff_Root.GetComponentsInChildren<BuffScript>();

        GameObject buff = Instantiate(StageScript.Inst.buff_Obj, buff_Root.transform.position, transform.rotation);

        switch (a_Type)
        {
            case EffectType.Defence:
                {
                    float value = Mathf.Floor(def_Save);
                    buff.GetComponent<BuffScript>().BuffSetting(BuffType.Defence, (int)value);
                    buff.transform.SetParent(buff_Root.transform);
                }
                break;
            case EffectType.AttUp:
                {
                    buff.GetComponent<BuffScript>().BuffSetting(BuffType.AttUp, 2);
                    buff.transform.SetParent(buff_Root.transform);
                }
                break;
            case EffectType.DefDown:
                {
                    buff.GetComponent<BuffScript>().BuffSetting(BuffType.DefDown, 2);
                    buff.transform.SetParent(HeroScript.Inst.buff_Root.transform);


                    for (int ii = 0; ii < HeroRef_List.Length; ii++)
                    {
                        if (HeroRef_List[ii].buffType == buff.GetComponent<BuffScript>().buffType)
                            Destroy(HeroRef_List[ii].gameObject);
                    }

                }
                break;
            case EffectType.AttDown:
                {
                    buff.GetComponent<BuffScript>().BuffSetting(BuffType.AttDown, 2);
                    buff.transform.SetParent(HeroScript.Inst.buff_Root.transform);


                    for (int ii = 0; ii < HeroRef_List.Length; ii++)
                    {
                        if (HeroRef_List[ii].buffType == buff.GetComponent<BuffScript>().buffType)
                            Destroy(HeroRef_List[ii].gameObject);
                    }

                }
                break;
            case EffectType.ADUp:
                {
                    buff.GetComponent<BuffScript>().BuffSetting(BuffType.ADUp, 2);
                    buff.transform.SetParent(buff_Root.transform);
                }
                break;
            case EffectType.Perfect:
                {
                    buff.GetComponent<BuffScript>().BuffSetting(BuffType.Perfect, 2);
                    buff.transform.SetParent(buff_Root.transform);
                }
                break;
            case EffectType.Summon:
                {
                    for (int ii = 0; ii < summon_Obj.Length; ii++)
                    {
                        if (summon_Obj[ii] != null)
                            continue;

                        GameObject enemy_Obj = Instantiate(StageScript.Inst.enemy_Prefab[(int)EnemyType.N_Ghost]);

                        float posX = 0.0f;
                        posX = (GlobalScript.g_Width * (-0.45f + (0.15f * (ii * 2)))) + (GlobalScript.g_Width);

                        enemy_Obj.transform.position = new Vector3(posX, GlobalScript.g_Height * 0.6f, 0);
                        enemy_Obj.transform.SetParent(StageScript.Inst.enemy_Root.transform);
                        enemy_Obj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                        enemy_Obj.GetComponent<EnemySript>().enemyType = EnemyType.N_Ghost;
                        enemy_Obj.GetComponent<EnemySript>().patt_Bool = false;
                        StageScript.Inst.enemy_List.Add(enemy_Obj.GetComponent<EnemySript>());
                        summon_Obj[ii] = enemy_Obj;
                    }

                    StageScript.Inst.pattCount = StageScript.Inst.enemy_List.Count;

                }
                break;
        }

        buff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        for (int ii = 0; ii < Ref_List.Length; ii++)
        {
            if (Ref_List[ii].buffType == buff.GetComponent<BuffScript>().buffType)
                Destroy(Ref_List[ii].gameObject);
        }

        EnemyPattEnd();
    }

    #endregion

    void EnemyPattEnd()     // 패턴이 끝났을 때 호출되는 함수
    {
        patt_Bool = false;                  // 패턴이 진행 중이 아님을 체크하는 변수
        BuffCount(patt_Now);                // 적용중인 버프의 카운트 감소
        StageScript.Inst.EnemyPattCall();   // 다른 몬스터가 남아 있으면 그 몬스터의 패턴을 실행 시키는 함수 없으면 턴을 넘김
        PattObjSetting();                   // 새로운 패턴 설정
    }

    void PattObjSetting()       // 패턴 설정을 위한 함수
    {
        int rand = Random.Range(0, e_Patten.Count);    // 몬스터가 가지고 있는 패턴 중 랜덤한 값 설정
        patt_Now = e_Patten[rand];
        string value = "";          // 몬스터 위에 표시되는 패턴에 수치

        if (enemyType == EnemyType.B_Monster)    // 보스 몬스터일 경우
        {
            int summon_Count = 0;       // 소환된 몬스터가 있는지 체크할 변수

            for (int ii = 0; ii < summon_Obj.Length; ii++)
            {
                if (summon_Obj[ii] == null)  // 소환된 몬스터가 한마리라도 비어있으면 소환 패턴 설정
                    summon_Count++;
            }

            if (summon_Count >= 2)      // 몬스터의 자리가 2개 이상 비어있으면 소환 패턴으로 설정
                patt_Now = EnemyPatt.Summon;
        }


        switch (patt_Now)
        {
            case EnemyPatt.Attack:
                {
                    value = (att_Point * att_Up).ToString();        // 최종 피해량을 문자열로 저장
                }
                break;
            case EnemyPatt.Defence:
                {
                    value = (def_Point * def_Up).ToString();        // 저장할 방어력 값을 문자열로 저장
                }
                break;
            case EnemyPatt.MultiAtt:
                {
                    value = ((att_Point * 0.5f) * att_Up).ToString();       // 멀티 공격의 피해량을 문자열로 저장
                }
                break;
            case EnemyPatt.AttDrain:
                {
                    value = (att_Point * att_Up).ToString();        // 최종 피해량을 문자열로 저장
                }
                break;
        }

        patt_Obj.GetComponent<PattScript>().PattSetting(patt_Now, value);       // 패턴 표시용 오브젝트를 해당 패턴으로 설정
    }

    void DefenceReset()     // 방어 리셋 함수
    {
        BuffScript[] Ref_List = null;       // 적용 중인 버프를 저장할 배열
        Ref_List = buff_Root.GetComponentsInChildren<BuffScript>();     // 적용중인 모든 버프를 저장

        for (int ii = 0; ii < Ref_List.Length; ii++)
        {
            if (Ref_List[ii].buffType != BuffType.Defence)  // 버프 타입이 방어일 경우만 실행
                continue;

            if (def_Save <= 0)      // 저장된 방어력이 남아 있다면
            {
                Destroy(Ref_List[ii].gameObject);       // 해당 버프 오브젝트 제거
                def_Save = 0;       // 저장된 방어력 초기화
            }
            else
                Ref_List[ii].buff_Txt.text = def_Save.ToString();   // 방어력 텍스트를 저장된 방어력으로 수정

        }
    }
}
