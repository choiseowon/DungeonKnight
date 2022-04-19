using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectScript : MonoBehaviour
{
    public static SelectScript Inst = null;
    GameObject[,] stage_Array = new GameObject[10, 5];
    GameObject stage_Boss = null;
    List<GameObject> stage_Old = new List<GameObject>();

    [Header("----- Map -----")]
    public Canvas canvas = null;
    public GameObject stage_Obj = null;
    public GameObject[] scroll_Obj;
    Vector3 stage_Pos = Vector3.zero;

    [Header("----- UI -----")]
    public GameObject stop_obj = null;
    public FadeScript fadeScript = null;

    void Start()
    {
        SoundScript.Inst.BgmSoundPlay("Normal_Bgm");

        if (GlobalScript.g_MapBool == false)
        {
            Inst = this;

            for (int ii = 0; ii < stage_Array.GetLength(0); ii++)
            {
                for (int kk = 0; kk < stage_Array.GetLength(1); kk++)
                {
                    stage_Array[ii, kk] = null;
                }
            }

            for (int ii = 0; ii < stage_Array.GetLength(0); ii++)
            {
                for (int kk = 0; kk < stage_Array.GetLength(1); kk++)
                {
                    GameObject obj = Instantiate(stage_Obj, stage_Pos, Quaternion.identity);
                    obj.transform.SetParent(scroll_Obj[ii].transform);
                    obj.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                    StageBtnScript m_stageBtnScript = obj.GetComponent<StageBtnScript>();

                    if (ii == 0)
                    {
                        m_stageBtnScript.stageType = StageType.Monster;
                    }
                    else if (ii == 4 || ii == 8)
                    {
                        m_stageBtnScript.stageType = StageType.Elite;
                        obj.GetComponent<StageBtnScript>().stage_Lock = true;
                    }
                    else
                    {
                        int stage_Ran = Random.Range(0, 4);
                        m_stageBtnScript.stageType = (StageType)stage_Ran;
                        obj.GetComponent<StageBtnScript>().stage_Lock = true;
                    }

                    stage_Array[ii, kk] = obj;
                }
            }

            stage_Boss = Instantiate(stage_Obj, stage_Pos, Quaternion.identity);
            stage_Boss.transform.SetParent(scroll_Obj[scroll_Obj.Length - 1].transform);
            stage_Boss.transform.localScale = new Vector3(3, 3, 3);
            StageBtnScript m_bossBtnScript = stage_Boss.GetComponent<StageBtnScript>();
            m_bossBtnScript.stageType = StageType.Boss;
            m_bossBtnScript.stage_Lock = true;

            DontDestroyOnLoad(canvas);

            GlobalScript.g_SelecCanvas = canvas;
            GlobalScript.g_MapBool = true;
        }
        else
        {
            canvas.gameObject.SetActive(false);
            canvas = GlobalScript.g_SelecCanvas;
            canvas.gameObject.SetActive(true);
        }
    }

    public void NextStageCheck(GameObject curBtn)
    {
        int array_X = 0;
        int array_Y = 0;

        for (int ii = 0; ii < stage_Array.GetLength(0); ii++) 
            for (int kk = 0; kk < stage_Array.GetLength(1); kk++)
            {
                ColorChange(stage_Array[ii, kk], Color.gray);
                StageLock(stage_Array[ii, kk], true);
                if (stage_Array[ii, kk] == curBtn)
                {
                    array_X = ii;
                    array_Y = kk;
                }
            }

        for (int ii = 0; ii < stage_Old.Count; ii++)
            ColorChange(stage_Old[ii], Color.red);

        stage_Old.Add(curBtn);

        if ((array_X + 1) < stage_Array.GetLength(0))
        {
            GameObject nextObj = stage_Array[array_X + 1, array_Y];
            ColorChange(nextObj, Color.white);
            StageLock(nextObj, false);

            if ((array_Y - 1) >= 0)
            {
                nextObj = stage_Array[array_X + 1, array_Y - 1];
                ColorChange(nextObj, Color.white);
                StageLock(nextObj, false);
            }

            if ((array_Y + 1) < stage_Array.GetLength(1))
            {
                nextObj = stage_Array[array_X + 1, array_Y + 1];
                ColorChange(nextObj, Color.white);
                StageLock(nextObj, false);
            }
        }
        else if((array_X + 1) == stage_Array.GetLength(0))
        {
            GameObject nextObj = stage_Boss;
            ColorChange(nextObj, Color.white);
            StageLock(nextObj, false);
        }
    }

    void ColorChange(GameObject obj, Color color)
    {
        StageBtnScript a_stageBtnScript = obj.GetComponent<StageBtnScript>();

        if(a_stageBtnScript != null)
        {
            a_stageBtnScript.stage_Btn.image.color = color;
        }
    }

    void StageLock(GameObject obj, bool Lock)
    {
        StageBtnScript a_stageBtnScript = obj.GetComponent<StageBtnScript>();

        if (a_stageBtnScript != null)
        {
            a_stageBtnScript.stage_Lock = Lock;
        }
    }
}