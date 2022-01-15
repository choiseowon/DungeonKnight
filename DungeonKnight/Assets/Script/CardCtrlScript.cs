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

public class PRS
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

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
        if (Input.GetKeyDown(KeyCode.A))
        {
            CardDraw(1);
        }

        if (Input.GetMouseButtonDown(1) && click_Card != null)
        {
            ClickCancel(click_Card);
        }

    }

    void ClickCancel(GameObject click_Card)
    {
        CardScript cardScript = click_Card.GetComponent<CardScript>();
        StageScript.Inst.TargetReset();
        cardScript.cardState = CardState.NonClick;
        cardScript.EventEndDrag();
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

    public void CardIndexSet(GameObject card_Obj)
    {
        over_Index = card_Obj.transform.GetSiblingIndex();
        card_Obj.transform.SetParent(over_Root.transform);
    }

    public void CardIndexGet(GameObject card_Obj)
    {
        card_Obj.transform.SetParent(card_Root.transform);
        card_Obj.transform.SetSiblingIndex(over_Index);
    }

    public void CardAlignment(bool DO_Bool)
    {
        List<PRS> originCardPRSs = new List<PRS>();
        originCardPRSs = RoundAlignment(card_Left, card_right, card_List.Count, 0.5f, Vector3.one);

        for (int i = 0; i < card_List.Count; i++)
        {
            var targetCard = card_List[i];
            targetCard.GetComponent<CardScript>().originPRS = originCardPRSs[i];
            targetCard.GetComponent<CardScript>().MoveTransform(targetCard.GetComponent<CardScript>().originPRS, DO_Bool, 0.5f);
        }

        for (int ii = 0; ii < card_List.Count; ii++)
        {
            card_List[ii].GetComponent<CardScript>().ManaCheck(StageScript.Inst.hero_Mana);
        }
    }

    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        for (int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            var targetRot = Quaternion.identity;
            if (objCount >= 4)
            {
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }
        return results;
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
        CardScript cardScript = card_Obj.GetComponent<CardScript>();
        int card_Mana = cardScript.card_Mana;
        StageScript.Inst.ManaSetting(card_Mana);

        card_Obj.transform.SetParent(deck_Root.transform);
        card_Obj.transform.position = deck_Root.transform.position;
        card_List.Remove(card_Obj);
        deck_List.Add(card_Obj);

        GameObject select_Obj = cardScript.card_SelecImg.gameObject;
        select_Obj.SetActive(false);
        cardScript.cardState = CardState.Deck;
        click_Card = null;

        HandSize();
        CardAlignment(true);
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
