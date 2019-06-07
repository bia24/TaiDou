using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    public AudioClip[] audioClips;
    private Dictionary<string, AudioClip> audioDic;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        InitAudioClipDic();
    }


    #region 内部封装逻辑方法
    private void InitAudioClipDic()
    {
        audioDic = new Dictionary<string, AudioClip>();
        foreach(var t in audioClips)
        {
            audioDic.Add(t.name, t);
        }
    }
    #endregion

    #region 外部调用方法
    public void Play(string clipName)
    {
        AudioClip ac = null;
        if(audioDic.TryGetValue(clipName,out ac))
        {
            audioSource.PlayOneShot(ac);
        }
    }
    #endregion
}
