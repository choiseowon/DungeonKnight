using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StageType
{
    Monster,
    Shop,
    Box,
    Camp,
    Elite,
    Boss,
    Count
}

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
    
    [Header("----- Hero -----")]
    int max_Mana = GlobalScript.g_ManaMax;
    [HideInInspector] public int hero_Mana = 0;

    [Header("----- Enemy -----")]
    public GameObject enemy_Root = null;
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
    public GameObject eff_Root = null;
    public GameObject eff_Obj = null;
    public GameObject buff_Obj = null;
    public Button turn_Btn = null;
    public Image mana_Img = null;
    public Text mana_Txt = null;
    public Button event_Btn = null;
    public GameObject boss_BGImg = null;

    void Start()
    {
        Inst = this;
        enemy_Level = GlobalScript.g_EnemyLev;
        stageType = GlobalScript.g_StageType;
        if (turn_Btn != null)
            turn_Btn.onClick.AddListener(() =>
            {
                HeroScript.Inst.HeroTurnEnd();

                if(turnState == TurnState.EnemyTurn)
                    turn_Btn.gameObject.SetActive(false);
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
    }

    public void ManaSetting(int a_Mana, bool max_Bool = false)
    {
        if (max_Bool == true)
            hero_Mana = GlobalScript.g_ManaMax;
        else
            hero_Mana -= a_Mana;

        if (mana_Img != null)
            mana_Img.fillAmount = (float)hero_Mana / (float)max_Mana;

        if (mana_Txt != null)
            mana_Txt.text = hero_Mana + " / " + max_Mana;
    }

    void EnemyCreate()
    {
        ManaSetting(0, true);
        CardCtrlScript.Inst.CardDraw(0, true, turn_Btn.gameObject);

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
                enemy_Obj.transform.SetParent(enemy_Root.transform);
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
            enemy_Obj.transform.SetParent(enemy_Root.transform);
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
            enemy_Obj.transform.SetParent(enemy_Root.transform);
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
            HeroScript.Inst.HeroTurnStart();

            if (HeroScript.Inst.die_Bool == true)
                return;

            ManaSetting(0, true);

            for (int ii = 0; ii < enemy_List.Count; ii++)
                enemy_List[ii].patt_Obj.SetActive(true);

            CardCtrlScript.Inst.CardDraw(0, true, turn_Btn.gameObject);

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

        reward_GTxt.text = "골드 - " + reward_Gold.ToString();

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

    public void TargetReset()
    {
        for(int ii = 0; ii < enemy_List.Count; ii++)
        {
            enemy_List[ii].target_Btn.image.enabled = false;
        }
    }

    void EventCreate(GameObject a_EventObj)
    {
        GameObject obj = Instantiate(a_EventObj, enemy_Root.transform.position, transform.rotation);
        obj.transform.position = new Vector3(GlobalScript.g_Width * -0.3f + (GlobalScript.g_Width), GlobalScript.g_Height * 0.6f, 0);
        obj.transform.SetParent(enemy_Root.transform);
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
