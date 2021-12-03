using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroScript : CharacScript
{
    public static HeroScript Inst = null;

    [Header("Hero Setting Value")]
    public bool target_Check = false;

    public CardType hero_patt = CardType.Count;
    public GameObject m_Target = null;
    EnemySript m_Enemy;

    [Header("Animation")]
    public Animator anim = null;
    public AnimationClip[] anim_Clip = null;

    [Header("Hero UI Value")]
    public Image hp_Img = null;
    public Text hp_Text = null;
    public GameObject buff_Root = null;

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        att_Point = GlobalScript.g_AttState;
        def_Point = GlobalScript.g_DefState;
        max_Hp = GlobalScript.g_HealthMax;
        now_Hp = GlobalScript.g_HealthNow;
        att_Up = 1.0f;
        def_Up = 1.0f;
        att_Down = 0.0f;
        def_Down = 0.0f;
        hp_Img.fillAmount = now_Hp / max_Hp;
        hp_Text.text = (int)now_Hp + " / " + (int)max_Hp;
    }

    void Update()
    {
    }

    public void PatternCall(CardType cardType)
    {
        if (patt_Bool == false)
            return;

        hero_patt = cardType;

        switch (hero_patt)
        {
            case CardType.Attack:
                {
                    TargetCheck();
                }
                break;
            case CardType.Defence:
                {
                    patt_Bool = false;
                    StageScript.Inst.turn_Btn.gameObject.SetActive(false);
                    DefenceCall();
                }
                break;
            case CardType.AllAttack:
                {
                    patt_Bool = false;
                    StageScript.Inst.turn_Btn.gameObject.SetActive(false);
                    AttackCall();
                }
                break;
            case CardType.MultiAtt:
                {
                    TargetCheck();
                }
                break;
            case CardType.AttDef:
                {
                    TargetCheck();
                }
                break;
            case CardType.AttUp:
                {
                    patt_Bool = false;
                    StageScript.Inst.turn_Btn.gameObject.SetActive(false);
                    AttUpCall();
                }
                break;
        }
    }

    public void DefenceCall()
    {
        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        eff.GetComponent<EffectScript>().EffectSetting(EffectType.Defence, this.gameObject); 
        SoundScript.Inst.SoundControl("Defence");

        switch (hero_patt)
        {
            case CardType.Defence:
                {
                    def_Save += (def_Point + GlobalScript.g_DefPuls) * (def_Up - def_Down);
                }
                break;
            case CardType.AttDef:
                {
                    def_Save += ((def_Point * 0.7f) + GlobalScript.g_DefPuls) * (def_Up - def_Down);
                }
                break;
        }
    }

    #region ---------- Attack Funcs

    void TargetCheck()
    {
        List<EnemySript> target_List = StageScript.Inst.enemy_List;
        target_Check = true;

        for (int ii = 0; ii < target_List.Count; ii++)
            target_List[ii].target_Btn.image.enabled = true;
    }

    public void AttackCall()//(GameObject a_Target)
    {
        if (target_Check == true)
        {
            List<EnemySript> target_List = StageScript.Inst.enemy_List;
            target_Check = false;

            for (int ii = 0; ii < target_List.Count; ii++)
                target_List[ii].target_Btn.image.enabled = false;

        }

        patt_Bool = false;
        StageScript.Inst.turn_Btn.gameObject.SetActive(false);
        SoundScript.Inst.SoundControl("Attack");
        anim.SetTrigger("Attack");
    }

    public void AttackFunc()
    {
        switch(hero_patt)
        {
            case CardType.Attack:
                {
                    float att_Damage = (att_Point + GlobalScript.g_AttPuls) * (att_Up - att_Down);
                    m_Target.GetComponent<EnemySript>().DamageCall(Mathf.Floor(att_Damage));
                }
                break;
            case CardType.AllAttack:
                {
                    float att_Damage = ((att_Point * 0.5f) + GlobalScript.g_AttPuls) * (att_Up - att_Down);
                    List<EnemySript> m_TargetList = StageScript.Inst.enemy_List;
                    for(int ii = 0; ii < m_TargetList.Count; ii++)
                    {
                        m_TargetList[ii].DamageCall(Mathf.Floor(att_Damage));
                    }
                }
                break;
            case CardType.MultiAtt:
                {
                    float att_Damage = ((att_Point * 0.3f) + GlobalScript.g_AttPuls) * (att_Up - att_Down);
                    m_Target.GetComponent<EnemySript>().DamageCall(Mathf.Floor(att_Damage));
                    m_Target.GetComponent<EnemySript>().multi_Count = 3;
                }
                break;
            case CardType.AttDef:
                {
                    float att_Damage = ((att_Point * 0.7f) + GlobalScript.g_AttPuls) * (att_Up - att_Down);
                    m_Target.GetComponent<EnemySript>().DamageCall(Mathf.Floor(att_Damage));
                    DefenceCall();
                }
                break;
        }

    }

    public void AttackEnd()
    {
        //patt_Bool = true;
    }

    #endregion

    #region ---------- AttUp Funcs

    public void AttUpCall()
    {
        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        eff.GetComponent<EffectScript>().EffectSetting(EffectType.AttUp, this.gameObject);
        SoundScript.Inst.SoundControl("AttackUp");
    }

    #endregion

    #region ---------- Damage Funcs
    public void DamageCall(float a_Damage, EnemySript a_Enemy)
    {
        float m_Damage = a_Damage;
        m_Enemy = a_Enemy;

        if (multi_Count > 0)
            multi_Damage = a_Damage;

        if (def_Save > 0)
        {
            m_Damage = a_Damage - def_Save;
            def_Save -= a_Damage;

            if (m_Damage <= 0)
                m_Damage = 0;

            DefenceReset();
        }

        if (a_Enemy.enemyType == EnemyType.B_Monster)
            a_Enemy.drain_Point = (int)m_Damage;

        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, transform.position, transform.rotation);
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        if(m_Damage > 0)
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

        GlobalScript.g_HealthNow = now_Hp;

        DefenceReset();

        anim.Play(anim_Clip[(int)EnemyClip.Damage].name, -1, 0.0f);

        HpImgCheck(now_Hp, max_Hp);

    }

    public void HpImgCheck(float a_Now, float a_Max)
    {
        if (hp_Img != null)
            hp_Img.fillAmount = a_Now / a_Max;

        if (hp_Text != null)
            hp_Text.text = (int)a_Now + " / " + (int)a_Max;
    }

    public void MultiCheck()
    {
        if (multi_Count > 1)
        {
            multi_Count--;
            DamageCall(Mathf.Floor(multi_Damage), m_Enemy);
        }
    }


    public void DamageEnd()
    {
        if (now_Hp <= 0.0f)
        {
            anim.SetTrigger("Death");
            SoundScript.Inst.SoundControl("Death");
            StageScript.Inst.battle_UI.SetActive(false);
            StageScript.Inst.card_Root.SetActive(false);
        }
            

    }

    public void DeathEnd()
    {
        StageScript.Inst.dead_Obj.SetActive(true);
        SoundScript.Inst.SoundControl("GameOver");
        Destroy(this.gameObject);
    }

    #endregion

    public void EffEndCall(EffectType a_Type)
    {
        BuffScript[] Ref_List = null;
        Ref_List = buff_Root.GetComponentsInChildren<BuffScript>();

        GameObject buff = Instantiate(StageScript.Inst.buff_Obj, buff_Root.transform.position, transform.rotation);
        buff.transform.SetParent(buff_Root.transform);
        buff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        switch (a_Type)
        {
            case EffectType.Defence:
                {
                    float value = Mathf.Floor(def_Save);
                    buff.GetComponent<BuffScript>().BuffSetting(BuffType.Defence, (int)value);
                }
                break;
            case EffectType.AttUp:
                {
                    att_Up = 1.2f;
                    buff.GetComponent<BuffScript>().BuffSetting(BuffType.AttUp, 2);

                    for (int ii = 0; ii < StageScript.Inst.card_List.Count; ii++)
                    {
                        StageScript.Inst.card_List[ii].GetComponent<CardScript>().CardSetting();
                    }
                        
                }
                break;
        }

        for (int ii = 0; ii < Ref_List.Length; ii++)
        {
            if (Ref_List[ii].buffType == buff.GetComponent<BuffScript>().buffType)
                Destroy(Ref_List[ii].gameObject);
        }

        patt_Bool = true;
        StageScript.Inst.turn_Btn.gameObject.SetActive(true);
    }

    public void HeroTurnEnd()
    {
        if (patt_Bool == false)
            return;

        BuffCount(hero_patt);
        StageScript.Inst.EnemyPattCall();
    }

    public void HeroTurnStart()
    {
        def_Save = 0.0f;
        DefenceReset();
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

    void BuffCount(CardType a_Patt)
    {
        BuffScript[] Ref_List = null;
        Ref_List = buff_Root.GetComponentsInChildren<BuffScript>();

        for (int ii = 0; ii < Ref_List.Length; ii++)
        {
            switch (Ref_List[ii].buffType)
            {
                case BuffType.AttUp:
                    {
                        if (a_Patt == CardType.AttUp)
                            return;
                    }
                    break;
            }

            if (Ref_List[ii].buffType == BuffType.Defence)
                return;

            Ref_List[ii].buff_Count--;

            if (Ref_List[ii].buff_Count <= 0)
            {
                switch (Ref_List[ii].buffType)
                {
                    case BuffType.AttUp:
                        {
                            att_Up = 1.0f;
                        }
                        break;
                    case BuffType.AttDown:
                        {
                            att_Down = 0.0f;
                        }
                        break;
                    case BuffType.DefDown:
                        {
                            def_Down = 0.0f;
                        }
                        break;
                }

                for (int kk = 0; kk < StageScript.Inst.card_List.Count; kk++)
                    StageScript.Inst.card_List[kk].GetComponent<CardScript>().CardSetting();

                Destroy(Ref_List[ii].gameObject);
            }
            else
                Ref_List[ii].buff_Txt.text = Ref_List[ii].buff_Count.ToString();
        }
    }
}
