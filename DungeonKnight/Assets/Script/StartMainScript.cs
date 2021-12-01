using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMainScript : MonoBehaviour
{
    public Button start_Btn = null;
    public Button end_Btn = null;
    public FadeScript fadeScript = null;

    [Header("Pause")]
    public GameObject pause_Obj = null;
    public Button yes_Btn = null;
    public Button no_Btn = null;

    void Start()
    {
        if(GlobalScript.g_SelecCanvas != null)
            Destroy(GlobalScript.g_SelecCanvas.gameObject);

        if (start_Btn != null)
            start_Btn.onClick.AddListener(() =>
            {
                if(fadeScript != null)
                    FadeCall("Select");
            });

        if(end_Btn != null)
            end_Btn.onClick.AddListener(() =>
            {
                if (pause_Obj != null)
                    pause_Obj.SetActive(true);
            });

        if (yes_Btn != null)
            yes_Btn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        if (no_Btn != null)
            no_Btn.onClick.AddListener(() =>
            {
                if (pause_Obj != null)
                    pause_Obj.SetActive(false);
            });

        //CardDecSeting(ValueScript.myCard_List);
        GlobalScript.ValueReset();
    }

    void Update()
    {

    }

    void FadeCall(string a_SceneName)
    {
        fadeScript.m_SceneName = a_SceneName;
        fadeScript.fadeState = FadeState.Out;
        fadeScript.FadeEnabled(true);
    }

    //void CardDecSeting(List<CardValue> myCard_List)
    //{
    //    CardValue cardValue = null;

    //    for (int ii = 0; ii < 3; ii++)
    //    {
    //        cardValue = new CardValue();
    //        cardValue.CardSeting(CardType.Attack);
    //        myCard_List.Add(cardValue);
    //    }

    //    for (int ii = 0; ii < 3; ii++)
    //    {
    //        cardValue = new CardValue();
    //        cardValue.CardSeting(CardType.Defence);
    //        myCard_List.Add(cardValue);
    //    }

    //    cardValue = new CardValue();
    //    cardValue.CardSeting(CardType.AttUp);
    //    myCard_List.Add(cardValue);

    //    cardValue = new CardValue();
    //    cardValue.CardSeting(CardType.AllAttack);
    //    myCard_List.Add(cardValue);

    //    cardValue = new CardValue();
    //    cardValue.CardSeting(CardType.MultiAtt);
    //    myCard_List.Add(cardValue);

    //    cardValue = new CardValue();
    //    cardValue.CardSeting(CardType.AttDef);
    //    myCard_List.Add(cardValue);

    //}
}
