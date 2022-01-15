using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffAnimScript : MonoBehaviour
{
    EffectScript effectScript;

    void Start()
    {
        effectScript = transform.parent.GetComponent<EffectScript>();
    }

    void Update()
    {
        
    }

    public void EffDestroy()
    {
        effectScript.EffectEnd();
        Destroy(effectScript.gameObject);
    }
}
