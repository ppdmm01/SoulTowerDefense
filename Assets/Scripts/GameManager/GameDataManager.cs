using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ���ݹ�����
/// </summary>
public class GameDataManager : Singleton<GameDataManager>
{
    public MusicData musicData;
    private GameDataManager()
    {
        musicData = JsonMgr.Instance.LoadData<MusicData>("MusicData");
    }

    /// <summary>
    /// ������������
    /// </summary>
    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicData,"MusicData");
    }
}
