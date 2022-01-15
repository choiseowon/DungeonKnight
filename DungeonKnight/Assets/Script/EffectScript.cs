using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EffectType
{
    Text,
    Defence,
    AttUp,
    DefDown,
    AttDown,
    ADUp,
    Perfect,
    Summon,
    Potion,
    Count
}

public class EffectScript : MonoBehaviour
{
    EffectType effectType = EffectType.Count;
    public GameObject[] eff_Obj;
    public GameObject target_Obj = null;
    public Outline[] line_Color;

    void Start()
    {

    }

    void Update()
    {
    }

    public void TextSetting(string a_Str, Color a_TxtColor, Color a_OutColor)
    {
        effectType = EffectType.Text;

        if (eff_Obj[(int)effectType] != null)
        {
            Text txt = eff_Obj[(int)effectType].transform.Find("Text_Eff").GetComponent<Text>();
            eff_Obj[(int)effectType].SetActive(true);
            txt.text = a_Str;
            txt.color = a_TxtColor;
            line_Color = txt.GetComponents<Outline>();

            for (int ii = 0; ii < line_Color.Length; ii++)
                line_Color[ii].effectColor = a_OutColor;

        }
    }

    public void EffectSetting(EffectType a_Type, GameObject a_Target)
    {
        effectType = a_Type;
        target_Obj = a_Target;

        if (eff_Obj[(int)effectType] != null)
            eff_Obj[(int)effectType].SetActive(true);
    }

    public void EffectEnd()
    {
        if (target_Obj == null)
            return;

        if (effectType == EffectType.Text)
            return;

        if (target_Obj.tag.Contains("Hero") == true)
        {
            target_Obj.GetComponent<HeroScript>().EffEndCall(effectType);
        }
        else if (target_Obj.tag.Contains("Enemy") == true)
        {
            target_Obj.GetComponent<EnemySript>().EffEndCall(effectType);
        }
    }
}