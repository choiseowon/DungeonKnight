using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public abstract class CardClass : MonoBehaviour, IPointerEnterHandler ,IPointerExitHandler, 
                                    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public CardType card_Type = CardType.Count;
    public CardState cardState = CardState.Deck;
    //public string card_Name = "";
    //public string card_Str = "";
    public int card_Mana = -1;
    public Image card_ManaImg = null;
    public Text card_Text = null;
    public Button card_Btn = null;
    public RawImage card_SelecImg = null;
    //public GameObject card_Obj = null;
    public int card_Gold = 0;

    protected Vector3 save_Pos = Vector3.zero;
    public PRS originPRS;

    public void OnPointerClick(PointerEventData data)   // 카드 클릭 시 호출되는 함수
    {
        if (data.button != PointerEventData.InputButton.Left)   // 마우스 왼쪽 버튼인지 확인
            return;

        if (cardState == CardState.Shop)    // 상점 카드일 경우
        {
            if (ShopScript.Inst.ShopCard(card_Gold, this.gameObject) == false)      // 상점 카드가 구매가 되었는 지 확인
                return;
        }
        else if (cardState == CardState.Reward)     // 보상 카드일 경우
        {
            StageScript.Inst.reward_COBtn.gameObject.SetActive(false);      // 카드 보상을 확인하는 버튼을 제거
            StageScript.Inst.reward_CObj.SetActive(false);      // 카드 보상 창을 끔
        }
        else
        {
            return;     // 상점 카드나 보상 카드가 아닐 경우 함수를 빠져나감
        }

        SoundScript.Inst.SfSoundPlay("ItemBuy");        // 사운드 호출
        GlobalScript.g_CardDec.Add(card_Type);      // 카드덱에 카드 추가
        StopScript.Inst.CardNewInput(GlobalScript.g_CardDec.Count - 1);     // 덱 목록에 추가된 카드가 보이도록 추가
        this.gameObject.SetActive(false);       // 해당 카드를 숨김

    }

    public void OnPointerEnter(PointerEventData data)   // 마우스가 카드 안으로 들어 왔을 경우
    {
        if (CardCtrlScript.Inst != null && StateCheck() == true)    // CardCtrlScript가 있는지와 특정 조건이 맞는지 확인
        {
            if (CardCtrlScript.Inst.draw_Bool == true)      // 카드를 드로우 중인지 확인
                return;

            if (CardCtrlScript.Inst.drag_Bool == true)      // 카드를 드래그 중인지 확인
                return;

            if (cardState == CardState.Click || CardCtrlScript.Inst.click_Card != null)       // 선택된 카드가 있는지와 그 카드인지 확인
                return;

            CardCtrlScript.Inst.CardOverParent(this.gameObject);    // 카드를 가장 위에 그려 주기 위해 부모 오브젝트 변경
        }

        card_SelecImg.gameObject.SetActive(true);       // 카드를 표시해 주는 오브젝트 켜기
        this.transform.rotation = Quaternion.identity;  // 각도를 기본 각도로 변경
    }

    public void OnPointerExit(PointerEventData eventData)   // 마우스가 카드를 빠져 나갔을 경우
    {
        if (CardCtrlScript.Inst != null && StateCheck() == true)    // CardCtrlScript가 있는지와 특정 조건이 맞는지 확인
        {
            if (CardCtrlScript.Inst.draw_Bool == true)      // 카드를 드로우 중인지 확인
                return;

            if (CardCtrlScript.Inst.drag_Bool == true)      // 카드를 드래그 중인지 확인
                return;

            if (StageScript.Inst.turnState == TurnState.RewardTurn)     // 보상을 받고 있는 상태인지 확인
                return;

            if (cardState == CardState.Click || CardCtrlScript.Inst.click_Card != null)       // 선택된 카드가 있는지와 그 카드인지 확인
                return;

            CardCtrlScript.Inst.CardListParent(this.gameObject);    // 카드의 부모 오브젝트를 이전의 오브젝트로 변경
            CardCtrlScript.Inst.CardAlignment(false);       // 카드의 각도와 위치를 수정
        }

        card_SelecImg.gameObject.SetActive(false);  // 카드를 표시해 주는 오브젝트 끄기
    }

    public void OnBeginDrag(PointerEventData data)      // 카드를 드래그하기 시작하면 호출되는 함수
    {
        if (data.button != PointerEventData.InputButton.Left)   // 마우스 왼쪽 버튼인지 확인
            return;

        if (StateCheck() == false)      // 특정 조건이 맞는지 확인
            return;

        if (HeroScript.Inst.die_Bool == true)   // 플레이어가 죽었는지 확인
            return;

        if (CardCtrlScript.Inst.drag_Bool == true)  // 드래그 중인 카드가 있는지 확인
            return;

        if (CardCtrlScript.Inst.click_Card != null)     // 선택된 카드인지 확인
            return;

        if (StageScript.Inst.hero_Mana < card_Mana)     // 카드의 필요 마나가 현재 마나보다 큰지 확인
            return;

        CardCtrlScript.Inst.CardOverParent(this.gameObject);    // 카드를 가장 위에 그려 주기 위해 부모 오브젝트 변경
        CardCtrlScript.Inst.drag_Bool = true;       // 드래그 체크 bool 변수를 true로 변경
        save_Pos = this.transform.position;     // 카드가 있던 위치를 저장
        card_SelecImg.gameObject.SetActive(true);       // 카드를 표시해 주는 오브젝트 켜기
    }

    public void OnDrag(PointerEventData data)   // 카드를 드래그 중일 때 호출되는 함수
    {
        if (StateCheck() == false)      // 특정 조건이 맞는지 확인
            return;

        if (HeroScript.Inst.die_Bool == true)   // 플레이어가 죽었는지 확인
            return;

        if (CardCtrlScript.Inst.drag_Bool == false)  // 드래그 중인 카드가 있는지 확인
            return;

        if (CardCtrlScript.Inst.click_Card != null)     // 선택된 카드인지 확인
            return;

        this.transform.rotation = Quaternion.identity;  // 각도를 기본 각도로 변경
        this.transform.position = Input.mousePosition;      // 카드가 마우스를 따라 다니도록 좌표 변경
    }

    public abstract void OnEndDrag(PointerEventData data);  // 드래그가 끝날 때 호출되는 함수를 추상함수로 선언

    public bool StateCheck()    // 조건을 체크하기 위한 함수
    {
        if (cardState == CardState.Shop) return false;      // 카드가 상점 카드일 경우
        if (cardState == CardState.Option) return false;    // 카드가 옵션창의 카드일 경우
        if (cardState == CardState.Reward) return false;    // 카드가 보상 카드일 경우

        return true;
    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)  // PRS 클래스, DOTween 사용 여부, DOTween의 딜레이 값
    {
        if (useDotween)     // DOTween 사용
        {
            this.transform.DOMove(prs.pos, dotweenTime);     // 현재 위치에서 해당 좌표까지 딜레이로 준 시간안에 이동
            this.transform.DORotateQuaternion(prs.rot, dotweenTime);     // 현재 각도부터 해당 각도까지 딜레이 안에 변경
            this.transform.DOScale(prs.scale, dotweenTime);  // 현재 크기에서 해당 크기까지 딜레이 안에 변경
        }
        else
        {
            this.transform.position = prs.pos;       // 해당 좌표로 바로 이동
            this.transform.rotation = prs.rot;       // 해당 각도로 바로 변경
            this.transform.localScale = prs.scale;   // 해당 크기로 바로 변경
        }
    }

    public void ManaCheck(int hero_Mana)
    {
        if (card_Mana > hero_Mana)
            card_ManaImg.gameObject.SetActive(true);
        else
            card_ManaImg.gameObject.SetActive(false);
    }

    public abstract void CardSetting();
}
