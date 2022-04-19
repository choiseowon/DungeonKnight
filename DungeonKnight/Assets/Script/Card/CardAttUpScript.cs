using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardAttUpScript : CardClass
{
    void Start()
    {
        card_Mana = 1;
    }

    public override void OnEndDrag(PointerEventData data)        // 드래그를 끝나면 발생하는 함수
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
        card_Text.text = "공격력을 20% 증가 시킵니다";
        card_Mana = 1;      // 카드 코스트
        card_Gold = 150;    // 카드 상점 구매가
    }
}
