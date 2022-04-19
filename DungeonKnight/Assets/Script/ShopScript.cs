using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public static ShopScript Inst;

    public Button shop_OBtn = null;
    public Button shop_CBtn = null;
    public GameObject shop_Root = null;
    public Text user_GTxt = null;
    public Text error_Txt = null;

    [Header("----- Item -----")]
    public Button weapon_Btn = null;
    public Button shield_Btn = null;
    public Button armor_Btn = null;
    public Text weapon_Txt = null;
    public Text shield_Txt = null;
    public Text armor_Txt = null;

    [Header("----- Card -----")]
    public GameObject card_Root = null;
    public Transform[] card_Tr = new Transform[5];
    List<CardClass> card_List = new List<CardClass>();
    public Text[] card_GTxt = null;

    void Start()
    {
        Inst = this;
        shop_Root.transform.position = new Vector3(GlobalScript.g_Width / 2, GlobalScript.g_Height / 2, 0);

        for(int ii = 0; ii < 5; ii++)
        {
            int rand = Random.Range(0, (int)CardType.Count);
            GameObject obj = Instantiate(CardCtrlScript.Inst.card_Obj[rand]);
            obj.transform.SetParent(card_Root.transform);
            obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            obj.transform.position = card_Tr[ii].position;
            CardClass cardClass = obj.GetComponent<CardClass>();
            card_List.Add(cardClass);
            cardClass.cardState = CardState.Shop;
            cardClass.CardSetting();
            card_GTxt[ii].text = cardClass.card_Gold + " 골드";
            card_GTxt[ii].transform.SetParent(obj.transform);
        }

        if (shop_OBtn != null)
            shop_OBtn.onClick.AddListener(() =>
            {
                shop_Root.SetActive(true);
            });

        if (shop_CBtn != null)
            shop_CBtn.onClick.AddListener(() =>
            {
                shop_Root.SetActive(false);
            });

        if(weapon_Btn != null)
            weapon_Btn.onClick.AddListener(() =>
            {
                ShopWeapon();
            });

        if (shield_Btn != null)
            shield_Btn.onClick.AddListener(() =>
            {
                ShopShield();
            });

        if (armor_Btn != null)
            armor_Btn.onClick.AddListener(() =>
            {
                ShopAmror();
            });


        weapon_Txt.text = "추가 공격력 + 4" + "\n" + GlobalScript.g_ShopWeapon.ToString() + " 골드";
        shield_Txt.text = "추가 방어력 + 3" + "\n" + GlobalScript.g_ShopShield.ToString() + " 골드";
        armor_Txt.text = "추가 체력 + 15" + "\n" + GlobalScript.g_ShopArmor.ToString() + " 골드";
        user_GTxt.text = "소지금" + "\n\n" + GlobalScript.g_Gold.ToString() + " 골드";

    }

    void ShopWeapon()
    {
        if (GlobalScript.g_Gold >= GlobalScript.g_ShopWeapon)
        {
            GlobalScript.g_AttPuls += 4;

            GlobalScript.g_Gold -= GlobalScript.g_ShopWeapon;
            GlobalScript.g_ShopWeapon += 50;
            weapon_Txt.text = "추가 공격력 + 4" + "\n" + GlobalScript.g_ShopWeapon.ToString() + " 골드";
            user_GTxt.text = "소지금" + "\n\n" + GlobalScript.g_Gold.ToString() + " 골드";

            SoundScript.Inst.SfSoundPlay("ItemBuy");
            StopScript.Inst.StateUpdate();

            for(int ii = 0; ii < card_List.Count; ii++)
                card_List[ii].CardSetting();
        }
        else
        {
            GoldError();
        }
    }

    void ShopShield()
    {
        if (GlobalScript.g_Gold >= GlobalScript.g_ShopShield)
        {
            GlobalScript.g_DefPuls += 3;

            GlobalScript.g_Gold -= GlobalScript.g_ShopShield;
            GlobalScript.g_ShopShield += 50;
            shield_Txt.text = "추가 방어력 + 3" + "\n" + GlobalScript.g_ShopShield.ToString() + " 골드";
            user_GTxt.text = "소지금" + "\n\n" + GlobalScript.g_Gold.ToString() + " 골드";

            SoundScript.Inst.SfSoundPlay("ItemBuy");
            StopScript.Inst.StateUpdate();

            for (int ii = 0; ii < card_List.Count; ii++)
                card_List[ii].CardSetting();
        }
        else
        {
            GoldError();
        }

    }

    void ShopAmror()
    {
        if (GlobalScript.g_Gold >= GlobalScript.g_ShopArmor)
        {
            GlobalScript.g_HealthMax += 15;
            GlobalScript.g_HealthNow += 15;

            GlobalScript.g_Gold -= GlobalScript.g_ShopArmor;
            GlobalScript.g_ShopArmor += 50;
            armor_Txt.text = "추가 체력 + 15" + "\n" + GlobalScript.g_ShopArmor.ToString() + " 골드";
            user_GTxt.text = "소지금" + "\n\n" + GlobalScript.g_Gold.ToString() + " 골드";

            SoundScript.Inst.SfSoundPlay("ItemBuy");
            StopScript.Inst.StateUpdate();
            HeroScript.Inst.hp_Ui.HpSetting(GlobalScript.g_HealthMax, GlobalScript.g_HealthNow);

            for (int ii = 0; ii < card_List.Count; ii++)
                card_List[ii].CardSetting();
        }
        else
        {
            GoldError();
        }

    }

    public bool ShopCard(int a_Gold, GameObject card_Obj)
    {
        if (GlobalScript.g_Gold >= a_Gold)
        {
            GlobalScript.g_Gold -= a_Gold;
            SoundScript.Inst.SfSoundPlay("ItemBuy");
            user_GTxt.text = "소지금" + "\n\n" + GlobalScript.g_Gold.ToString() + " 골드";
            return true;
        }
        else
        {
            GoldError();
            return false;
        }
    }

    public void GoldError()
    {
        Animator ani = error_Txt.transform.GetComponent<Animator>();
        ani.SetTrigger("End");
    }
}
