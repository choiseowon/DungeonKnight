using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EffectType
{
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
    public Text[] eff_Txt;
    Queue<Text> text_Pool = new Queue<Text>();
    public GameObject[] eff_Obj;
    public GameObject target_Obj = null;
    public Outline[] line_Color;
    public RuntimeAnimatorController eff_Runtime;

    IEnumerator[] text_Co = new IEnumerator[3];

    void Start()
    {
        foreach (Text txt in eff_Txt)
            text_Pool.Enqueue(txt);

        for (int ii = 0; ii < 0; ii++)
            text_Co[ii] = null;
    }

    void Update()
    {
    }

    public void TextSetting(string a_Str, Color a_TxtColor, Color a_OutColor)
    {
        Text txt = text_Pool.Dequeue();
        txt.gameObject.SetActive(true);
        txt.text = a_Str;
        txt.color = a_TxtColor;
        line_Color = txt.GetComponents<Outline>();

        for (int ii = 0; ii < line_Color.Length; ii++)
            line_Color[ii].effectColor = a_OutColor;

        eff_Runtime = txt.GetComponent<Animator>().runtimeAnimatorController;

        if (eff_Runtime != null)
        {
            float time = GetTime(eff_Runtime, "Text_Ani");

            time = time * 0.95f;
            int index = 0;

            for(int ii = 0; ii < 0; ii++)
            {
                if (text_Co[ii] == null)
                    index = ii;
            }

            text_Co[index] = TextCo(time, txt.gameObject, index);
            StartCoroutine(text_Co[index]);
        }
    }

    IEnumerator TextCo(float time, GameObject obj, int index)
    {
        yield return new WaitForSeconds(time);
        text_Co[index] = null;
        obj.SetActive(false);
        text_Pool.Enqueue(obj.GetComponent<Text>());
    }

    IEnumerator effect_Co = null;

    public void EffectSetting(EffectType a_Type, GameObject a_Target)
    {
        effectType = a_Type;
        target_Obj = a_Target;

        if (eff_Obj[(int)effectType] != null)
            eff_Obj[(int)effectType].SetActive(true);

        eff_Runtime = eff_Obj[(int)effectType].GetComponent<Animator>().runtimeAnimatorController;
        eff_Obj[(int)effectType].transform.position = target_Obj.transform.position;

        if (eff_Runtime != null)
        {
            float time = GetTime(eff_Runtime, a_Type.ToString());
            time = time * 0.95f;
            effect_Co = EffectCo(time, eff_Obj[(int)effectType]);
            StartCoroutine(effect_Co);
        }
    }

    IEnumerator EffectCo(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.transform.position = this.transform.position;
        obj.SetActive(false);
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

    void OnDisable()
    {
        if(effect_Co != null)
            StopCoroutine(effect_Co);
        
        for(int ii = 0; ii < 0; ii++)
        {
            if (text_Co[ii] != null)
                StopCoroutine(text_Co[ii]);
        }
    }

    public void EffectEnd()
    {
        //if (target_Obj == null)
        //    return;

        //if (effectType == EffectType.Text)
        //    return;

        //if (target_Obj.tag.Contains("Hero") == true)
        //{
        //    target_Obj.GetComponent<HeroScript>().EffEndCall(effectType);
        //}
        //else if (target_Obj.tag.Contains("Enemy") == true)
        //{
        //    target_Obj.GetComponent<EnemySript>().EffEndCall(effectType);
        //}
    }
}