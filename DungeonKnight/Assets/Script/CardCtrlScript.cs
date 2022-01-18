using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardState
{
    Click,
    NonClick,
    Deck,
    Option,
    Reward,
    Shop
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

public class PRS    // 좌표와 각도 크기를 저장하는 클래스
{
    public Vector3 pos;     // 좌표
    public Quaternion rot;  // 각도
    public Vector3 scale;   // 크기

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}

public class CardCtrlScript : MonoBehaviour
{
    public static CardCtrlScript Inst;
    [HideInInspector] public GameObject click_Card = null;

    [Header("----- Card Value -----")]
    public GameObject deck_Root = null;
    public GameObject card_Root = null;     // 카드 오브젝트의 부모 오브젝트
    public GameObject card_Obj = null;      // 카드 오브젝트 프리팹 적용 변수
    public GameObject over_Root = null;
    int over_Index = -1;
    [HideInInspector] public List<GameObject> card_List = new List<GameObject>();   // 카드 리스트
    List<GameObject> deck_List = new List<GameObject>();
    List<CardType> deck_Type = new List<CardType>();   // 남아있는 카드 체크용 리스트
    public Transform card_Left = null;
    public Transform card_right = null;

    void Start()
    {
        Inst = this;
        CardCreate();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && click_Card != null)      // 선택 중인 카드가 있을 경우 마우스 오른쪽 클릭 시 발생
        {
            ClickCancel(click_Card);        // 선택 중인 카드 취소 함수 호출
        }

    }

    void ClickCancel(GameObject click_Card)     // 선택된 카드를 취소하는 함수
    {
        CardScript cardScript = click_Card.GetComponent<CardScript>();      // 매개변수로 넘어온 오브젝트의 스크립트를 받아옴
        StageScript.Inst.TargetReset();     // 타겟 표시 초기화
        cardScript.cardState = CardState.NonClick;      // 카드의 상태를 변경
        cardScript.EventEndDrag();      // 드래그가 끝날 때 호출되는 함수 호출
    }

    void HandSize()
    {
        if (card_List.Count < 4 || card_List.Count > 7)
            return;

        Vector3 pos = card_Root.transform.position;

        float width_Size = GlobalScript.g_Width;
        float x_Size = width_Size * card_List.Count / 25;

        pos.x -= x_Size;
        card_Left.transform.position = pos;

        pos = card_Root.transform.position;

        pos.x += x_Size;
        card_right.transform.position = pos;
    }

    public void CardIndexSet(GameObject card_Obj)   // 매개변수로 오브젝트를 받아옴
    {
        over_Index = card_Obj.transform.GetSiblingIndex();      // 해당 오브젝트가 부모의 몇번째 자식인지 int로 저장
        card_Obj.transform.SetParent(over_Root.transform);      // 다른 부모로 옮겨서 다른 카드보다 위에 그려지도록 변경
    }

    public void CardIndexGet(GameObject card_Obj)   // 매개변수로 오브젝트를 받아옴
    {
        card_Obj.transform.SetParent(card_Root.transform);      // 부모를 다른 카드 오브젝트와 같은 부모로 변경
        card_Obj.transform.SetSiblingIndex(over_Index);         // 이전에 몇번째 자식인지 저장했던 int 값으로 다시 그 위치로 변경
    }

    public void CardAlignment(bool DO_Bool) // 카드 좌표 및 각도 조정을 위한 함수 매개변수는 DOTween 사용 여부
    {
        List<PRS> originCardPRSs = new List<PRS>();     // 좌표의 값을 저장할 리스트
        originCardPRSs = RoundAlignment(card_Left, card_right, card_List.Count, Vector3.one);   // PRS 클래스를 설정하여 값을 전달

        for (int i = 0; i < card_List.Count; i++)   // 카드 리스트에 있는 수 만큼 반복 실행
        {
            var targetCard = card_List[i].GetComponent<CardScript>();   // 해당 카드의 스크립트를 받아옴
            targetCard.originPRS = originCardPRSs[i];    // 카드의 PRS 클래스 값을 넘겨 줌
            targetCard.MoveTransform(targetCard.originPRS, DO_Bool, 0.5f);  // 카드의 좌표 및 각도 값을 변경하는 함수 호출 DO_Bool이 true면 DOTween을 0.5초 동안 실행
        }

        for (int ii = 0; ii < card_List.Count; ii++)    // 카드 리스트에 있는 수 만큼 반복 실행
        {
            card_List[ii].GetComponent<CardScript>().ManaCheck(StageScript.Inst.hero_Mana);     // 현재 마나 상태를 비교하여 카드가 사용 가능한지 체크
        }
    }

    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, Vector3 scale)  // 왼쪽 기준과 오른쪽 기준을 잡고 좌표 및 각도를 설정
    {
        float[] objLerps = new float[objCount];         // 기준이 되는 길이의 비율을 저장할 값 (0 ~ 1)
        List<PRS> results = new List<PRS>(objCount);    // PRS 클래스를 매개변수 값 만큼 생성

        switch (objCount)   // 매개변수의 값 만큼 반복
        {
            case 1: objLerps = new float[] { 0.5f }; break;             // 카드가 1장이면 0.5의 위치(중간)으로 좌표 설정
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break;     // 카드가 2장일 경우의 좌표
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break; // 카드가 3장일 경우의 좌표
            default:        // 카드가 4장 이상일 경우에 좌표
                float interval = 1f / (objCount - 1);       // 최대 비율인 1을 카드의 개수 -1로 나눈다
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;     // 나눈 길이의 비율을 저장
                break;
        }

        for (int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);   // 왼쪽과 오른쪽 기준까지의 길이의 저장한 비율 위치에 카드를 이동
            var targetRot = Quaternion.identity;    // 카드의 기본 각도
            if (objCount >= 4)  // 카드가 4장 이상일 경우
            {
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);   // 왼쪽과 오른쪽 각도 사이의 저장한 비율 각도로 부드럽게 이동
            }
            results.Add(new PRS(targetPos, targetRot, scale));      // 설정된 PRS 클래스를 리스트에 추가
        }
        return results;     // 리스트를 반환함
    }

    void CardCreate()
    {
        for (int ii = 0; ii < GlobalScript.g_CardDec.Count; ii++)
        {
            CardType a_Type = GlobalScript.g_CardDec[ii];
            deck_Type.Add(a_Type);
        }

        for (int ii = 0; ii < deck_Type.Count; ii++)
        {
            GameObject deck_Card = Instantiate(card_Obj, deck_Root.transform.position, transform.rotation);
            deck_Card.transform.SetParent(deck_Root.transform);
            deck_Card.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            deck_List.Add(deck_Card);

            CardScript deck_Script = deck_Card.GetComponent<CardScript>();
            deck_Script.card_Type = deck_Type[ii];
            deck_Script.CardSetting();
        }

    }

    public void CardReSetting()
    {
        for(int ii = 0; ii < card_List.Count; ii++)
        {
            card_List[ii].GetComponent<CardScript>().CardSetting();
        }

        for(int ii = 0; ii < deck_List.Count; ii++)
        {
            deck_List[ii].GetComponent<CardScript>().CardSetting();
        }
    }

    public void CardDraw(int draw_Index, bool max_Index = false, GameObject turn_Btn = null)   // 매개변수 숫자 만큼 카드 생성 매턴 최대 5장까지 반복
    {
        if (max_Index == true)
            draw_Index = 5 - card_List.Count;

        StartCoroutine(DrawDeleay(draw_Index, turn_Btn));
    }

    public void CardRemove(GameObject card_Obj)     // 사용한 카드를 제거하는 함수
    {
        CardScript cardScript = card_Obj.GetComponent<CardScript>();    // 매개변수로 넘어온 오브젝트이 스크립트를 받아옴
        int card_Mana = cardScript.card_Mana;       // 해당 스크립트의 저장된 마나 변수 값 받아옴
        StageScript.Inst.ManaSetting(card_Mana);    // 받아온 변수로 마나 값 셋팅 함수 호출

        card_Obj.transform.SetParent(deck_Root.transform);      // 부모를 카드덱으로 변경함
        card_Obj.transform.position = deck_Root.transform.position;     // 해당 부모 위치로 옮겨 카드를 숨김
        card_List.Remove(card_Obj);     // 카드 리스트에서 제거
        deck_List.Add(card_Obj);        // 덱 리스트에 추가

        GameObject select_Obj = cardScript.card_SelecImg.gameObject;      // 카드를 표시해 주는 오브젝트
        select_Obj.SetActive(false);    // 해당 오브젝트 끄기
        cardScript.cardState = CardState.Deck;      // 해당 카드의 상태를 덱으로 변경
        click_Card = null;      // 선택된 카드가 없도록 변경

        HandSize();     // 카드를 들고있는 길이 재 설정
        CardAlignment(true);        // 카드 각도와 좌표를 재 설정
    }

    IEnumerator DrawDeleay(int draw_Index, GameObject turn_Btn = null)
    {
        for (int ii = 0; ii < draw_Index; ii++)
        {
            int rand = Random.Range(0, deck_List.Count);       // 남아 있는 카드 리스트 만큼 랜덤 변수 선언
            GameObject draw_Card = deck_List[rand];
            draw_Card.transform.SetParent(card_Root.transform);
            draw_Card.GetComponent<CardScript>().cardState = CardState.NonClick;
            deck_List.Remove(draw_Card);
            card_List.Add(draw_Card);
            //draw_Card.transform.position = card_Root.transform.position;

            HandSize();
            CardAlignment(true);

            yield return new WaitForSeconds(0.5f);
        }

        if (turn_Btn != null)
            turn_Btn.SetActive(true);
    }

}
