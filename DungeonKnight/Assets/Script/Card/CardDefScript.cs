using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDefScript : CardClass
{
    int def_State = 0;
    int def_Plus = 0;
    float def_Up = 1.0f;

    void Start()
    {
        card_Mana = 1;
    }

    public override void OnEndDrag(PointerEventData data)       // 드래그를 끝나면 발생하는 함수
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
            CardCtrlScript.Inst.CardRemove(this.gameObject);    // 카드 제거 함수 호출
            HeroScript.Inst.PattAdd(card_Type.ToString());         // 카드 타입에 맞는 플레이어 캐릭터의 패턴 추가
        }
        else if (save_Pos != Vector3.zero)
        {
            this.transform.position = save_Pos;
            save_Pos = Vector3.zero;
        }
    }

    public override void CardSetting()
    {
        int realDef_Value = 0;  // 카드에 표시될 대미지의 변수

        def_State = GlobalScript.g_DefState;    // Static으로 선언된 방어력 변수 값을 받아옴
        def_Plus = GlobalScript.g_DefPuls;      // Static으로 선언된 추가 방어력 변수 값을 받아옴

        if (StateCheck() == true)
            def_Up = 1.0f - HeroScript.Inst.def_Down;
        else
            def_Up = 1.0f;

        realDef_Value = (int)Mathf.Floor((def_State + def_Plus) * def_Up);
        card_Text.text = "피해량을 " + realDef_Value + " 방어합니다";
        card_Mana = 1;      // 카드 코스트
        card_Gold = 100;    // 카드 상점 구매가
    }
}
