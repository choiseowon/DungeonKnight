using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundClass     // 사운드 관련 변수와 변수 값 설정 함수 클래스
{
    public AudioSource audio;      // 오디오를 저장할 변수
    string audio_Str;       // 사운드 검색 시 체크하기 위한 변수
    float origin_Volume = 0.0f;     // 각각의 사운드 마다 설정 해둔 사운드 크기 저장

    public void SoundSetting(AudioSource a_Audio, string a_Str)     // 사운드 저장 함수
    {
        audio = a_Audio;    // 매개변수로 받아온 오디오 저장
        audio_Str = a_Str;  // 매개변수로 받아온 이름 저장
        origin_Volume = a_Audio.volume;     // 오디오 고유의 소리 크기 저장
    }

    public void SoundVolume(float bg_V, float sf_V)     // 클래스에 저장된 오디오의 볼륨을 조절하는 함수
    {
        if(audio.gameObject.tag.Contains("Bgm") == true)        // 배경음악인지 효과음인지 구분하는 조건
        {
            audio.volume = origin_Volume * bg_V;        // 원본 볼륨에 조절한 볼륨 값을 곱하는 계산
        }
        else
        {
            audio.volume = origin_Volume * sf_V;        // 원본 볼륨에 조절한 볼륨 값을 곱하는 계산
        }
    }
    
    public void SoundStop()     // 오디오를 중지 시키는 함수
    {
        if(audio != null)       // 오디오가 비어있지 않다는 조건
        {
            audio.gameObject.SetActive(false);      // 오디오가 붙어 있는 오브젝트를 끔
            audio.gameObject.SetActive(true);       // 오디오가 붙어 있는 오브젝트를 킴, 끄고 키는 것으로 오디오 초기화
        }
    }

    public bool SoundCheck(string str)      // 매개변수로 넘어온 이름이 해당 오디오의 이름과 같은지 비교
    {
        if (str == audio_Str)   // 이름이 같다면 참, 아니면 거짓 반환
            return true;
        else
            return false;
    }
}

public class SoundScript : MonoBehaviour
{
    public static SoundScript Inst;             // 해당 스크립트 싱글톤 선언   
    public GameObject sound_Root = null;        // 오디오 오브젝트들의 부모 오브젝트
    SoundClass[] sounds = new SoundClass[20];   // 존재하는 사운드의 최대 크기 만큼 배열 선언
    SoundClass now_Sound = new SoundClass();    // 현재 재생되고 있는 클래스를 저장하기 위한 선언

    void Start()
    {
        Inst = this;
        AudioSource[] m_Audio;      // 오디오 배열 선언
        m_Audio = sound_Root.GetComponentsInChildren<AudioSource>();    // 선언한 오디오 배열에 모든 오디오를 대입

        string[] m_Str = new string[m_Audio.Length];    // 대입한 오디오 오브젝트 만큼 배열 크기 선언

        for (int ii = 0; ii < m_Audio.Length; ii++)     // 오디오 개수 만큼 반복
        {
            m_Str[ii] = m_Audio[ii].gameObject.name;    // 오디오의 이름을 변수로 저장
            sounds[ii] = new SoundClass();      // 클래스 배열에 새로운 클래스 생성
            sounds[ii].SoundSetting(m_Audio[ii], m_Str[ii]);    // 생성한 클래스에 해당 변수의 값을 넘겨줌
        }
    }

    void Update()
    {
        for(int ii = 0; ii < sounds.Length; ii++)       // 모든 오디오의 볼륨을 실시간으로 조정
        {
            if (sounds[ii] != null)     // 오디오가 있다면
                sounds[ii].SoundVolume(GlobalScript.bg_Volume, GlobalScript.sf_Volume);     // 오디오 볼륨을 조정
        }

    }

    public void SoundControl(string str)        // 사운드 재생이 필요할 경우 호출되는 함수
    {
        SoundClass play_Sound = new SoundClass();       // 클래스 생성

        for(int ii = 0; ii < sounds.Length; ii++)       // 존재하는 오디오 만큼 반복
        {
            if (sounds[ii].SoundCheck(str))     // 매개변수로 입력된 이름과 같은 사운드를 검색
            {
                play_Sound = sounds[ii];        // 검색된 오디오의 클래스를 대입
                break;
            }
        }

        if (play_Sound.audio.tag.Contains("Bgm") == true)      // 해당 오디오가 배경음악일 경우
        {
            if (now_Sound == play_Sound)        // 해당 오디오가 재생되고 있는 오디오와 같을 경우 반환
                return;

            play_Sound.audio.Play();       // 오디오 재생
            now_Sound.SoundStop();      // 이전에 재생되고 있던 배경음악 정지
            now_Sound = play_Sound;     // 새로 재생되고 있는 오디오의 클래스를 비교용 클래스에 저장
        }
        else
        {
            play_Sound.audio.PlayOneShot(play_Sound.audio.clip);      // 해당 오디오 한 번 재생
        }

    }
}
