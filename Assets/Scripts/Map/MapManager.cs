using Newtonsoft.Json;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapConfig config; //��ͼ������Ϣ
    public MapView view; //��ͼ��ʾ��

    private bool isQuit; //�Ƿ��˳���Ϸ

    public Map CurrentMap { get; private set; } //��ǰ�ĵ�ͼ

    private void Start()
    {
        isQuit = false;
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
        Debug.Log("�����µ�ͼ");   
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
        if (PlayerStateManager.Instance.CurrentState != PlayerState.Map && CurrentMap.path.Count >= 1)
        {
            CurrentMap.path.RemoveAt(CurrentMap.path.Count - 1); //�������;�˳�����ָ�����һ���ڵ�
        }
        SaveMap();
        isQuit = true;
    }

    private void OnDestroy()
    {
        Debug.Log("�ݻ�");
        Debug.Log(PlayerStateManager.Instance.CurrentState);
        Debug.Log(CurrentMap.path.Count);
        //��Ϸ�˳�ʱ�����жϣ������ظ��жϣ�
        if (!isQuit && PlayerStateManager.Instance.CurrentState != PlayerState.Map && CurrentMap.path.Count >= 1)
        {
            //MenuPanel panel = UIManager.Instance.GetPanel<MenuPanel>();
            //if (panel != null)
            //{
            //    CurrentMap.path.RemoveAt(CurrentMap.path.Count - 1); //����ǲ˵����˳��ģ��������Ķ�Ҫ�˻ص���һ���ڵ�
            //}
            if (PlayerStateManager.Instance.CurrentState != PlayerState.Fight &&
                    PlayerStateManager.Instance.CurrentState != PlayerState.Boss)
            {
                Debug.Log("�˻ؽڵ�");
                //�������;�˳�����ָ�����һ���ڵ�,������ڼ���ս��ҳ��ʱ��ɾ���ģ�����ɾ��
                CurrentMap.path.RemoveAt(CurrentMap.path.Count - 1);
            }
        }
        SaveMap();
    }
}
