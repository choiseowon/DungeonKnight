using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventScript : MonoBehaviour
{
    public Text click_Txt = null;
    bool event_Bool = true;

    [Header("----- Box -----")]
    public Button box_Btn = null;
    public Text box_Txt = null;
    Animator box_Anim;

    [Header("----- Camp -----")]
    public Button camp_Btn = null;
    public Text camp_Txt = null;


    void Start()
    {
        if (box_Btn != null)
            box_Btn.onClick.AddListener(() =>
            {
                BoxOpen();
            });

        if (camp_Btn != null)
            camp_Btn.onClick.AddListener(() =>
            {
                CampCall();
            });
    }

    void BoxOpen()
    {
        if (event_Bool == false)
            return;

        box_Anim = this.GetComponent<Animator>();

        if(box_Anim != null)
            box_Anim.SetTrigger("Open");

        SoundScript.Inst.SoundControl("BoxOpen");
        SoundScript.Inst.SoundControl("Buy");

        GlobalScript.g_Gold += 500;

        box_Txt.gameObject.SetActive(true);
        click_Txt.gameObject.SetActive(false);

        event_Bool = false;
    }

    void CampCall()
    {
        if (event_Bool == false)
            return;

        GlobalScript.g_HealthNow = GlobalScript.g_HealthMax;

        //GlobalScript.g_HealthNow += GlobalScript.g_HealthMax / 5;

        //if (GlobalScript.g_HealthNow >= GlobalScript.g_HealthMax)
        //    GlobalScript.g_HealthNow = GlobalScript.g_HealthMax;

        HeroScript.Inst.HpImgCheck(GlobalScript.g_HealthNow, GlobalScript.g_HealthMax);
        HeroScript.Inst.anim.SetTrigger("Potion");

        GameObject eff = Instantiate(StageScript.Inst.eff_Obj, HeroScript.Inst.transform.position, transform.rotation);
        eff.transform.SetParent(StageScript.Inst.eff_Root.transform);
        eff.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        eff.GetComponent<EffectScript>().EffectSetting(EffectType.Potion, this.gameObject);
        SoundScript.Inst.SoundControl("Potion");

        camp_Txt.gameObject.SetActive(true);
        click_Txt.gameObject.SetActive(false);
        event_Bool = false;
    }

}
