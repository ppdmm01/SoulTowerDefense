using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMono<AudioManager>
{
    private MusicData data;
    //BGM
    [SerializeField]
    private AudioSource BGMAudioSource;
    //��Ч
    [SerializeField]
    private AudioSource soundAudioSource;

    //BGM��Ƭ�ֵ�
    private Dictionary<string, AudioClip> audioDic;

    protected override void Awake()
    {
        base.Awake();
        //BGMѭ������
        BGMAudioSource.loop = true;
        //��ʼ���ֵ�
        audioDic = new Dictionary<string, AudioClip>();
        data = GameDataManager.Instance.musicData;
    }

    private void Start()
    {
        //����������������
        BGMAudioSource.mute = !data.isMusicOpen;
        BGMAudioSource.volume = data.musicValue;
        soundAudioSource.mute = !data.isSoundOpen;
        soundAudioSource.volume = data.soundValue;
    }

    /// <summary>
    /// �������ֿ���
    /// </summary>
    /// <param name="isOn">�Ƿ񿪹�����</param>
    public void SetIsMusicOpen(bool isOn)
    {
        BGMAudioSource.mute = !isOn;
    }

    /// <summary>
    /// ��������������С 
    /// </summary>
    /// <param name="value">����ֵ</param>
    public void SetMusicVolume(float value)
    {
        BGMAudioSource.volume = value;
    }

    /// <summary>
    /// ������Ч����
    /// </summary>
    /// <param name="isOn">�Ƿ񿪹�����</param>
    public void SetIsSoundOpen(bool isOn)
    {
        soundAudioSource.mute = !isOn;
    }

    /// <summary>
    /// ������Ч������С 
    /// </summary>
    /// <param name="value">����ֵ</param>
    public void SetSoundVolume(float value)
    {
        soundAudioSource.volume = value;
    }

    /// <summary>
    /// �õ�������Ƭ
    /// </summary>
    /// <param name="path">��Դ·��</param>
    /// <returns>��Ƭ</returns>
    private AudioClip GetAudio(string path)
    {
        if (!audioDic.ContainsKey(path))
        {
            audioDic.Add(path, Resources.Load<AudioClip>(path));
        }
        return audioDic[path];
    }

    /// <summary>
    /// ����BGM
    /// </summary>
    /// <param name="path">BGM��Դ·��</param>
    public void PlayBGM(string path)
    {
        //��ͬ��BGM����Ҫ�л�
        if (BGMAudioSource.clip == null || BGMAudioSource.clip != GetAudio(path))
        {
            StopAllCoroutines();
            StartCoroutine(ChangeBGM(GetAudio(path)));
        }
    }

    /// <summary>
    /// �ı�BGM
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public IEnumerator ChangeBGM(AudioClip clip)
    {
        float initVolume = data.musicValue;
        //����
        while (BGMAudioSource.volume > 0)
        {
            BGMAudioSource.volume -= 0.02f;
            yield return null;
        }
        //�л�����
        BGMAudioSource.Stop();
        BGMAudioSource.clip = clip;
        BGMAudioSource.Play();
        //����
        while (BGMAudioSource.volume < initVolume)
        {
            BGMAudioSource.volume += 0.02f;
            yield return null;
        }
    }

    /// <summary>
    /// ֹͣ����BGM
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
