using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 游戏数据管理器
/// </summary>
public class GameDataManager : Singleton<GameDataManager>
{
    public MusicData musicData;
    public List<GridData> gridDatas;
    public Map mapData;

    private GameDataManager()
    {
        Debug.Log(Application.persistentDataPath);
        musicData = JsonMgr.Instance.LoadData<MusicData>("MusicData") ?? new MusicData();
        gridDatas = JsonMgr.Instance.LoadData<List<GridData>>("GridDatas") ?? new List<GridData>();
        mapData = JsonMgr.Instance.LoadData<Map>("MapData") ?? new Map();
    }

    /// <summary>
    /// 保存音乐数据
    /// </summary>
    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicData,"MusicData");
    }

    /// <summary>
    /// 获取背包数据
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
        //如果已存在，则更新
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

    //清空数据
    public void ClearGameData()
    {
        mapData = null;
        gridDatas = null;
        SaveGridData();
        SaveMapData(null);
    }
}
