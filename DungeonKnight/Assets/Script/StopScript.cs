using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopScript : MonoBehaviour
{
    public static StopScript Inst;

    public GameObject stop_Root = null;
    public Button stop_Btn = null;
    public Button card_OBtn = null;
    public Button main_Btn = null;
    public Button op_Btn = null;
    public Button end_Btn = null;
    public Button return_Btn = null;
    public FadeScript fadeScript = null;

    [Header("----- State -----")]
    public Text att_Txt = null;
    public Text def_Txt = null;
    public Text health_Txt = null;
    public Text gold_Txt = null;

    [Header("----- Card -----")]
    public GameObject card_Obj = null;
    public GameObject card_Root = null;
    public GameObject card_List = null;
    public Button card_CBtn = null;

    [Header("----- Option -----")]
    public GameObject op_Obj = null;
    public Slider bg_Slider = null;
    public Slider sf_Slider = null;
    public Button op_CBtn = null;

    [Header("----- End -----")]
    public GameObject end_Obj = null;
    public Button end_OKBtn = null;
    public Button end_NOBtn = null;


    void Start()
    {
        Inst = this;

        if (stop_Btn != null)
            stop_Btn.onClick.AddListener(() =>
            {
                stop_Root.SetActive(true);
            });

        if (return_Btn != null)
            return_Btn.onClick.AddListener(() =>
            {
                stop_Root.SetActive(false);
            });

        if (card_OBtn != null)
            card_OBtn.onClick.AddListener(() =>
            {
                card_Root.SetActive(true);
            });

        if (main_Btn != null)
            main_Btn.onClick.AddListener(() =>
            {
                FadeCall("StartMain");
            });

        if (end_Btn != null)
            end_Btn.onClick.AddListener(() =>
            {
                end_Obj.SetActive(true);
            });

        if (op_Btn != null)
            op_Btn.onClick.AddListener(() =>
            {
                op_Obj.SetActive(true);
            });

        if (end_OKBtn != null)
            end_OKBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        if (end_NOBtn != null)
            end_NOBtn.onClick.AddListener(() =>
            {
                end_Obj.SetActive(false);
            });

        if (card_CBtn != null)
            card_CBtn.onClick.AddListener(() =>
            {
                card_Root.SetActive(false);
            });

        if (op_CBtn != null)
            op_CBtn.onClick.AddListener(() =>
            {
                op_Obj.SetActive(false);
            });

        bg_Slider.value = GlobalScript.bg_Volume;
        sf_Slider.value = GlobalScript.sf_Volume;

        StateUpdate();
        CardDecList();
    }

    void Update()
    {
        GlobalScript.bg_Volume = bg_Slider.value;
        GlobalScript.sf_Volume = sf_Slider.value;
    }

    public void StateUpdate()
    {
        att_Txt.text = "추가 공격력 + " + GlobalScript.g_AttPuls;
        def_Txt.text = "추가 방어력 + " + GlobalScript.g_DefPuls;
        health_Txt.text = "체력 : " + GlobalScript.g_HealthNow + " / " + GlobalScript.g_HealthMax;
        gold_Txt.text = "소지금 : " + GlobalScript.g_Gold;
    }

    void CardDecList()
    {
        for(int ii = 0; ii < GlobalScript.g_CardDec.Count; ii++)
        {
            GameObject card = Instantiate(card_Obj);
            card.transform.SetParent(card_List.transform);
            card.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            CardScript cardScript = card.GetComponent<CardScript>();
            cardScript.card_Type = GlobalScript.g_CardDec[ii];
            cardScript.cardState = CardState.Option;
            cardScript.CardSetting();
        }
    }

    void FadeCall(string a_SceneName)
    {
        this.gameObject.SetActive(false);

        fadeScript.m_SceneName = a_SceneName;
        fadeScript.fadeState = FadeState.Out;
        fadeScript.FadeEnabled(true);
    }
}
