using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum MonState
{
    Patt,
    Damage,
    Death,
    Count
}

public abstract class CharactorClass : MonoBehaviour
{
    protected float max_Hp = 0.0f;      // 캐릭터의 최대 체력
    protected float now_Hp = 0.0f;      // 캐릭터의 현재 체력

    public Animator anim = null;
    public AnimationClip[] anim_Clip = null;

    public GameObject buff_Root = null;
    public GameObject buff_Obj = null;
    protected Dictionary<BuffType, float> buff_ValueDict = new Dictionary<BuffType, float>();
    protected Dictionary<BuffType, GameObject> buff_ViewDict = new Dictionary<BuffType, GameObject>();
    public EffectScript eff_Root = null;
    public HpUiScript hp_Ui = null;

    protected string patt_Str = "";
    protected RuntimeAnimatorController anim_Runtime = null;
    protected RuntimeAnimatorController eff_Runtime;

    public abstract void PattCall();
}

public abstract class MonsterClass : CharactorClass
{
    public MonState monState = MonState.Count;
    //public EnemyType enemyType = EnemyType.Count;
    protected List<string> patt_List = new List<string>();
    public GameObject patt_Obj = null;
    public Button target_Btn = null;
    protected bool damage_Bool = false;
    protected bool patt_Bool = false;
    protected BuffType buffType = BuffType.Count;
    protected float buff_Value = 0.0f;

    public void TargetFunc()
    {
        target_Btn.onClick.AddListener(() =>
        {
            StageScript.Inst.TargetReset();
            HeroScript.Inst.target_Pool.Enqueue(this.gameObject);
            CardCtrlScript.Inst.drag_Bool = false;
            HeroScript.Inst.PattAdd(CardCtrlScript.Inst.click_Card.GetComponent<CardClass>().card_Type.ToString());
            CardCtrlScript.Inst.CardRemove(CardCtrlScript.Inst.click_Card);
        });
    }

    public abstract void PattSetting();
}

public interface IAttack
{
    float att_Point { get; set; }
    void Attack();
}

public interface IDefence
{
    float def_Point { get; set; }
    float def_Save { get; set; }
    void Defence();
}

public interface IAllAttack
{
    void AllAttack();
}

public interface IMultiAtt
{
    void MultiAtt();
}

public interface IAttDef
{
    void AttDef();
}

public interface IDrain
{
    void Drain();
}

public interface IAttUp
{
    float att_Up { get; set; }
    void AttUp();
}

public interface IADUp
{
    float att_Up { get; set; }
    float def_Up { get; set; }
    void ADUp();
}

public interface IPerfect
{
    bool perfect_Bool { get; set; }
    void Perfect();
}

public interface ISummon
{
    int summon_Count { get; set; }
    GameObject summon_Prefab { get; set; }
    GameObject[] summon_Obj { get; set; }
    void Summon();
}

public interface IAttDown
{
    float att_Down { get; set; }
    void AttDown();
}

public interface IDefDown
{
    float def_Down { get; set; }
    void DefDown();
}

public interface IDamage
{
    void Damage(float a_Damage);
}

public class InterfaceScript
{
}