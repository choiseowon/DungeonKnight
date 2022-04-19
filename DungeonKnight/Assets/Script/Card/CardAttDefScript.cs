using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardAttDefScript : CardClass
{
    int att_State = 0;
    int att_Plus = 0;
    float att_Up = 1.0f;

    int def_State = 0;
    int def_Plus = 0;
    float def_Up = 1.0f;

    void Start()
    {
        card_Mana = 1;
    }

    public override void OnEndDrag(PointerEventData data)      // 드래그를 끝나면 발생하는 함수
    {
        if (StateCheck() == false)
            return;

        if (HeroScript.Inst.die_Bool == true)
            return;

        if (CardCtrlScript.Inst.drag_Bool == false)
            return;

        if (CardCtrlScript.Inst.click_Card != null)
            return;

        Vector3 check_Pos = Camera.main.ViewportToScreenPoint(new Vector3(0.0f, 0.35f, 0.0f));      // 화면 비율에 맞는 위치 좌표 저장
        CardCtrlScript.Inst.drag_Bool = false;

        if (this.transform.position.y > check_Pos.y)    // 특정 좌표 이상으로 카드를 이동 시켰다면
        {
            Vector3 stop_Pos = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.3f, 0.0f));      // 화면 비율에 맞는 위치 좌표 저장
            cardState = CardState.Click;                // 카드의 상태를 선택 중으로 변경
            this.transform.position = stop_Pos;     // 카드를 특정 좌표로 이동
            CardCtrlScript.Inst.click_Card = this.gameObject;   // 선택된 카드의 오브젝트를 넘겨줌
            StageScript.Inst.TargetOn();
        }
        else if (save_Pos != Vector3.zero)
        {
            this.transform.position = save_Pos;
            save_Pos = Vector3.zero;
        }
    }

    public override void CardSetting()
    {
        int realAtt_Value = 0;  // 카드에 표시될 대미지의 변수
        int realDef_Value = 0;  // 카드에 표시될 대미지의 변수

        att_State = GlobalScript.g_AttState;    // Static으로 선언된 공격력 변수 값을 받아옴
        att_Plus = GlobalScript.g_AttPuls;      // Static으로 선언된 추가 공격력 변수 값을 받아옴

        def_State = GlobalScript.g_DefState;    // Static으로 선언된 방어력 변수 값을 받아옴
        def_Plus = GlobalScript.g_DefPuls;      // Static으로 선언된 추가 방어력 변수 값을 받아옴

        if (StateCheck() == true)
        {
            att_Up = HeroScript.Inst.att_Up - HeroScript.Inst.att_Down;
            def_Up = 1.0f - HeroScript.Inst.def_Down;
        }
        else
        {
            att_Up = 1.0f;
            def_Up = 1.0f;
        }

        realAtt_Value = (int)Mathf.Floor(((att_State * 7 / 10) + att_Plus) * att_Up);
        realDef_Value = (int)Mathf.Floor(((def_State * 7 / 10) + def_Plus) * def_Up);
        card_Text.text = "적에게 " + realAtt_Value + " 피해를 준 후\n" + realDef_Value + " 피해를 막습니다.";
        card_Mana = 1;      // 카드 코스트
        card_Gold = 150;    // 카드 상점 구매가
    }
}
