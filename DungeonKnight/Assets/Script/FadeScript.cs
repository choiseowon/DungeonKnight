using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum FadeState
{
    In,
    Out,
    Ready
}

public class FadeScript : MonoBehaviour
{
    float fade_Time = 0.0f;
    public Image fade_Image = null;
    [HideInInspector] public FadeState fadeState = FadeState.Ready;
    [HideInInspector] public string m_SceneName = "";

    void Start()
    {
        fadeState = FadeState.In;
        fade_Image = this.GetComponent<Image>();

        FadeEnabled(true);
    }

    void Update()
    {
        if (fadeState != FadeState.Ready)
            FadeOnOff();

    }

    void FadeOnOff()
    {
        fade_Time += Time.deltaTime;

        if (fade_Time >= 1.0f)
        {
            if(fadeState == FadeState.In)
            {
                fadeState = FadeState.Ready;
                FadeEnabled(false);
            }
            else if(fadeState == FadeState.Out)
            {
                if (GlobalScript.g_SelecCanvas != null)
                {
                    if (m_SceneName == "Stage")
                    {
                        fadeState = FadeState.In;
                        FadeEnabled(true);
                        GlobalScript.g_SelecCanvas.gameObject.SetActive(false);
                    }

                }

                PlayerPrefs.SetFloat("BgmVolume", GlobalScript.bgm_Volume);   // 게임 종료 시 볼륨값 로컬로 저장
                PlayerPrefs.SetFloat("SfVolume", GlobalScript.sf_Volume);   // 게임 종료 시 볼륨값 로컬로 저장

                SceneManager.LoadScene(m_SceneName);

            }
                

        }

        if (fadeState == FadeState.In)
            fade_Image.color = new Color(0, 0, 0, 1 - fade_Time);
        else if(fadeState == FadeState.Out)
            fade_Image.color = new Color(0, 0, 0, 0 + fade_Time);
    }

    public void FadeEnabled(bool a_FadeBool)
    {
        fade_Time = 0.0f;
        fade_Image.enabled = a_FadeBool;
    }
}
