using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundClass
{
    AudioSource audio;
    string audio_Str;
    float origin_Volume = 0.0f;

    public void SoundSetting(AudioSource a_Audio, string a_Str)
    {
        audio = a_Audio;
        audio_Str = a_Str;
        origin_Volume = a_Audio.volume;
    }

    public void SoundReturn(out AudioSource a_Audio, out string a_Str)
    {
        a_Audio = audio;
        a_Str = audio_Str;

    }

    public void SoundVolume(float bg_V, float sf_V)
    {
        if(audio.gameObject.tag.Contains("Bgm") == true)
        {
            audio.volume = origin_Volume * bg_V;
        }
        else
        {
            audio.volume = origin_Volume * sf_V;
        }
    }
    
    public void SoundStop()
    {
        if(audio != null)
        {
            audio.gameObject.SetActive(false);
            audio.gameObject.SetActive(true);
        }
    }

    public bool SoundCheck(string str)
    {
        if (str == audio_Str)
            return true;
        else
            return false;
    }
}

public class SoundScript : MonoBehaviour
{
    public static SoundScript Inst;
    public GameObject sound_Root = null;
    SoundClass[] sounds = new SoundClass[20];
    SoundClass now_Sound = new SoundClass();

    void Start()
    {
        Inst = this;
        AudioSource[] m_Audio;
        m_Audio = sound_Root.GetComponentsInChildren<AudioSource>();

        string[] m_Str = new string[m_Audio.Length];

        for (int ii = 0; ii < m_Audio.Length; ii++)
        {
            m_Str[ii] = m_Audio[ii].gameObject.name;
            sounds[ii] = new SoundClass();
            sounds[ii].SoundSetting(m_Audio[ii], m_Str[ii]);
        }
    }

    void Update()
    {
        for(int ii = 0; ii < sounds.Length; ii++)
        {
            if (sounds[ii] != null)
                sounds[ii].SoundVolume(GlobalScript.bg_Volume, GlobalScript.sf_Volume);
        }

    }

    public void SoundControl(string str)
    {
        SoundClass play_Sound = new SoundClass();

        for(int ii = 0; ii < sounds.Length; ii++)
        {
            if (sounds[ii].SoundCheck(str))
            {
                play_Sound = sounds[ii];
                break;
            }
        }

        AudioSource audio;
        string m_Str;
        play_Sound.SoundReturn(out audio, out m_Str);

        if (audio.tag.Contains("Bgm") == true)
        {
            if (now_Sound == play_Sound)
                return;

            audio.Play();
            now_Sound.SoundStop();
            now_Sound = play_Sound;
        }
        else
        {
            audio.PlayOneShot(audio.clip);
        }

    }
}
