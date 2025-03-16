using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapConfig config; //地图配置信息
    public MapView view; //地图显示器

    public Map CurrentMap { get; private set; } //当前的地图

    private void Start()
    {
        Map map = GameDataManager.Instance.mapData;
        if (map != null && map.nodes != null && map.path != null)
        {
            if (map.path.Any(p => p.Equals(map.GetBossNode().point)))
            {
                //玩家已经到达boss节点，生成新地图
                GenerateNewMap();
            }
            else
            {
                CurrentMap = map;
                //生成目前的地图
                view.ShowMap(map);
            }
        }
        else
        {
            GenerateNewMap();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        Map map = MapGenerator.GetMap(config); //根据配置表生成随机的地图
        CurrentMap = map;
        view.ShowMap(map); //显示地图
    }

    public void SaveMap()
    {
        if (CurrentMap == null) return;
        GameDataManager.Instance.SaveMapData(CurrentMap);
    }

    private void OnApplicationQuit()
    {
        SaveMap();
    }

    private void OnDestroy()
    {
        SaveMap();
    }
}
