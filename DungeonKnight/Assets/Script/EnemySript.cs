using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    List<EnemyPatt> patt_List = new List<EnemyPatt>();
    EnemyPatt patt_Now = EnemyPatt.Count;

    void Start()
    {
        //enemyType = EnemyType.N_Ghost;
        EnemySetting(enemyType);

        if (target_Btn != null)
        {
            target_Btn.onClick.AddListener(() =>
            {
                if (HeroScript.Inst.target_Check != true)
                    return;

                int mana = StageScript.Inst.hero_Mana;
                int card_Mana = StageScript.Inst.cardScript.card_Mana;
                StageScript.Inst.ManaSetting(mana - card_Mana);
                StageScript.Inst.CardRemove(StageScript.Inst.cardScript.gameObject);
                HeroScript.Inst.m_Target = this.gameObject;
                HeroScript.Inst.AttackCall();
            });
        }

        PattObjSetting();
    }

    void Update()
    {
    }

    void EnemySetting(EnemyType a_Type)
    {
        switch (a_Type)
        {
            case EnemyType.N_Ghost:
                {
                    patt_List.Add(EnemyPatt.Attack);
                    patt_List.Add(EnemyPatt.AttUp);

                    max_Hp = 100.0f;
                    att_Point = 10.0f;
                }
                break;
            case EnemyType.N_Thief:
                {
                    patt_List.Add(EnemyPatt.Attack);
                    patt_List.Add(EnemyPatt.Defence);
                    patt_List.Add(EnemyPatt.AttUp);

                    max_Hp = 100.0f;
                    att_Point = 10.0f;
                    def_Point = 5.0f;
                }
                break;
            case EnemyType.N_Knight:
                {
                    patt_List.Add(EnemyPatt.Attack);
                    patt_List.Add(EnemyPatt.Defence);
                    patt_List.Add(EnemyPatt.AttUp);

                    max_Hp = 150.0f;
                    att_Point = 10.0f;
                    def_Point = 10.0f;
                }
                break;
            case EnemyType.E_Thief:
                {
                    patt_List.Add(EnemyPatt.Attack);
                    patt_List.Add(EnemyPatt.Defence);
                    patt_List.Add(EnemyPatt.AttUp);
                    patt_List.Add(EnemyPatt.MultiAtt);
                    patt_List.Add(EnemyPatt.DefDown);

                    max_Hp = 400.0f;
                    att_Point = 30.0f;
                    def_Point = 15.0f;
                }
                break;
            case EnemyType.E_Knight:
                {
                    patt_List.Add(EnemyPatt.Attack);
                    patt_List.Add(EnemyPatt.Defence);
                    patt_List.Add(EnemyPatt.ADUp);
                    patt_List.Add(EnemyPatt.AttDown);
                    patt_List.Add(EnemyPatt.Perfect);

                    max_Hp = 500.0f;
                    att_Point = 25.0f;
                    def_Point = 20.0f;
                }
                break;
            case EnemyType.B_Monster:
                {
                    patt_List.Add(EnemyPatt.AttDrain);
                    patt_List.Add(EnemyPatt.Defence);
                    patt_List.Add(EnemyPatt.ADUp);
                    patt_List.Add(EnemyPatt.MultiAtt);
                    patt_List.Add(EnemyPatt.Summon);

                    max_Hp = 1000.0f;
                    att_Point = 50.0f;
                    def_Point = 30.0f;
                }
                break;

        }

        now_Hp = max_Hp;

        if (hp_Img != null)
            hp_Img.fillAmount = now_Hp / max_Hp;

        if (hp_Text != null)
            hp_Text.text = (int)now_Hp + " / " + (int)max_Hp;
    }

    public void PatternCall()
    {
        if (HeroScript.Inst == null)
            return;

        def_Save = 0;               // 남아있던 방어막을 0으로 만든
        DefenceReset();             // 방어막 표시 초기화
        patt_Obj.SetActive(false);  // 패턴 표시 끄기

        // patt_Now의 값에 따라 다른 함수 호출
        if(patt_Now == EnemyPatt.Attack || patt_Now == EnemyPatt.MultiAtt || patt_Now == EnemyPatt.AttDrain)
        {
            AttackCall();
        }
        else if(patt_Now == EnemyPatt.Defence)
        {
            DefenceCall();
        }
        else if(patt_Now == EnemyPatt.AttUp || patt_Now == EnemyPatt.ADUp || patt_Now == EnemyPatt.Perfect)
        {
            BuffCall();
        }
        else if(patt_Now == EnemyPatt.AttDown || patt_Now == EnemyPatt.DefDown)
        {
            DeBuffCall();
        }
        else if(patt_Now == EnemyPatt.Summon)
        {
            SummonCall();
        }
    }

    #region ---------- Attack Funcs

    public void AttackCall()
    {
        // 패턴 종류에 따른 애니메이션 호출
        if (patt_Now != EnemyPatt.AttDrain)
            anim.SetTrigger("Attack");
        else
            anim.SetTrigger("Drain");

        SoundScript.Inst.SoundControl("Attack");
    }

    public void AttackFunc()
    {
        // 패턴에 따른 대미지 값을 정한 후 대미지 호출
        switch(patt_Now)
        {
            case EnemyPatt.Attack:
                {
                    float att_Damage = att_Point * att_Up;
                    HeroScript.Inst.DamageCall(Mathf.Floor(att_Damage), this);
                }
                break;
            case EnemyPatt.MultiAtt:
                {
                    float att_Damage = (att_Point * 0.5f) * att_Up;
                    HeroScript.Inst.multi_Count = 3;
                    HeroScript.Inst.DamageCall(Mathf.Floor(att_Damage), this);
                }
                break;
            case EnemyPatt.AttDrain:
                {
                    float att_Damage = att_Point * att_Up;
                    HeroScript.Inst.DamageCall(Mathf.Floor(att_Damage), this);
                }
                break;
        }
    }

    public void DrainHP()
    {
        now_Hp += drain_Point;

        if (now_Hp >= max_Hp)
            now_Hp = max_Hp;

        if (hp_Img != null)
            hp_Img.fillAmount = now_Hp / max_Hp;

        if (hp_Text != null)
            hp_Text.text = (int)now_Hp + " / " + (int)max_Hp;

        // 이펙트 오브젝트를 생성하여 이펙트 효과
        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        string m_Str = "- " + Mathf.Floor(drain_Point).ToString();
        eff.GetComponent<EffectScript>().TextSetting(m_Str, new Color32(0, 255, 0, 255), Color.white);
        SoundScript.Inst.SoundControl("Potion");

    }

    // 패턴이 끝나면 호출되는 함수
    public void AttackEnd()
    {
        EnemyPattEnd();
    }
    #endregion

    #region ---------- Defence Funcs

    public void DefenceCall()
    {
        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        eff.GetComponent<EffectScript>().EffectSetting(EffectType.Defence, this.gameObject);
        def_Save += (int)(def_Point * def_Up);
        SoundScript.Inst.SoundControl("Defence");
    }

    #endregion

    #region ---------- Damage Funcs
    public void DamageCall(float a_Damage)
    {
        if (per_Bool == true)
            a_Damage = 0;

        float m_Damage = a_Damage;
        multi_Damage = a_Damage;

        if (def_Save > 0)
        {
            m_Damage = a_Damage - def_Save;
            def_Save -= a_Damage;

            if (m_Damage <= 0)
                m_Damage = 0;


            DefenceReset();
        }

        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        if (m_Damage > 0)
        {
            string m_Str = "- " + Mathf.Floor(m_Damage).ToString();
            eff.GetComponent<EffectScript>().TextSetting(m_Str, new Color32(255, 50, 50, 255), Color.white);
            SoundScript.Inst.SoundControl("Damage");
        }
        else
        {
            string m_Str = "방어함";
            eff.GetComponent<EffectScript>().TextSetting(m_Str, new Color32(255, 255, 255, 255), Color.black);
            SoundScript.Inst.SoundControl("Guard");
        }

        now_Hp -= m_Damage;
        if (now_Hp <= 0)
            now_Hp = 0;

        anim.Play(anim_Clip[(int)EnemyClip.Damage].name, -1, 0.0f);

        if (hp_Img != null)
            hp_Img.fillAmount = now_Hp / max_Hp;

        if (hp_Text != null)
            hp_Text.text = (int)now_Hp + " / " + (int)max_Hp;

    }

    public void MultiCheck()
    {
        if (multi_Count > 1)
        {
            multi_Count--;
            DamageCall(multi_Damage);
        }
    }

    public void DamageEnd()
    {
        if (now_Hp <= 0.0f)
        {
            StageScript.Inst.enemy_List.Remove(this);
            anim.SetTrigger("Death");
            SoundScript.Inst.SoundControl("Death");

            if(enemyType == EnemyType.B_Monster)
                for (int ii = 0; ii < summon_Obj.Length; ii++)
                {
                    if (summon_Obj[ii] != null)
                    {
                        Debug.Log("AA");
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
        patt_List.Remove(patt_Now);

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
                            patt_List.Add(EnemyPatt.AttUp);
                            att_Up = 1.0f;
                        }
                        break;
                    case BuffType.ADUp:
                        {
                            patt_List.Add(EnemyPatt.ADUp);
                            att_Up = 1.0f;
                            def_Up = 1.0f;
                        }
                        break;
                    case BuffType.Perfect:
                        {
                            patt_List.Add(EnemyPatt.Perfect);
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
                        enemy_Obj.transform.SetParent(StageScript.Inst.char_Root.transform);
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

    void EnemyPattEnd()
    {
        patt_Bool = false;                  // 패턴이 진행 중이 아님을 체크하는 변수
        BuffCount(patt_Now);                // 적용중인 버프의 카운트 감소
        StageScript.Inst.EnemyPattCall();   // 다른 몬스터가 남아 있으면 그 몬스터의 패턴을 실행 시키는 함수 없으면 턴을 넘김
        PattObjSetting();                   // 새로운 패턴 설정
    }

    void PattObjSetting()
    {
        int rand = Random.Range(0, patt_List.Count);    // 몬스터가 가지고 있는 패턴 중 랜덤한 값 설정

        if(enemyType == EnemyType.B_Monster)    // 보스 몬스터일 경우
        {
            rand = Random.Range(0, patt_List.Count - 1);    // 패턴 리스트의 마지막 값은 제외 (몬스터 소환) 

            for(int ii = 0; ii < summon_Obj.Length; ii++)
            {
                if(summon_Obj[ii] == null)  // 소환된 몬스터가 한마리라도 비어있으면 소환 패턴 설정
                    rand = patt_List.Count - 1;
            }
        }

        patt_Now = patt_List[rand];
        string value = "";          // 몬스터 위에 표시되는 패턴에 수치

        switch (patt_Now)
        {
            case EnemyPatt.Attack:
                {
                    value = (att_Point * att_Up).ToString();
                }
                break;
            case EnemyPatt.Defence:
                {
                    value = (def_Point * def_Up).ToString();
                }
                break;
            case EnemyPatt.MultiAtt:
                {
                    value = ((att_Point * 0.5f) * att_Up).ToString();
                }
                break;
            case EnemyPatt.AttDrain:
                {
                    value = (att_Point * att_Up).ToString();
                }
                break;
        }

        patt_Obj.GetComponent<PattScript>().PattSetting(patt_Now, value);
    }

    void DefenceReset()
    {
        BuffScript[] Ref_List = null;
        Ref_List = buff_Root.GetComponentsInChildren<BuffScript>();

        for (int ii = 0; ii < Ref_List.Length; ii++)
        {
            if (Ref_List[ii].buffType != BuffType.Defence)
                continue;

            if (def_Save <= 0)
            {
                Destroy(Ref_List[ii].gameObject);
                def_Save = 0;
            }
            else
                Ref_List[ii].buff_Txt.text = def_Save.ToString();

        }
    }
}
