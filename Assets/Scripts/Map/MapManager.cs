using System.Linq;
using System.Xml;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapConfig config; //地图配置信息
    public MapView view; //地图显示器

    public Map CurrentMap { get; private set; } //当前的地图

    private void Start()
    {
        GenerateNewMap();
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
}
