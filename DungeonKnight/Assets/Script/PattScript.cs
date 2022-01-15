using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PattScript : MonoBehaviour
{
    public GameObject[] patt_Obj = null;
    public Text patt_Txt = null;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PattSetting(EnemyPatt a_Patt, string a_Value)
    {
        foreach(GameObject obj in patt_Obj)
        {
            obj.SetActive(false);
        }

        patt_Obj[(int)a_Patt].SetActive(true);
        patt_Txt.gameObject.SetActive(true);
        patt_Txt.text = a_Value;
    }
}
