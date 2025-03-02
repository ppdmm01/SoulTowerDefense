using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMono<AudioManager>
{
    private MusicData data;
    //BGM
    [SerializeField]
    private AudioSource BGMAudioSource;
    //音效
    [SerializeField]
    private AudioSource soundAudioSource;

    //BGM切片字典
    private Dictionary<string, AudioClip> audioDic;

    protected override void Awake()
    {
        base.Awake();
        //BGM循环播放
        BGMAudioSource.loop = true;
        //初始化字典
        audioDic = new Dictionary<string, AudioClip>();
        data = GameDataManager.Instance.musicData;
    }

    private void Start()
    {
        //根据数据设置音乐
        BGMAudioSource.mute = !data.isMusicOpen;
        BGMAudioSource.volume = data.musicValue;
        soundAudioSource.mute = !data.isSoundOpen;
        soundAudioSource.volume = data.soundValue;
    }

    /// <summary>
    /// 设置音乐开关
    /// </summary>
    /// <param name="isOn">是否开关音乐</param>
    public void SetIsMusicOpen(bool isOn)
    {
        BGMAudioSource.mute = !isOn;
    }

    /// <summary>
    /// 设置音乐音量大小 
    /// </summary>
    /// <param name="value">音量值</param>
    public void SetMusicVolume(float value)
    {
        BGMAudioSource.volume = value;
    }

    /// <summary>
    /// 设置音效开关
    /// </summary>
    /// <param name="isOn">是否开关音乐</param>
    public void SetIsSoundOpen(bool isOn)
    {
        soundAudioSource.mute = !isOn;
    }

    /// <summary>
    /// 设置音效音量大小 
    /// </summary>
    /// <param name="value">音量值</param>
    public void SetSoundVolume(float value)
    {
        soundAudioSource.volume = value;
    }

    /// <summary>
    /// 得到音乐切片
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns>切片</returns>
    private AudioClip GetAudio(string path)
    {
        if (!audioDic.ContainsKey(path))
        {
            audioDic.Add(path, Resources.Load<AudioClip>(path));
        }
        return audioDic[path];
    }

    /// <summary>
    /// 播放BGM
    /// </summary>
    /// <param name="path">BGM资源路径</param>
    public void PlayBGM(string path)
    {
        //不同的BGM才需要切换
        if (BGMAudioSource.clip == null || BGMAudioSource.clip != GetAudio(path))
        {
            StopAllCoroutines();
            StartCoroutine(ChangeBGM(GetAudio(path)));
        }
    }

    /// <summary>
    /// 改变BGM
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public IEnumerator ChangeBGM(AudioClip clip)
    {
        float initVolume = data.musicValue;
        //淡出
        while (BGMAudioSource.volume > 0)
        {
            BGMAudioSource.volume -= 0.02f;
            yield return null;
        }
        //切换音乐
        BGMAudioSource.Stop();
        BGMAudioSource.clip = clip;
        BGMAudioSource.Play();
        //淡入
        while (BGMAudioSource.volume < initVolume)
        {
            BGMAudioSource.volume += 0.02f;
            yield return null;
        }
    }

    /// <summary>
    /// 停止播放BGM
    /// </summary>
    public void StopBGM()
    {
        BGMAudioSource.Stop();
    }

    public void PlaySound(string path)
    {
        soundAudioSource.PlayOneShot(Resources.Load<AudioClip>(path));
    }
}
