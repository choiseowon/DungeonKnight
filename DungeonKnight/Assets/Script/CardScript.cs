using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum CardState
{
    Click,
    NonClick,
    Option,
    Reward
}

public enum CardType
{
    Attack,
    Defence,
    AllAttack,
    MultiAtt,
    AttDef,
    AttUp,
    Count
}

public class CardScript : MonoBehaviour
{
    [Header("Value")]
    public CardType card_Type = CardType.Count;
    public CardState cardState = CardState.NonClick;
    public string card_Name = "";
    public string card_Str = "";
    public int card_Mana = 1;
    int att_State = 10;
    int def_State = 10;
    int att_Plus = 0;
    int def_Plus = 0;
    float att_Up = 1.0f;
    float def_Up = 1.0f;
    public Color32 card_Color = new Color32(255, 255, 255, 255);
    public int card_Gold = 0;

    [Header("UI")]
    public Sprite[] card_Sprite;
    public Image card_ManaImg = null;
    public Image card_MImg = null;
    public Text card_Text = null;
    public Button card_Btn = null;
    public Text card_ManTxt = null;

    [Header("----- Event Trigger -----")]
    EventTrigger eventTrigger;

    void Start()
    {
        eventTrigger = card_Btn.GetComponent<EventTrigger>();

        EventTrigger.Entry entry_Enter = new EventTrigger.Entry();
        entry_Enter.eventID = EventTriggerType.PointerEnter;
        entry_Enter.callback.AddListener((data) =>
        {
            EventEnter();
        });
        eventTrigger.triggers.Add(entry_Enter);

        EventTrigger.Entry entry_Exit = new EventTrigger.Entry();
        entry_Exit.eventID = EventTriggerType.PointerExit;
        entry_Exit.callback.AddListener((data) =>
        {
            EventExit();
        });
        eventTrigger.triggers.Add(entry_Exit);

        if (card_Btn != null)
            card_Btn.onClick.AddListener(() =>
            {
                if(SceneManager.GetActiveScene().name.Contains("Select") == false)
                {
                    if (cardState == CardState.Reward
                        || StageScript.Inst.stageType == StageType.Shop)
                        RewardClick();
                    else
                        EventClick();
                }
            });
    }

    void Update()
    {
    }

    public void EventEnter()
    {
        if(cardState == CardState.Option)
        {
            this.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
            return;
        }

        if (StageScript.Inst.stageType == StageType.Shop)
            return;

        if (StageScript.Inst.turnState == TurnState.EnemyTurn)
            return;

        if (HeroScript.Inst.patt_Bool == false)
            return;

        this.transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
        card_Btn.transform.localPosition = new Vector3(0, 50, 0);
        this.gameObject.transform.SetAsLastSibling();
    }

    public void EventExit()
    {
        if (cardState == CardState.Option)
        {
            this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            return;
        }

        if (cardState == CardState.Click)
            return;

        this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        card_Btn.transform.localPosition = new Vector3(0, 0, 0);
        StageScript.Inst.CardPosReset();

        if (StageScript.Inst.click_Obj != null)
            StageScript.Inst.click_Obj.transform.SetAsLastSibling();


    }

    public void EventClick()        // 카드 사용 함수
    {
        if (cardState == CardState.Option)      // 옵션 창에서 확인하는 카드의 경우 사용 불가
            return;

        if (card_ManaImg.gameObject.activeSelf == true)     // 마나 부족 이미지가 켜져 있으면 사용 불가
            return;

        if(card_Type == CardType.Defence || card_Type == CardType.AttUp || card_Type == CardType.AllAttack)     // 타겟을 선택하지 않는 카드 인지 체크
        {
            if (HeroScript.Inst.patt_Bool == false)     // 플레이어 캐릭터가 패턴을 진행 중이면 함수를 빠져나감
                return;

            HeroScript.Inst.PatternCall(card_Type);         // 카드 타입에 맞는 플레이어 캐릭터의 패턴 함수 호출
            int mana = StageScript.Inst.hero_Mana;              // 플레이어의 마나를 조정하기 위한 변수를 받아옴
            StageScript.Inst.ManaSetting(mana - card_Mana);     // 마나 값 설정 함수 호출
            StageScript.Inst.CardRemove(this.gameObject);       // 카드 삭제 함수 호출
        }
        else
        {
            if (HeroScript.Inst.patt_Bool == false)
                return;

            cardState = CardState.Click;                // 카드가 선택 되어 있음을 체크하는 변수
            HeroScript.Inst.PatternCall(card_Type);         // 카드 타입에 맞는 플레이어 캐릭터의 패턴 함수 호출 
            StageScript.Inst.cardScript = this;             // 선택된 카드의 스크립트를 넘겨줌
            StageScript.Inst.click_Obj = this.gameObject;   // 선택된 카드의 오브젝트를 넘겨줌
        }
    }

    void RewardClick()
    {
        if(StageScript.Inst.stageType == StageType.Shop)
        {
            bool card_Buy = ShopScript.Inst.ShopCard(card_Gold);

            if (card_Buy == true)
            {
                GameObject parent_Obj = this.transform.parent.gameObject;

                if (parent_Obj != null)
                    parent_Obj.SetActive(false);
            }
            else
                return;
        }
        else
        {
            StageScript.Inst.reward_COBtn.gameObject.SetActive(false);
            StageScript.Inst.reward_CObj.SetActive(false);
        }

        SoundScript.Inst.SoundControl("Buy");
        GlobalScript.g_CardDec.Add(card_Type);
    }
    
    public void ManaCheck(int hero_mana)
    {
        if (card_Mana > hero_mana)
            card_ManaImg.gameObject.SetActive(true);
        else
            card_ManaImg.gameObject.SetActive(false);
    }

    public void CardSetting()
    {
        int realAtt_Value = 0;  // 카드에 표시될 대미지의 변수
        int realDef_Value = 0;  // 카드에 표시될 방어막의 변수

        att_State = GlobalScript.g_AttState;    // Static으로 선언된 공격력 변수 값을 받아옴
        def_State = GlobalScript.g_DefState;    // Static으로 선언된 방어력 변수 값을 받아옴

        att_Plus = GlobalScript.g_AttPuls;      // Static으로 선언된 추가 공격력 변수 값을 받아옴
        def_Plus = GlobalScript.g_DefPuls;      // Static으로 선언된 추가 방어력 변수 값을 받아옴

        if (cardState == CardState.NonClick)    // 인게임에서 버프를 값을 적용하기 위한 if문
        {
            att_Up = HeroScript.Inst.att_Up - HeroScript.Inst.att_Down;
            def_Up = HeroScript.Inst.def_Up - HeroScript.Inst.def_Down;
        }

        switch (card_Type)  // 카드 타입에 따라서 변수 값을 다르게 설정
        {
            case CardType.Attack:
                {
                    realAtt_Value = (int)((att_State + att_Plus) * att_Up);
                    card_Str = "적에게 " + realAtt_Value + " 피해를 줍니다";     // 카드에 표시될 텍스트
                    card_Color = new Color32(255, 50, 50, 255);         // 카드 이미지 컬러
                    card_Mana = 1;      // 카드 코스트
                    card_Gold = 100;    // 카드 상점 구매가
                }
                break;

            case CardType.Defence:
                {
                    realDef_Value = (int)((def_State + def_Plus) * def_Up);
                    card_Str = "피해량을 " + realDef_Value + " 방어합니다";
                    card_Color = new Color32(0, 150, 255, 255);
                    card_Mana = 1;
                    card_Gold = 100;
                }
                break;

            case CardType.AllAttack:
                {
                    realAtt_Value = (int)(((att_State * 5 / 10) + att_Plus) * att_Up);
                    card_Str = "모든 적에게 " + realAtt_Value + " 피해를 줍니다";
                    card_Color = new Color32(255, 50, 50, 255);
                    card_Mana = 2;
                    card_Gold = 200;
                }
                break;

            case CardType.MultiAtt:
                {
                    realAtt_Value = (int)(((att_State * 3 / 10) + att_Plus) * att_Up);
                    card_Str = "적에게 " + realAtt_Value + " 피해를 3번 줍니다";
                    card_Color = new Color32(255, 50, 50, 255);
                    card_Mana = 2;
                    card_Gold = 200;
                }
                break;

            case CardType.AttDef:
                {
                    realAtt_Value = (int)(((att_State * 7 / 10) + att_Plus) * att_Up);
                    realDef_Value = (int)(((def_State * 7 / 10) + def_Plus) * def_Up);
                    card_Str = "적에게 " + realAtt_Value + " 피해를 준 후\n" + realDef_Value + " 피해를 막습니다.";
                    card_Color = new Color32(0, 150, 255, 255);
                    card_Mana = 1;
                    card_Gold = 150;
                }
                break;

            case CardType.AttUp:
                {
                    card_Str = "공격력을 20% 증가 시킵니다";
                    card_Color = new Color32(255, 50, 50, 255);
                    card_Mana = 1;
                    card_Gold = 150;
                }
                break;
        }

        card_MImg.sprite = card_Sprite[(int)card_Type];     // 카드 타입에 따른 이미지 설정
        card_MImg.color = card_Color;
        card_Text.text = card_Str;
        card_ManTxt.text = card_Mana.ToString();
    }
}
