using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardAttackScript : CardClass
{    
    int att_State = 0;
    int att_Plus = 0;
    float att_Up = 1.0f;

    void Start()
    {
        card_Mana = 1;
    }

    public override void OnEndDrag(PointerEventData data)      // 드래그를 끝나면 발생하는 함수
    {
        if (StateCheck() == false)      // 특정 조건이 맞는지 확인
            return;

        if (HeroScript.Inst.die_Bool == true)   // 플레이어가 죽었는지 확인
            return;

        if (CardCtrlScript.Inst.drag_Bool == false)  // 드래그 중인 카드가 있는지 확인
            return;

        if (CardCtrlScript.Inst.click_Card != null)     // 선택된 카드인지 확인
            return;

        Vector3 check_Pos = Camera.main.ViewportToScreenPoint(new Vector3(0.0f, 0.35f, 0.0f));      // 화면 비율에 맞는 위치 좌표 저장

        if (this.transform.position.y > check_Pos.y)    // 특정 좌표 이상으로 카드를 이동 시켰다면
        {
            Vector3 stop_Pos = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.3f, 0.0f));      // 화면 비율에 맞는 위치 좌표 저장
            cardState = CardState.Click;                // 카드의 상태를 선택 중으로 변경
            this.transform.position = stop_Pos;     // 카드를 특정 좌표로 이동
            CardCtrlScript.Inst.click_Card = this.gameObject;   // 선택된 카드의 오브젝트를 넘겨줌
            StageScript.Inst.TargetOn();        // 타겟 버튼을 표시해주는 함수 호출
        }
        else if(save_Pos != Vector3.zero)       // 저장된 좌표가 있을 경우
        {
            CardCtrlScript.Inst.drag_Bool = false;      // 드래그 중이 아니라고 변경
            this.transform.position = save_Pos;     // 저장된 위치로 이동
            save_Pos = Vector3.zero;        // 저장된 위치 제거
        }
    }

    public override void CardSetting()
    {
        int realAtt_Value = 0;  // 카드에 표시될 대미지의 변수

        att_State = GlobalScript.g_AttState;    // Static으로 선언된 공격력 변수 값을 받아옴
        att_Plus = GlobalScript.g_AttPuls;      // Static으로 선언된 추가 공격력 변수 값을 받아옴

        if (StateCheck() == true)
            att_Up = HeroScript.Inst.att_Up - HeroScript.Inst.att_Down;
        else
            att_Up = 1.0f;

        realAtt_Value = (int)Mathf.Floor((att_State + att_Plus) * att_Up);
        card_Text.text = "적에게 " + realAtt_Value + " 피해를 줍니다"; ;
        card_Mana = 1;      // 카드 코스트
        card_Gold = 100;    // 카드 상점 구매가
    }

}
