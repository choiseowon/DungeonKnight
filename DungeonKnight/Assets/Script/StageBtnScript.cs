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

    public bool on_Check = false;

    public List<StageBtnScript> next_BtnList = new List<StageBtnScript>();

    public GameObject[] line_Root = null;
    GameObject[] line_Obj = new GameObject[5];

    Vector3 line_Pos1 = Vector3.zero;
    Vector3 line_Pos2 = Vector3.zero;

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
                    SelectScript.Inst.NextStageCheck(this);
                    GlobalScript.g_StageType = stageType;
                    FadeCall("Stage");
                }
            });
        }
    }

    public void BtnOnOff(bool onoff)
    {
        stage_Btn.gameObject.SetActive(onoff);
    }

    public void NextLine()
    {
        for(int ii = 0; ii < next_BtnList.Count; ii++)
        {
            line_Pos1 = this.transform.position;
            line_Pos2 = next_BtnList[ii].transform.position;
            line_Root[ii].SetActive(true);

            for (int kk = 0; kk < line_Obj.Length; kk++)
            {
                float size = 1.0f / line_Obj.Length;
                line_Obj[kk] = line_Root[ii].transform.GetChild(kk).gameObject;
                line_Obj[kk].transform.position = Vector3.Lerp(line_Pos1, line_Pos2, size * kk);
                float angle = GetAngle(line_Pos1, line_Pos2);
                line_Obj[kk].transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    public void LineColor(Color a_color)
    {
        for (int ii = 0; ii < next_BtnList.Count; ii++)
        {
            line_Root[ii].SetActive(true);

            for (int kk = 0; kk < line_Obj.Length; kk++)
            {
                line_Obj[kk] = line_Root[ii].transform.GetChild(kk).gameObject;
                line_Obj[kk].GetComponent<Image>().color = a_color;
            }
        }
    }

    float GetAngle(Vector2 start, Vector2 end)
    {
        Vector2 v2 = end - start;
        return Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
    }

    void FadeCall(string a_SceneName)
    {
        StopScript.Inst.gameObject.SetActive(false);

        fadeScript.m_SceneName = a_SceneName;
        fadeScript.fadeState = FadeState.Out;
        fadeScript.FadeEnabled(true);
    }
}
