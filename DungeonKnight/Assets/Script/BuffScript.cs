using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuffType
{
    Defence,
    AttUp,
    DefDown,
    AttDown,
    ADUp,
    Perfect,
    Count
}

public class BuffScript : MonoBehaviour
{
    public BuffType buffType = BuffType.Count;
    public GameObject[] buff_Img = null;
    public Text buff_Txt = null;
    public int buff_Count = 0;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void BuffSetting(BuffType a_Type, int a_Count)
    {
        buffType = a_Type;
        buff_Img[(int)buffType].SetActive(true);
        buff_Count = a_Count;
        buff_Txt.text = buff_Count.ToString();
    }
}
