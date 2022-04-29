using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectScript : MonoBehaviour
{
    public static SelectScript Inst = null;
    public GameObject stage_Btn = null;
    StageBtnScript[,] stage_Array = new StageBtnScript[12, 5];
    StageBtnScript stage_Boss;
    List<StageBtnScript> stage_Old = new List<StageBtnScript>();

    [Header("----- Map -----")]
    public Canvas canvas = null;
    public GameObject content_Root = null;
    public Transform left_Tr = null;
    public Transform right_Tr = null;
    public Transform top_Tr = null;
    public Transform boss_Tr = null;

    [Header("----- UI -----")]
    public GameObject stop_obj = null;
    public FadeScript fadeScript = null;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        SoundScript.Inst.BgmSoundPlay("Normal_Bgm");

        if (GlobalScript.g_MapBool == false)
        {
            Inst = this;

            StageBtnCreate();

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

    void StageBtnCreate()
    {
        for (int ii = 0; ii < stage_Array.GetLength(0); ii++)
        {
            Vector3 top_Pos = Vector3.Lerp(left_Tr.position, top_Tr.position, (1.0f / (stage_Array.GetLength(0) - 1)) * ii);

            for (int kk = 0; kk < stage_Array.GetLength(1); kk++)
            {
                int randX = Random.Range(-25, 25);
                int randY = Random.Range(-25, 25);
                GameObject obj = Instantiate(stage_Btn);
                obj.transform.SetParent(content_Root.transform);
                obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                Vector3 lr_pos = Vector3.Lerp(left_Tr.position, right_Tr.position, (1.0f / (stage_Array.GetLength(1) - 1)) * kk);
                lr_pos.x = lr_pos.x + randX;
                top_Pos.y = top_Pos.y + randY;
                obj.transform.position = new Vector3(lr_pos.x, top_Pos.y, 0);
                stage_Array[ii, kk] = obj.GetComponent<StageBtnScript>();

                if (ii == 0)
                {
                    stage_Array[ii, kk].stageType = StageType.Monster;
                }
                else if (ii == 4 || ii == 8)
                {
                    stage_Array[ii, kk].stageType = StageType.Elite;
                    stage_Array[ii, kk].stage_Lock = true;
                }
                else if(ii == 5 || ii == 11)
                {
                    stage_Array[ii, kk].stageType = StageType.Camp;
                    stage_Array[ii, kk].stage_Lock = true;
                }
                else
                {
                    int stage_Ran = Random.Range(0, 3);
                    stage_Array[ii, kk].stageType = (StageType)stage_Ran;
                    stage_Array[ii, kk].stage_Lock = true;
                }
            }

            if (ii == stage_Array.GetLength(0) - 1)
            {
                GameObject obj = Instantiate(stage_Btn);
                obj.transform.SetParent(content_Root.transform);
                obj.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                obj.transform.position = boss_Tr.position;
                stage_Boss = obj.GetComponent<StageBtnScript>();
                stage_Boss.on_Check = true;
                stage_Boss.BtnOnOff(true);
                stage_Boss.stageType = StageType.Boss;
                stage_Boss.stage_Lock = true;
            }
        }

        List<StageBtnScript> start_List = new List<StageBtnScript>();

        for (int ii = 0; ii < stage_Array.GetLength(1); ii++)
            start_List.Add(stage_Array[0, ii].GetComponent<StageBtnScript>());

        int start_Rand = Random.Range(3, 5);
        for (int ii = 0; ii < start_Rand; ii++)
        {
            int rand = Random.Range(0, start_List.Count);
            start_List[rand].on_Check = true;
            start_List[rand].BtnOnOff(true);
            start_List.Remove(start_List[rand]);
        }

        for (int ii = 0; ii < stage_Array.GetLength(0); ii++)
        {
            bool cross_Check = false;
            int on_Count = 0;

            for (int kk = 0; kk < stage_Array.GetLength(1); kk++)
            {
                StageBtnScript nowBtn = stage_Array[ii, kk];

                if (nowBtn.on_Check == false)
                    continue;

                if (ii == stage_Array.GetLength(0) - 1)
                {
                    nowBtn.next_BtnList.Add(stage_Boss);
                    nowBtn.NextLine();
                    continue;
                }

                List<StageBtnScript> next_List = new List<StageBtnScript>();
                if (kk > 0 && kk < stage_Array.GetLength(1) - 1)
                {
                    StageBtnScript stageBtn;

                    stageBtn = stage_Array[ii + 1, kk - 1];

                    if (cross_Check == false)
                        next_List.Add(stageBtn);

                    stageBtn = stage_Array[ii + 1, kk];
                    next_List.Add(stageBtn);

                    stageBtn = stage_Array[ii + 1, kk + 1];
                    next_List.Add(stageBtn);

                }
                else if (kk <= 0)
                {
                    StageBtnScript stageBtn;

                    stageBtn = stage_Array[ii + 1, kk];
                    next_List.Add(stageBtn);

                    stageBtn = stage_Array[ii + 1, kk + 1];
                    next_List.Add(stageBtn);
                }
                else if (kk >= stage_Array.GetLength(1) - 1)
                {
                    StageBtnScript stageBtn;

                    stageBtn = stage_Array[ii + 1, kk - 1];

                    if (cross_Check == false)
                        next_List.Add(stageBtn);

                    stageBtn = stage_Array[ii + 1, kk];

                    next_List.Add(stageBtn);

                }

                int min_Rand = 1;
                int max_Rand = next_List.Count + 1;
                int rand = 0;

                if (on_Count < kk)
                    min_Rand += 1;

                if (max_Rand > 3)
                    max_Rand = 3;

                if (min_Rand > 2)
                    rand = 2;
                else
                    rand = Random.Range(min_Rand, max_Rand);

                for (int nn = 0; nn < rand; nn++)
                {
                    if (next_List.Count <= 0)
                        break;

                    int rand2 = Random.Range(0, next_List.Count);
                    nowBtn.next_BtnList.Add(next_List[rand2]);
                    next_List[rand2].on_Check = true;
                    next_List[rand2].BtnOnOff(true);
                    next_List.Remove(next_List[rand2]);
                    on_Count += 1;
                    if ((ii + 1 < stage_Array.GetLength(0)) && kk + 1 < stage_Array.GetLength(1))
                    {
                        if (stage_Array[ii + 1, kk + 1].on_Check == true)
                            cross_Check = true;
                        else
                            cross_Check = false;
                    }
                }

                nowBtn.NextLine();
            }
        }
    }

    public void NextStageCheck(StageBtnScript selecBtn)
    {

        for (int ii = 0; ii < stage_Array.GetLength(0); ii++)
            for (int kk = 0; kk < stage_Array.GetLength(1); kk++)
            {
                stage_Array[ii, kk].stage_Btn.image.color = Color.gray;
                stage_Array[ii, kk].stage_Lock = true;
                stage_Array[ii, kk].LineColor(Color.black);
            }

        selecBtn.LineColor(Color.white);

        foreach (StageBtnScript stageBtn in selecBtn.next_BtnList)
        {
            stageBtn.stage_Btn.image.color = Color.white;
            stageBtn.stage_Lock = false;
            //stageBtn.LineColor(Color.white);
        }

        stage_Old.Add(selecBtn);

        for (int ii = 0; ii < stage_Old.Count; ii++)
        {
            stage_Old[ii].stage_Btn.image.color = Color.green;
        }
    }
}