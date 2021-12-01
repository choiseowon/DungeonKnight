using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum TurnState
{
    HeroTurn,
    EnemyTurn,
    RewardTurn
}

public class StageScript : MonoBehaviour
{
    public static StageScript Inst = null;
    public TurnState turnState = TurnState.HeroTurn;
    public StageType stageType = StageType.Count;
    [HideInInspector] public CardScript cardScript = null;

    [Header("----- Card Value -----")]
    public GameObject card_Root = null;     // 카드 오브젝트의 부모 오브젝트
    public GameObject card_Obj = null;      // 카드 오브젝트 프리팹 적용 변수
    [HideInInspector] public List<GameObject> card_List = new List<GameObject>();   // 카드 리스트
    List<CardType> deck_CList = new List<CardType>();   // 남아있는 카드 체크용 리스트

    [Header("----- Hero -----")]
    public GameObject hero_Obj = null;
    public GameObject char_Root = null;
    int max_Mana = GlobalScript.g_ManaMax;
    [HideInInspector] public int hero_Mana = 0;

    [Header("----- Enemy -----")]
    public int enemy_Level = 1;
    [HideInInspector] public List<EnemySript> enemy_List = new List<EnemySript>();
    public GameObject[] enemy_Prefab = null;
    public int pattCount = 0;

    [Header("----- Reward -----")]
    public GameObject reward_Root = null;
    public Button reward_COBtn = null;
    public Button reward_GBtn = null;
    public Text reward_GTxt = null;
    public Button reward_RBtn = null;
    public GameObject reward_CObj = null;
    public Button reward_CCBtn = null;
    public GameObject[] reward_Card = new GameObject[3];
    int reward_Gold = 0;

    [Header("----- Shop -----")]
    public GameObject shop_Obj = null;

    [Header("----- Event -----")]
    public GameObject camp_Obj = null;
    public GameObject box_Obj = null;

    [Header("----- Dead -----")]
    public GameObject dead_Obj = null;
    public Button dead_Btn = null;

    [Header("----- Clear -----")]
    public GameObject clear_Obj = null;
    public Button clear_Btn = null;

    [Header("----- UI -----")]
    public GameObject battle_UI = null;
    public GameObject event_UI = null;
    public FadeScript fadeScript = null;
    public Canvas m_Canvas = null;
    [HideInInspector] public GameObject click_Obj = null;
    public GameObject eff_Root = null;
    public GameObject eff_Obj = null;
    public GameObject buff_Obj = null;
    public Button turn_Btn = null;
    public Image mana_Img = null;
    public Text mana_Txt = null;
    public Button bg_Btn = null;
    public Button event_Btn = null;
    public GameObject boss_BGImg = null;

    void Start()
    {
        Inst = this;
        HeroCreate();
        enemy_Level = GlobalScript.g_EnemyLev;
        stageType = GlobalScript.g_StageType;
        if (turn_Btn != null)
            turn_Btn.onClick.AddListener(() =>
            {
                HeroScript.Inst.HeroTurnEnd();

                if(turnState == TurnState.EnemyTurn)
                    turn_Btn.gameObject.SetActive(false);
            });

        if (bg_Btn != null)
            bg_Btn.onClick.AddListener(() =>
            {
                if (cardScript != null)
                {
                    cardScript.cardState = CardState.NonClick;
                    cardScript.EventExit();
                    for (int ii = 0; ii < enemy_List.Count; ii++)
                    {
                        enemy_List[ii].target_Btn.image.enabled = false;
                    }
                    HeroScript.Inst.target_Check = false;
                    cardScript = null;
                    click_Obj = null;
                    CardPosReset();
                }
            });

        if (reward_COBtn != null)
            reward_COBtn.onClick.AddListener(() =>
            {
                reward_CObj.SetActive(true);
            });

        if (reward_CCBtn != null)
            reward_CCBtn.onClick.AddListener(() =>
            {
                reward_CObj.SetActive(false);
            });

        if (reward_GBtn != null)
            reward_GBtn.onClick.AddListener(() =>
            {
                SoundScript.Inst.SoundControl("Buy");
                GlobalScript.g_Gold += reward_Gold;
                reward_GBtn.gameObject.SetActive(false);
            });

        if (reward_RBtn != null)
            reward_RBtn.onClick.AddListener(() =>
            {
                FadeCall("Select");
            });

        if (event_Btn != null)
            event_Btn.onClick.AddListener(() =>
            {
                FadeCall("Select");
            });

        if (dead_Btn != null)
            dead_Btn.onClick.AddListener(() =>
            {
                FadeCall("StartMain");
            });

        if (clear_Btn != null)
            clear_Btn.onClick.AddListener(() =>
            {
                FadeCall("StartMain");
            });

        switch (stageType)
        {
            case StageType.Monster:
                {
                    SoundScript.Inst.SoundControl("Normal");
                    EnemyCreate();
                    battle_UI.SetActive(true);
                }
                break;
            case StageType.Shop:
                {
                    SoundScript.Inst.SoundControl("Normal");
                    EventCreate(shop_Obj);
                    event_UI.SetActive(true);
                }
                break;
            case StageType.Box:
                {
                    SoundScript.Inst.SoundControl("Normal");
                    EventCreate(box_Obj);
                    event_UI.SetActive(true);
                }
                break;
            case StageType.Camp:
                {
                    SoundScript.Inst.SoundControl("Normal");
                    SoundScript.Inst.SoundControl("Camping");
                    EventCreate(camp_Obj);
                    event_UI.SetActive(true);
                }
                break;
            case StageType.Elite:
                {
                    SoundScript.Inst.SoundControl("Elite");
                    EnemyCreate();
                    battle_UI.SetActive(true);
                }
                break;
            case StageType.Boss:
                {
                    SoundScript.Inst.SoundControl("Boss");
                    boss_BGImg.SetActive(true);
                    EnemyCreate();
                    battle_UI.SetActive(true);
                }
                break;
        }
    }

    void Update()
    {
        if (stageType == StageType.Shop)
            return;
        
        for (int ii = 0; ii < card_List.Count; ii++)
        {
            float sizeX = card_List[ii].GetComponent<RectTransform>().rect.width;
            float posX = sizeX * (ii - ((card_List.Count - 1) / 2.0f));
            float spacingX = 0.0f;

            if (card_List.Count <= 5)
                spacingX = sizeX * 0.125f;

            spacingX = spacingX * ii;
            card_List[ii].transform.localPosition = new Vector3(posX + spacingX, 0, 0);
        }

    }

    public void ManaSetting(int a_Mana)
    {
        hero_Mana = a_Mana;

        if (mana_Img != null)
            mana_Img.fillAmount = (float)hero_Mana / (float)max_Mana;

        if (mana_Txt != null)
            mana_Txt.text = hero_Mana + " / " + max_Mana;

        for(int ii = 0; ii < card_List.Count; ii++)
        {
            card_List[ii].GetComponent<CardScript>().ManaCheck(hero_Mana);
        }
    }

    public void CardPosReset()
    {
        for (int ii = 0; ii < card_List.Count; ii++)
        {
            card_List[ii].transform.SetSiblingIndex(ii);
        }
    }

    public void CardRemove(GameObject card_Obj)     // 사용한 카드를 제거하는 함수
    {
        int index = 0;      // 리스트에서 몇 번째 인덱스인지 저장할 변수

        for(int ii = 0; ii < card_List.Count; ii++)
        {
            if (card_List[ii] == card_Obj)      // 사용한 카드를 카드 리스트에서 비교 후 인덱스 저장
                index = ii;
        }

        card_List.Remove(card_List[index]);     // 해당 인덱스의 오브젝트를 리스트에서 제거
        deck_CList.Add(card_Obj.GetComponent<CardScript>().card_Type);      // 제거된 오브젝트이 카드 타입을 남아있는 카드 리스트에 추가
        Destroy(card_Obj);      // 해당 오브젝트 삭제

    }

    void CardDraw(int draw_index)   // 매개변수 숫자 만큼 카드 생성 매턴 최대 5장까지 반복
    {
        for (int ii = 0; ii < draw_index; ii++)
        {
            GameObject view_obj = Instantiate(card_Obj, card_Root.transform.position, transform.rotation);  // 오브젝트 생성
            view_obj.transform.SetParent(card_Root.transform);
            view_obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            card_List.Add(view_obj);                // 카드 리스트에 해당 오브젝트 추가

            int rand = Random.Range(0, deck_CList.Count);       // 남아 있는 카드 리스트 만큼 랜덤 변수 선언
            view_obj.GetComponent<CardScript>().card_Type = deck_CList[rand];       // 생성된 카드의 타입을 랜덤으로 정해진 타입으로 설정
            view_obj.GetComponent<CardScript>().CardSetting();                      // 카드의 타입에 맞게 카드의 변수 설정 함수 호출
            deck_CList.Remove(deck_CList[rand]);                // 설정된 카드 타입은 남아있는 카드 리스트에서 제거
        }
    }

    void HeroCreate()
    {
        GameObject obj = Instantiate(hero_Obj, char_Root.transform.position, transform.rotation);
        obj.transform.position = new Vector3(GlobalScript.g_Width * 0.15f, GlobalScript.g_Height * 0.6f, 0);
        obj.transform.SetParent(char_Root.transform);
        obj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    void EnemyCreate()
    {
        ManaSetting(max_Mana);

        for (int ii = 0; ii < GlobalScript.g_CardDec.Count; ii++)
        {
            CardType m_Type = GlobalScript.g_CardDec[ii];
            deck_CList.Add(m_Type);
        }

        CardDraw(5);

        if (stageType == StageType.Monster)
        {
            for (int ii = 0; ii < enemy_Level; ii++)
            {
                int rand = Random.Range(0, (int)EnemyType.N_Knight + 1);
                GameObject enemy_Obj = Instantiate(enemy_Prefab[rand]);

                float posX = 0.0f;
                if (enemy_Level == 1)
                    posX = (GlobalScript.g_Width * -0.3f) + (GlobalScript.g_Width);
                else if (enemy_Level == 2)
                    posX = (GlobalScript.g_Width * (-0.4f + (0.2f * ii))) + (GlobalScript.g_Width);
                else
                    posX = (GlobalScript.g_Width * (-0.45f + (0.15f * ii))) + (GlobalScript.g_Width);

                enemy_Obj.transform.position = new Vector3(posX, GlobalScript.g_Height * 0.6f, 0);
                enemy_Obj.transform.SetParent(char_Root.transform);
                enemy_Obj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                enemy_Obj.GetComponent<EnemySript>().enemyType = (EnemyType)rand;
                enemy_List.Add(enemy_Obj.GetComponent<EnemySript>());
            }
        }
        else if(stageType == StageType.Elite)
        {
            int rand = Random.Range((int)EnemyType.E_Thief, (int)EnemyType.E_Knight + 1);
            GameObject enemy_Obj = Instantiate(enemy_Prefab[rand]);

            float posX = (GlobalScript.g_Width * -0.3f) + (GlobalScript.g_Width);

            enemy_Obj.transform.position = new Vector3(posX, GlobalScript.g_Height * 0.6f, 0);
            enemy_Obj.transform.SetParent(char_Root.transform);
            enemy_Obj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            enemy_Obj.GetComponent<EnemySript>().enemyType = (EnemyType)rand;
            enemy_List.Add(enemy_Obj.GetComponent<EnemySript>());
        }
        else if(stageType == StageType.Boss)
        {
            int rand = (int)EnemyType.B_Monster;
            GameObject enemy_Obj = Instantiate(enemy_Prefab[rand]);

            float posX = (GlobalScript.g_Width * -0.3f) + (GlobalScript.g_Width);

            enemy_Obj.transform.position = new Vector3(posX, GlobalScript.g_Height * 0.6f, 0);
            enemy_Obj.transform.SetParent(char_Root.transform);
            enemy_Obj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            enemy_Obj.GetComponent<EnemySript>().enemyType = (EnemyType)rand;
            enemy_List.Add(enemy_Obj.GetComponent<EnemySript>());
        }
    }

    public void EnemyPattCall()
    {

        if(turnState == TurnState.HeroTurn)
        {
            turnState = TurnState.EnemyTurn;

            for (int ii = 0; ii < enemy_List.Count; ii++)
                enemy_List[ii].patt_Bool = true;
        }

        if (pattCount >= enemy_List.Count)
        {
            pattCount = 0;
            turnState = TurnState.HeroTurn;
            turn_Btn.gameObject.SetActive(true);
            HeroScript.Inst.HeroTurnStart();
            ManaSetting(max_Mana);

            for (int ii = 0; ii < enemy_List.Count; ii++)
                enemy_List[ii].patt_Obj.SetActive(true);

            while (card_List.Count < 5)
            {
                CardDraw(1);
            }

            return;
        }

        for (int ii = 0; ii < enemy_List.Count; ii++)
        {
            if(enemy_List[ii].patt_Bool == true)
            {
                enemy_List[ii].PatternCall();
                pattCount++;

                break;
            }
        }
    }

    public void RewardCall()
    {
        turnState = TurnState.RewardTurn;

        if (stageType == StageType.Boss)
        {
            SoundScript.Inst.SoundControl("Victory");
            clear_Obj.SetActive(true);
            return;
        }

        SoundScript.Inst.SoundControl("Clear");
        reward_Root.SetActive(true);

        if (stageType == StageType.Elite)
        {
            enemy_Level += 1;

            if (enemy_Level > 3)
                enemy_Level = 3;

            GlobalScript.g_EnemyLev = enemy_Level;
            reward_Gold = 500;
        }
        else
        {
            reward_Gold = enemy_Level * 100;
        }

        reward_GTxt.text = "Gold - " + reward_Gold.ToString();

        if (enemy_Level <= 1)
        {
            enemy_Level += 1;
            GlobalScript.g_EnemyLev = enemy_Level;
        }

        for (int ii = 0; ii < reward_Card.Length; ii++)
        {
            int rand = Random.Range(0, (int)CardType.Count);

            reward_Card[ii].GetComponent<CardScript>().card_Type = (CardType)rand;
            reward_Card[ii].GetComponent<CardScript>().cardState = CardState.Reward;
            reward_Card[ii].GetComponent<CardScript>().CardSetting();
        }
    }

    void EventCreate(GameObject a_EventObj)
    {
        GameObject obj = Instantiate(a_EventObj, char_Root.transform.position, transform.rotation);
        obj.transform.position = new Vector3(GlobalScript.g_Width * -0.3f + (GlobalScript.g_Width), GlobalScript.g_Height * 0.6f, 0);
        obj.transform.SetParent(char_Root.transform);
        obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    void FadeCall(string a_SceneName)
    {
        Inst = null;
        HeroScript.Inst = null;
        fadeScript.m_SceneName = a_SceneName;
        fadeScript.fadeState = FadeState.Out;
        fadeScript.FadeEnabled(true);
    }
}
