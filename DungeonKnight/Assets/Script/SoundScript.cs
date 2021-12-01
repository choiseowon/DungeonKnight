using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    public static SoundScript Inst;
    GameObject soundObj;
    // Start is called before the first frame update

    void Start()
    {
        Inst = this;
    }

    public void SoundControl(string str)
    {
        AudioSource audio;

        soundObj = this.transform.Find("Sound").gameObject;

        audio = soundObj.transform.Find(str).GetComponent<AudioSource>();


        if (str == "Camping" || str == "Boss" || str == "Elite" || str == "Normal")
        {
            audio.Play();
        }
        else if(str == "Clear" || str == "GameOver")
        {
            audio.Play();
            soundObj.transform.Find("Boss").gameObject.SetActive(false);
            soundObj.transform.Find("Elite").gameObject.SetActive(false);
            soundObj.transform.Find("Normal").gameObject.SetActive(false);
        }
        else if(str == "Victory")
        {
            audio.Play();
            soundObj.transform.Find("Boss").gameObject.SetActive(false);
        }
        else
        {
            audio.PlayOneShot(audio.clip);
        }

    }
}
