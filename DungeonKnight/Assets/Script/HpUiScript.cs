using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpUiScript : MonoBehaviour
{
    public Image hp_Img = null;
    public Text hp_Txt = null;

    public void HpSetting(float hp_Max, float hp_Now)
    {
        if (hp_Img != null)
            hp_Img.fillAmount = hp_Now / hp_Max;        // 체력 이미지를 해당 비율에 맞게 설정

        if (hp_Txt != null)
            hp_Txt.text = (int)hp_Now + " / " + (int)hp_Max;     // 체력 표기 텍스트 수정
    }
}
