using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ��Ϸ���ݹ�����
/// </summary>
public class GameDataManager : Singleton<GameDataManager>
{
    public MusicData musicData; //��������
    public List<GridData> gridDatas; //������Ʒ����
    public Map mapData; //��ͼ����
    public GameRes gameResData; //��Ϸ��Դ����

    private GameDataManager()
    {
        Debug.Log(Application.persistentDataPath);
        musicData = JsonMgr.Instance.LoadData<MusicData>("MusicData") ?? new MusicData();
        gridDatas = JsonMgr.Instance.LoadData<List<GridData>>("GridDatas") ?? new List<GridData>();
        mapData = JsonMgr.Instance.LoadData<Map>("MapData") ?? new Map();
        gameResData = JsonMgr.Instance.LoadData<GameRes>("GameRes");
    }

    /// <summary>
    /// ������������
    /// </summary>
    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicData,"MusicData");
    }

    /// <summary>
    /// ��ȡ��������
    /// </summary>
    /// <param name="gridName"></param>
    /// <returns></returns>
    public GridData GetGridData(string gridName)
    {
        if (gridDatas == null) return null;

        if (gridDatas.Any(data => data != null && data.gridName == gridName))
        {
            return gridDatas.FirstOrDefault(data => data != null && data.gridName == gridName);
        }
        else
        {
            return null;
        }
    }

    public void UpdateGridData(GridData newData)
    {
        if (gridDatas == null) gridDatas = new List<GridData>();
        //����Ѵ��ڣ������
        int index = gridDatas.FindIndex(data => data != null && data.gridName == newData.gridName);
        if (index != -1)
        {
            gridDatas[index] = newData;
        }
        else
        {
            gridDatas.Add(newData);
        }
    }

    public void RemoveGridData(string gridName)
    {
        if (gridDatas == null) gridDatas = new List<GridData>();
        if (gridDatas.Any(data => data != null && data.gridName == gridName))
            gridDatas.RemoveAll(data => data != null && data.gridName == gridName);
    }

    public void SaveGridData()
    {
        JsonMgr.Instance.SaveData(gridDatas, "GridDatas");
    }

    public void SaveMapData(Map map)
    {
        this.mapData = map;
        JsonMgr.Instance.SaveData(mapData, "MapData");
    }

    //������Ϸ��Դ
    public void UpdateGameResData(GameRes res)
    {
        gameResData = res;
    }

    //������Ϸ��Դ
    public void SaveGameResData()
    {
        JsonMgr.Instance.SaveData(gameResData, "GameRes");
    }

    //�������
    public void ClearGameData()
    {
        mapData = null;
        gridDatas = null;
        gameResData = null;
        SaveGridData();
        SaveMapData(null);
    }
}
