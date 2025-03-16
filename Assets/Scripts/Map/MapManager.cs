using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapConfig config; //��ͼ������Ϣ
    public MapView view; //��ͼ��ʾ��

    public Map CurrentMap { get; private set; } //��ǰ�ĵ�ͼ

    private void Start()
    {
        Map map = GameDataManager.Instance.mapData;
        if (map != null && map.nodes != null && map.path != null)
        {
            if (map.path.Any(p => p.Equals(map.GetBossNode().point)))
            {
                //����Ѿ�����boss�ڵ㣬�����µ�ͼ
                GenerateNewMap();
            }
            else
            {
                CurrentMap = map;
                //����Ŀǰ�ĵ�ͼ
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
        Map map = MapGenerator.GetMap(config); //�������ñ���������ĵ�ͼ
        CurrentMap = map;
        view.ShowMap(map); //��ʾ��ͼ
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
