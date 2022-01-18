using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardScript : MonoBehaviour
{
    [Header("Value")]
    public CardType card_Type = CardType.Count;
    public CardState cardState = CardState.Deck;
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
    public RawImage card_SelecImg = null;

    EventTrigger eventTrigger;      // 이벤트 트리거 생성

    public PRS originPRS;

    void Start()
    {
        eventTrigger = card_Btn.GetComponent<EventTrigger>();       // 카드에 있는 이벤트 트리거 받아오기

        EventTrigger.Entry entry_Enter = new EventTrigger.Entry();  // 이벤트 트리거에 추가할 Entry 생성
        entry_Enter.eventID = EventTriggerType.PointerEnter;    // Entry 타입을 PointerEnter 로 설정
        entry_Enter.callback.AddListener((data) =>
        {
            EventEnter();       // PointerEnter가 되면 발생 시킬 함수
        });
        eventTrigger.triggers.Add(entry_Enter);     // 해당 Entry를 이벤트 트리거에 추가

        EventTrigger.Entry entry_Exit = new EventTrigger.Entry();  // 이벤트 트리거에 추가할 Entry 생성
        entry_Exit.eventID = EventTriggerType.PointerExit;    // Entry 타입을 PointerExit 로 설정
        entry_Exit.callback.AddListener((data) =>
        {
            EventExit();       // PointerExit 되면 발생 시킬 함수
        });
        eventTrigger.triggers.Add(entry_Exit);     // 해당 Entry를 이벤트 트리거에 추가

        EventTrigger.Entry enty_BeginDrag = new EventTrigger.Entry();  // 이벤트 트리거에 추가할 Entry 생성
        enty_BeginDrag.eventID = EventTriggerType.BeginDrag;    // Entry 타입을 BeginDrag 로 설정
        enty_BeginDrag.callback.AddListener((data) =>
        {
            EventBeginDrag();       // BeginDrag 되면 발생 시킬 함수
        });
        eventTrigger.triggers.Add(enty_BeginDrag);     // 해당 Entry를 이벤트 트리거에 추가

        EventTrigger.Entry enty_Drag = new EventTrigger.Entry();  // 이벤트 트리거에 추가할 Entry 생성
        enty_Drag.eventID = EventTriggerType.Drag;    // Entry 타입을 Drag 로 설정
        enty_Drag.callback.AddListener((data) =>
        {
            EventDrag();       // Drag 되면 발생 시킬 함수
        });
        eventTrigger.triggers.Add(enty_Drag);     // 해당 Entry를 이벤트 트리거에 추가

        EventTrigger.Entry enty_EndDrag = new EventTrigger.Entry();  // 이벤트 트리거에 추가할 Entry 생성
        enty_EndDrag.eventID = EventTriggerType.EndDrag;    // Entry 타입을 EndDrag 로 설정
        enty_EndDrag.callback.AddListener((data) =>
        {
            EventEndDrag();       // EndDrag 되면 발생 시킬 함수
        });
        eventTrigger.triggers.Add(enty_EndDrag);     // 해당 Entry를 이벤트 트리거에 추가


        if (card_Btn != null)
            card_Btn.onClick.AddListener(() =>
            {
                if (cardState == CardState.Reward || cardState == CardState.Shop)
                    RewardClick();
            });

    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)  // PRS 클래스, DOTween 사용 여부, DOTween의 딜레이 값
    {
        if (useDotween)     // DOTween 사용
        {
            transform.DOMove(prs.pos, dotweenTime);     // 현재 위치에서 해당 좌표까지 딜레이로 준 시간안에 이동
            transform.DORotateQuaternion(prs.rot, dotweenTime);     // 현재 각도부터 해당 각도까지 딜레이 안에 변경
            transform.DOScale(prs.scale, dotweenTime);  // 현재 크기에서 해당 크기까지 딜레이 안에 변경
        }
        else
        {
            transform.position = prs.pos;       // 해당 좌표로 바로 이동
            transform.rotation = prs.rot;       // 해당 각도로 바로 변경
            transform.localScale = prs.scale;   // 해당 크기로 바로 변경
        }
    }

    void Update()
    {
    }

    public void EventEnter()        // 카드에 마우스를 올리면 발생하는 함수
    {
        card_SelecImg.gameObject.SetActive(true);       // 카드를 표시해 주는 오브젝트 켜기
        this.transform.rotation = Quaternion.identity;  // 각도를 기본 각도로 변경

        if(cardState == CardState.NonClick)     // 카드의 상태가 선택이 안된 카드 일 경우
        {
            CardCtrlScript.Inst.CardIndexSet(this.gameObject);      // 카드를 맨위로 그리기 위한 함수 호출
        }
    }

    public void EventExit()     // 카드에서 마우스가 빠져 나가면 발생하는 함수
    {
        if (cardState == CardState.Click)       // 선택 중인 카드 일 경우 제외
            return;

        card_SelecImg.gameObject.SetActive(false);  // 카드를 표시해 주는 오브젝트 끄기

        if (cardState == CardState.NonClick)    // 카드가 선택이 안된 카드 일 경우
        {
            if (cardState != CardState.Deck)    // 카드가 덱에 있는 카드가 아닐 경우
                CardCtrlScript.Inst.CardIndexGet(this.gameObject);      // 카드를 기존에 있던 위치로 옮겨 그리는 순서 바꾸기

            CardCtrlScript.Inst.CardAlignment(false);   // 카드의 위치 및 각도 재 설정
        }
            
    }

    public void EventBeginDrag()        // 카드를 드래그하기 시작할 때 발생하는 함수
    {
        if (card_ManaImg.gameObject.activeSelf == true)     // 카드 마나부족 표시가 활성화되 있으면 제외
            return;

        if (cardState != CardState.NonClick)    // 선택된 카드의 경우 제외
            return;

        if (HeroScript.Inst.patt_Bool == false)     // 주인공 캐릭터가 패턴 진행 중이면 제외
            return;

        card_SelecImg.gameObject.SetActive(true);       // 카드를 표시해 주는 오브젝트 켜기
        StageScript.Inst.turn_Btn.gameObject.SetActive(false);      // 턴 종료 버튼 숨기기
    }

    public void EventDrag()     // 카드가 드래그 중일 경우 발생하는 함수
    {
        if (card_ManaImg.gameObject.activeSelf == true)     // 카드 마나부족 표시가 활성화되 있으면 제외
            return;

        if (cardState != CardState.NonClick)    // 선택된 카드의 경우 제외
            return;

        if (HeroScript.Inst.patt_Bool == false)     // 주인공 캐릭터가 패턴 진행 중이면 제외
            return;

        this.transform.position = Input.mousePosition;      // 카드가 마우스를 따라 다니도록 좌표 변경
    }

    public void EventEndDrag()      // 드래그를 끝나면 발생하는 함수
    {
        if (card_ManaImg.gameObject.activeSelf == true)     // 카드 마나부족 표시가 활성화되 있으면 제외
            return;

        if (cardState != CardState.NonClick)    // 선택된 카드의 경우 제외
            return;

        if (HeroScript.Inst.patt_Bool == false)     // 주인공 캐릭터가 패턴 진행 중이면 제외
            return;

        Vector3 check_Pos = Camera.main.ViewportToScreenPoint(new Vector3(0.0f, 0.35f, 0.0f));      // 화면 비율에 맞는 위치 좌표 저장

        if (this.transform.position.y > check_Pos.y)    // 특정 좌표 이상으로 카드를 이동 시켰다면
        {
            if (card_ManaImg.gameObject.activeSelf == true)     // 마나 부족 이미지가 켜져 있으면 사용 불가
                return;

            if (card_Type == CardType.Defence || card_Type == CardType.AttUp || card_Type == CardType.AllAttack)     // 타겟을 선택하지 않는 카드 인지 체크
            {
                if (HeroScript.Inst.patt_Bool == false)     // 플레이어 캐릭터가 패턴을 진행 중이면 함수를 빠져나감
                    return;

                HeroScript.Inst.PatternCall(card_Type);         // 카드 타입에 맞는 플레이어 캐릭터의 패턴 함수 호출
                CardCtrlScript.Inst.CardRemove(this.gameObject);    // 카드 제거 함수 호출
            }
            else
            {
                if (HeroScript.Inst.patt_Bool == false)     // 플레이어 캐릭터가 패턴을 진행 중이면 함수를 빠져나감
                    return;

                Vector3 stop_Pos = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.3f, 0.0f));      // 화면 비율에 맞는 위치 좌표 저장
                cardState = CardState.Click;                // 카드의 상태를 선택 중으로 변경
                HeroScript.Inst.PatternCall(card_Type);         // 카드 타입에 맞는 플레이어 캐릭터의 패턴 함수 호출
                this.transform.position = stop_Pos;     // 카드를 특정 좌표로 이동
                CardCtrlScript.Inst.click_Card = this.gameObject;   // 선택된 카드의 오브젝트를 넘겨줌
                EventEnter();        // 카드에 마우스를 올리면 발생하는 함수
            }
        }
        else
        {
            card_SelecImg.gameObject.SetActive(false);  // 카드를 표시해 주는 오브젝트 끄기
            StageScript.Inst.turn_Btn.gameObject.SetActive(true);   // 턴 죵료 버튼 표시해주기
            CardCtrlScript.Inst.click_Card = null;      // 선택된 카드 오브젝트가 없도록 변경
            CardCtrlScript.Inst.CardIndexGet(this.gameObject);      // 카드를 기존에 있던 위치로 옮겨 그리는 순서 바꾸기
            CardCtrlScript.Inst.CardAlignment(false);   // 카드의 위치 및 각도 재 설정
        }
    }

    void RewardClick()
    {
        if(cardState == CardState.Shop)
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

        att_Up = 1.0f;
        def_Up = 1.0f;

        if (cardState != CardState.Option && cardState != CardState.Reward && cardState != CardState.Shop)    // 인게임에서 버프를 값을 적용하기 위한 if문
        {
            if(HeroScript.Inst != null)
            {
                att_Up = HeroScript.Inst.att_Up - HeroScript.Inst.att_Down;
                def_Up = HeroScript.Inst.def_Up - HeroScript.Inst.def_Down;
            }
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
