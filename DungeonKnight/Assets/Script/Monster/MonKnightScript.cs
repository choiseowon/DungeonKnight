using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonKnightScript : MonsterClass, IAttack, IDefence, IAttUp, IDamage
{
    public float att_Point { get; set; }
    public float def_Point { get; set; }
    public float def_Save { get; set; }
    public float att_Up { get; set; }

    void Start()
    {
        max_Hp = 150.0f;        // 최대 체력 설정
        now_Hp = max_Hp;        // 최대 체력 설정
        att_Point = 10.0f;
        def_Point = 10.0f;
        att_Up = 1.0f;
        anim_Runtime = anim.runtimeAnimatorController;

        if (target_Btn != null)
            TargetFunc();

        hp_Ui.HpSetting(max_Hp, now_Hp);

        patt_List.Add("Attack");
        patt_List.Add("Defence");
        patt_List.Add("AttUp");

        PattSetting();
    }

    void Update()
    {

    }

    public override void PattSetting()
    {
        int rand = Random.Range(0, patt_List.Count);
        patt_Str = patt_List[rand];
        patt_Obj.SetActive(true);

        string value_Str = "";

        if (buffType != BuffType.Count)
        {
            BuffCheck(buffType, buff_Value);
            buffType = BuffType.Count;
            buff_Value = 0.0f;
        }

        switch (patt_Str)
        {
            case "Attack":
                value_Str = Mathf.Floor(att_Point * att_Up).ToString();
                break;
            case "Defence":
                value_Str = Mathf.Floor(def_Point).ToString();
                break;
        }

        patt_Obj.GetComponent<PattScript>().PattSetting(patt_Str, value_Str);
    }

    public override void PattCall()
    {
        if (damage_Bool == true)
        {
            patt_Bool = true;
            return;
        }

        if (buff_ValueDict.ContainsKey(BuffType.Defence) == true)
            buff_ValueDict.Remove(BuffType.Defence);

        monState = MonState.Patt;
        this.GetType().GetMethod(patt_Str).Invoke(this, null);
        BuffCount();
        patt_Obj.SetActive(false);
    }

    void PattEnd()
    {
        StageScript.Inst.EnemyPattCall();
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
        SoundScript.Inst.SfSoundPlay("Attack");        // 사운드 재생

        float att_Damage = Mathf.Floor(att_Point * att_Up);      // 최종 대미지는 공격력과 버프를 계산하여 수치를 설정
        float time = GetTime(anim_Runtime, "Attack");

        StartCoroutine(AttackCo(time, att_Damage));
    }

    IEnumerator AttackCo(float time, float a_Damage)
    {
        yield return new WaitForSeconds(time * 0.65f);
        HeroScript.Inst.Damage(a_Damage);
        yield return new WaitForSeconds(time * 0.35f);
        PattEnd();
    }

    public void Defence()
    {
        def_Save += Mathf.Floor(def_Point);
        eff_Root.EffectSetting(EffectType.Defence, this.gameObject);
        SoundScript.Inst.SfSoundPlay("Defence");
        eff_Runtime = eff_Root.eff_Runtime;

        if (eff_Runtime != null)
        {
            float time = GetTime(eff_Runtime, "Defence");
            StartCoroutine(EffectCo(BuffType.Defence, def_Save, time));
        }
    }

    public void AttUp()
    {
        patt_List.Remove("AttUp");
        att_Up = 1.2f;
        eff_Root.EffectSetting(EffectType.AttUp, this.gameObject);
        SoundScript.Inst.SfSoundPlay("PowerUp");
        eff_Runtime = eff_Root.eff_Runtime;

        if (eff_Runtime != null)
        {
            float time = GetTime(eff_Runtime, "AttUp");
            StartCoroutine(EffectCo(BuffType.AttUp, 2.0f, time));
        }
    }

    IEnumerator EffectCo(BuffType a_Buff, float buff_Value, float time)
    {
        yield return new WaitForSeconds(time);
        buffType = a_Buff;
        this.buff_Value = buff_Value;
        PattEnd();
    }

    void BuffCheck(BuffType buff_Type, float buff_Value)
    {
        if (buff_ValueDict.ContainsKey(buff_Type) == true)
        {
            if (buff_Value <= 0.0f)
                buff_ValueDict.Remove(buff_Type);
            else
                buff_ValueDict[buff_Type] = buff_Value;

            BuffView();
        }
        else
        {
            buff_ValueDict.Add(buff_Type, buff_Value);
            GameObject obj = InstantiateFunc(buff_Root.transform, buff_Obj);
            obj.GetComponent<BuffScript>().BuffSetting(buff_Type, (int)buff_Value);
        }
    }

    GameObject InstantiateFunc(Transform a_Parent, GameObject a_Prefab)
    {
        GameObject obj = Instantiate(a_Prefab);
        obj.transform.SetParent(a_Parent);
        obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        return obj;
    }

    public void BuffCount()
    {
        foreach (BuffType m_Type in buff_ValueDict.Keys.ToList())
            buff_ValueDict[m_Type] = buff_ValueDict[m_Type] - 1;

        BuffView();
    }

    void BuffView()
    {
        BuffScript[] buff_Array = buff_Root.GetComponentsInChildren<BuffScript>();

        foreach (BuffScript buff in buff_Array)
        {
            if (buff.buffType == BuffType.Defence && buff_ValueDict.ContainsKey(BuffType.Defence) != true)
            {
                def_Save = 0;
                Destroy(buff.gameObject);
            }

            buff_ValueDict.TryGetValue(buff.buffType, out float count);
            buff.buff_Count = (int)count;

            if (buff.buff_Count <= 0)
            {
                switch (buff.buffType)
                {
                    case BuffType.AttUp:
                        {
                            att_Up = 1.0f;
                            patt_List.Add("AttUp");
                        }
                        break;
                }

                buff_ValueDict.Remove(buff.buffType);
                Destroy(buff.gameObject);
            }
            else
            {
                buff.BuffSetting(buff.buffType, buff.buff_Count);
            }
        }
    }

    float GetTime(RuntimeAnimatorController a_RunTime, string ani_Str)
    {
        float time = 0.0f;

        for (int ii = 0; ii < a_RunTime.animationClips.Length; ii++)
            if (a_RunTime.animationClips[ii].name.Contains(ani_Str))
            {
                time = a_RunTime.animationClips[ii].length;
            }

        return time;
    }

    IEnumerator damage_Co = null;

    public void Damage(float a_Damage)
    {
        monState = MonState.Damage;
        damage_Bool = true;
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
            // 이펙트를 텍스트로 설정. 매개변수로 문자열과 기본색상, 테두리 색상 값을 넘겨줌
            if(damage_Co == null)
                anim.SetTrigger("Damage");
            SoundScript.Inst.SfSoundPlay("Damage");        // 사운드 재생
        }
        else
        {
            m_Str = "방어함";       // 피해량이 0 이면 해당 문자열 저장
            eff_Root.TextSetting(m_Str, new Color32(255, 255, 255, 255), Color.black);
            // 이펙트를 텍스트로 설정. 매개변수로 문자열과 기본색상, 테두리 색상 값을 넘겨줌 
            SoundScript.Inst.SfSoundPlay("Guard");     // 사운드 재생
        }

        if (now_Hp <= 0 && target_Btn != null)
        {
            target_Btn.gameObject.SetActive(false);
            target_Btn = null;
        }

        float time = GetTime(anim_Runtime, "Damage");

        if (damage_Co != null)
            return;

        damage_Co = DamageCo(time);
        StartCoroutine(damage_Co);
    }

    IEnumerator DamageCo(float time)
    {
        yield return new WaitForSeconds(time);
        if (now_Hp <= 0.0f)     // 현제 체력이 0 이거나 그보다 작을 경우
        {
            float death_time = GetTime(anim_Runtime, "Death");
            monState = MonState.Death;
            StartCoroutine(DeathCo(death_time));
        }
        else
        {
            damage_Bool = false;
            monState = MonState.Count;

            if (patt_Bool == true)
            {
                PattCall();
                patt_Bool = false;
            }

        }

        damage_Co = null;
    }

    IEnumerator DeathCo(float time)
    {
        anim.SetTrigger("Death");
        SoundScript.Inst.SfSoundPlay("Death");

        yield return new WaitForSeconds(time);

        monState = MonState.Count;
        StageScript.Inst.enemy_List.Remove(this.gameObject);
        StageScript.Inst.EnemyPoolReset();

        if (StageScript.Inst.enemy_List.Count <= 0)
            StageScript.Inst.RewardCall();

        Destroy(this.gameObject);
    }
}
