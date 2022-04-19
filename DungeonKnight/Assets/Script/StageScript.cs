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
    public Queue<GameObject> monster_Pool = new Queue<GameObject>();

    public static StageScript Inst = null;
    public TurnState turnState = TurnState.HeroTurn;
    public StageType stageType = StageType.Count;
    
    [Header("----- Hero -----")]
    int max_Mana = GlobalScript.g_ManaMax;
    [HideInInspector] public int hero_Mana = 0;

    [Header("----- Enemy -----")]
    public GameObject enemy_Root = null;
    public int enemy_Level = 1;
    [HideInInspector] public List<GameObject> enemy_List = new List<GameObject>();
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
    public Transform[] reward_Tr = new Transform[3];
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
                if (CardCtrlScript.Inst.click_Card != null)
                    return;

                turnState = TurnState.EnemyTurn;
                HeroScript.Inst.PattAdd("TurnEnd");

                if (turnState == TurnState.EnemyTurn)
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
                SoundScript.Inst.SfSoundPlay("ItemBuy");
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
                    SoundScript.Inst.BgmSoundPlay("Normal_Bgm");
                    EnemyCreate();
                    battle_UI.SetActive(true);
                }
                break;
            case StageType.Shop:
                {
                    SoundScript.Inst.BgmSoundPlay("Normal_Bgm");
                    EventCreate(shop_Obj);
                    event_UI.SetActive(true);
                }
                break;
            case StageType.Box:
                {
                    SoundScript.Inst.BgmSoundPlay("Normal_Bgm");
                    EventCreate(box_Obj);
                    event_UI.SetActive(true);
                }
                break;
            case StageType.Camp:
                {
                    SoundScript.Inst.BgmSoundPlay("Normal_Bgm");
                    SoundScript.Inst.SfSoundPlay("Campfire");
                    EventCreate(camp_Obj);
                    event_UI.SetActive(true);
                }
                break;
            case StageType.Elite:
                {
                    SoundScript.Inst.BgmSoundPlay("Elite_Bgm");
                    EnemyCreate();
                    battle_UI.SetActive(true);
                }
                break;
            case StageType.Boss:
                {
                    SoundScript.Inst.BgmSoundPlay("Boss_Bgm");
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
                enemy_List.Add(enemy_Obj);
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
            enemy_List.Add(enemy_Obj);
        }
        else if(stageType == StageType.Boss)
        {
            int rand = (int)EnemyType.B_Monster;
            GameObject enemy_Obj = Instantiate(enemy_Prefab[rand]);

            float posX = (GlobalScript.g_Width * -0.3f) + (GlobalScript.g_Width);

            enemy_Obj.transform.position = new Vector3(posX, GlobalScript.g_Height * 0.6f, 0);
            enemy_Obj.transform.SetParent(enemy_Root.transform);
            enemy_Obj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            enemy_List.Add(enemy_Obj);
        }

        foreach(GameObject obj in enemy_List)
            monster_Pool.Enqueue(obj);
    }

    public void EnemyPattCall()
    {
        TargetReset();

        if (HeroScript.Inst.die_Bool == true)       // 플레이어가 죽었는지 체크
            return;

        if (monster_Pool.Count <= 0)      // 패턴을 진행할 몬스터의 수가 0 보다 작거나 같다면
        {
            turnState = TurnState.HeroTurn;     // 플레이어 턴으로 변경

            HeroScript.Inst.BuffCount();        // 버프 카운트 감소
            ManaSetting(0, true);       // 카드 사용을 위한 마나 초기화
            EnemyPattReset();       // 몬스터 패턴 리셋

            CardCtrlScript.Inst.CardDraw(0, true, turn_Btn.gameObject);     // 카드를 드로우
        }
        else if(monster_Pool.Count > 0)     // 패턴을 진행할 몬스터가 있다면
        {
            GameObject obj = monster_Pool.Dequeue();     // 가장 처음 저장된 몬스터를 꺼내옴
            obj.GetComponent<MonsterClass>().PattCall();     // 몬스터 패턴 호출
        }
    }

    public void EnemyPoolReset()
    {
        monster_Pool.Clear();

        foreach (GameObject obj in enemy_List)
            monster_Pool.Enqueue(obj);
    }

    public void EnemyPattReset()
    {
        monster_Pool.Clear();

        foreach (GameObject obj in enemy_List)
        {
            obj.GetComponent<MonsterClass>().PattSetting();
            monster_Pool.Enqueue(obj);
        }
    }

    public void RewardCall()
    {
        turnState = TurnState.RewardTurn;

        if (stageType == StageType.Boss)
        {
            SoundScript.Inst.BgmSoundPlay("Victory_Bgm");
            SoundScript.Inst.bgm_Audio.loop = false;
            clear_Obj.SetActive(true);
            return;
        }

        SoundScript.Inst.BgmSoundPlay("Clear_Bgm");
        SoundScript.Inst.bgm_Audio.loop = false;
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

        for (int ii = 0; ii < 3; ii++)
        {
            int rand = Random.Range(0, (int)CardType.Count);
            GameObject obj = Instantiate(CardCtrlScript.Inst.card_Obj[rand]);
            obj.transform.SetParent(reward_CObj.transform);
            obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            obj.transform.position = reward_Tr[ii].position;
            CardClass cardClass = obj.GetComponent<CardClass>();
            cardClass.cardState = CardState.Reward;
            cardClass.CardSetting();
        }
    }

    public void TargetOn()
    {
        for (int ii = 0; ii < enemy_List.Count; ii++)
        {
            MonsterClass monsterClass = enemy_List[ii].GetComponent<MonsterClass>();
            if (monsterClass.target_Btn != null)
                monsterClass.target_Btn.image.enabled = true;
        }
    }

    public void TargetReset()
    {
        for(int ii = 0; ii < enemy_List.Count; ii++)
        {
            MonsterClass monsterClass = enemy_List[ii].GetComponent<MonsterClass>();
            if (monsterClass.target_Btn != null)
                monsterClass.target_Btn.image.enabled = false;
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
