using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMainScript : MonoBehaviour
{
    public Button start_Btn = null;
    public Button op_Btn = null;
    public Button end_Btn = null;
    public FadeScript fadeScript = null;

    [Header("----- Option -----")]
    public GameObject op_Obj = null;
    public Slider bg_Slider = null;
    public Slider sf_Slider = null;
    public Button op_CBtn = null;

    [Header("Pause")]
    public GameObject pause_Obj = null;
    public Button yes_Btn = null;
    public Button no_Btn = null;

    void Start()
    {
        if(GlobalScript.g_SelecCanvas != null)
            Destroy(GlobalScript.g_SelecCanvas.gameObject);

        if (start_Btn != null)
            start_Btn.onClick.AddListener(() =>
            {
                if(fadeScript != null)
                    FadeCall("Select");
            });

        if (op_Btn != null)
            op_Btn.onClick.AddListener(() =>
            {
                if (op_Obj != null)
                    op_Obj.SetActive(true);
            });

        if (end_Btn != null)
            end_Btn.onClick.AddListener(() =>
            {
                if (pause_Obj != null)
                    pause_Obj.SetActive(true);
            });

        if (op_CBtn != null)
            op_CBtn.onClick.AddListener(() =>
            {
                if (op_Obj != null)
                    op_Obj.SetActive(false);
            });

        if (yes_Btn != null)
            yes_Btn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        if (no_Btn != null)
            no_Btn.onClick.AddListener(() =>
            {
                if (pause_Obj != null)
                    pause_Obj.SetActive(false);
            });

        GlobalScript.ValueReset();
        bg_Slider.value = GlobalScript.bg_Volume;
        sf_Slider.value = GlobalScript.sf_Volume;
    }

    void Update()
    {
        GlobalScript.bg_Volume = bg_Slider.value;
        GlobalScript.sf_Volume = sf_Slider.value;
    }

    void FadeCall(string a_SceneName)
    {
        fadeScript.m_SceneName = a_SceneName;
        fadeScript.fadeState = FadeState.Out;
        fadeScript.FadeEnabled(true);
    }
}
