using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class ArrowScript : MonoBehaviour
{
    public GameObject arrow_Root = null;
    GameObject[] arrow_Obj = new GameObject[20];
    Vector3[] circle_Pos = new Vector3[20];
    Vector3 mouse_Pos = Vector3.zero;
    Vector3 dis_Pos = Vector3.zero;


    float a = 500;
    float b = 300;
    float h = 0;
    float k = 0;
    //float theta = 0;
    Vector3[] positions = new Vector3[20];

    void Start()
    {
        for(int ii = 0; ii < arrow_Root.transform.childCount; ii++)
        {
            arrow_Obj[ii] = arrow_Root.transform.GetChild(ii).gameObject;
        }
    }

    void Update()
    {
        TargetArrow();
    }

    void TargetArrow()
    {
        mouse_Pos = Input.mousePosition;
        dis_Pos = this.transform.position - mouse_Pos;

        float[] objLerps = new float[arrow_Obj.Length];
        float interval = 1f / (arrow_Obj.Length - 1);
        for (int i = 0; i < arrow_Obj.Length; i++)
            objLerps[i] = interval * i;

        for(int ii = 0; ii < arrow_Obj.Length; ii++)
        {
            Vector3 center_Pos = Vector3.Lerp(mouse_Pos, this.transform.position, 0.5f);
            float theta = Mathf.Atan2(dis_Pos.y, dis_Pos.x) * Mathf.Rad2Deg;
            Debug.Log(theta);
            positions = CreateEllipse(dis_Pos.x, dis_Pos.y / 2, center_Pos.x, center_Pos.y, theta, arrow_Obj.Length);
            arrow_Obj[ii].transform.position = positions[ii];
        }

    }

    Vector3[] CreateEllipse(float a, float b, float h, float k, float theta, int resolution)
    {
        positions = new Vector3[resolution];
        Quaternion q = Quaternion.AngleAxis(theta, Vector3.forward);
        Vector3 center = new Vector3(h, k, 0.0f);
        for (int i = 0; i < resolution; i++)
        {
            float angle = (float)i / (float)resolution * 2.0f * (Mathf.PI);
            positions[i] = new Vector3(a * Mathf.Cos(angle), b * Mathf.Sin(angle), 0.0f);
            positions[i] = q * positions[i] + center;
        }
        return positions;
    }
}
