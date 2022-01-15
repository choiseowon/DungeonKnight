using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageBtnScript : MonoBehaviour
{
    public StageType stageType = StageType.Monster;
    public Button stage_Btn = null;
    public Sprite[] stage_Sprite;
    public bool stage_Lock = false;
    FadeScript fadeScript = null;

    void Start()
    {
        fadeScript = SelectScript.Inst.fadeScript;

        if(stage_Lock == false)
            stage_Btn.image.color = Color.white;
        else
            stage_Btn.image.color = Color.gray;

        if(stage_Btn != null)
        {
            stage_Btn.image.sprite = stage_Sprite[(int)stageType];
            stage_Btn.onClick.AddListener(() =>
            {
                if (stage_Lock == false)
                {
                    SelectScript.Inst.NextStageCheck(this.gameObject);
                    stage_Btn.image.color = Color.green;
                    GlobalScript.g_StageType = stageType;
                    FadeCall("Stage");
                }
            });
        }
    }

    void Update()
    {

    }

    void FadeCall(string a_SceneName)
    {
        StopScript.Inst.gameObject.SetActive(false);

        fadeScript.m_SceneName = a_SceneName;
        fadeScript.fadeState = FadeState.Out;
        fadeScript.FadeEnabled(true);
    }
}
