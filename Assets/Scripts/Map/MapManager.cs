using Newtonsoft.Json;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapConfig config; //地图配置信息
    public MapView view; //地图显示器

    private bool isQuit; //是否退出游戏

    public Map CurrentMap { get; private set; } //当前的地图

    private void Start()
    {
        isQuit = false;
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
        Debug.Log("生成新地图");   
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
        if (PlayerStateManager.Instance.CurrentState != PlayerState.Map && CurrentMap.path.Count >= 1)
        {
            CurrentMap.path.RemoveAt(CurrentMap.path.Count - 1); //如果在中途退出，则恢复到上一个节点
        }
        SaveMap();
        isQuit = true;
    }

    private void OnDestroy()
    {
        Debug.Log("摧毁");
        Debug.Log(PlayerStateManager.Instance.CurrentState);
        Debug.Log(CurrentMap.path.Count);
        //游戏退出时不用判断（避免重复判断）
        if (!isQuit && PlayerStateManager.Instance.CurrentState != PlayerState.Map && CurrentMap.path.Count >= 1)
        {
            //MenuPanel panel = UIManager.Instance.GetPanel<MenuPanel>();
            //if (panel != null)
            //{
            //    CurrentMap.path.RemoveAt(CurrentMap.path.Count - 1); //如果是菜单中退出的，无论在哪都要退回到上一个节点
            //}
            if (PlayerStateManager.Instance.CurrentState != PlayerState.Fight &&
                    PlayerStateManager.Instance.CurrentState != PlayerState.Boss)
            {
                Debug.Log("退回节点");
                //如果在中途退出，则恢复到上一个节点,如果是在加载战斗页面时被删除的，则不用删除
                CurrentMap.path.RemoveAt(CurrentMap.path.Count - 1);
            }
        }
        SaveMap();
    }
}
