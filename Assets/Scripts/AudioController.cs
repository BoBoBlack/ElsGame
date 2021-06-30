using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    None,
    PointDown,
    SetItem,
    DeletLine,
    Nice
}
public class AudioController : MonoBehaviour
{
    public static AudioController Instance;
    private void Awake()
    {
        Instance = this;
    }
    AudioSource mAudio;
    public bool IsPlayGameAudio = true;
    public AudioClip audio_PointDown;
    public AudioClip audio_SetItem;
    public AudioClip audio_DeletLine;
    public AudioClip audio_Nice;
    AudioType curPlayType;
    // Start is called before the first frame update
    void Start()
    {
        mAudio = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioType audioType)
    {
        if (!IsPlayGameAudio) return;
        mAudio.Stop();
        switch (audioType)
        {
            case AudioType.PointDown:
                mAudio.clip = audio_PointDown;
                break;
            case AudioType.SetItem:
                mAudio.clip = audio_SetItem;
                break;
            case AudioType.DeletLine:
                mAudio.clip = audio_DeletLine;
                break;
            case AudioType.Nice:
                mAudio.clip = audio_Nice;
                break;
        }
        if (mAudio.clip != null)
            mAudio.Play();
    }
    public void ChangeAudioState()
    {
        if(IsPlayGameAudio)
        {
            IsPlayGameAudio = false;
            mAudio.Stop();
        }
        else
        {
            IsPlayGameAudio = true;
        }
    }
}
