using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 地图操作跟踪器，检测玩家操作
/// </summary>
public class MapPlayerTracker : MonoBehaviour
{
    public bool lockAfterSelecting = false; //选择节点后是否锁住操作（若为true，记得完成节点的事件后将Locked置为false）
    public float enterNodeDelay = 1f; //进入节点事件的延迟时间
    public MapManager mapManager;
    public MapView view;

    public static MapPlayerTracker Instance;

    public bool Locked { get; set; } //当前操作是否锁住

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 选择节点操作
    /// </summary>
    public void SelectNode(MapNode mapNode)
    {
        if (Locked) return;

        if (mapManager.CurrentMap.path.Count == 0)
        {
            // 玩家没有选择过路径，可以从第一层的任意节点开始
            if (mapNode.Node.point.y == 0)
                SendPlayerToNode(mapNode);
            else
                PlayWarningThatNodeCannotBeAccessed();
        }
        else
        {
            Vector2Int currentPoint = mapManager.CurrentMap.path[mapManager.CurrentMap.path.Count - 1];
            Node currentNode = mapManager.CurrentMap.GetNode(currentPoint);

            // 如果玩家选择的是当前路径可达的下一节点，则成功
            if (currentNode != null && currentNode.outgoing.Any(point => point.Equals(mapNode.Node.point)))
                SendPlayerToNode(mapNode);
            else
                PlayWarningThatNodeCannotBeAccessed();
        }
    }

    //让玩家前往节点
    private void SendPlayerToNode(MapNode mapNode)
    {
        Locked = lockAfterSelecting;
        mapManager.CurrentMap.path.Add(mapNode.Node.point);
        view.SetAttainableNodes();
        view.SetLineColors();
        mapNode.ShowSwirlAnimation();

        DOTween.Sequence().AppendInterval(enterNodeDelay).OnComplete(() => EnterNode(mapNode)); //等待一段时间后进入节点
    }

    /// <summary>
    /// 进入节点，执行节点的操作
    /// </summary>
    private static void EnterNode(MapNode mapNode)
    {
        Debug.Log("进入节点: " + mapNode.Node.blueprintName + " 节点类型: " + mapNode.Node.nodeType);
        //触发进入节点事件
        EventCenter.Instance.EventTrigger(EventType.EnterMapNode);
        //若lockAfterSelecting为true，记得完成节点的事件后将Locked置为false
        switch (mapNode.Node.nodeType)
        {
            case NodeType.MinorEnemy:
                UIManager.Instance.HidePanel<MapPanel>();
                UIManager.Instance.ShowPanel<PreFightPanel>();
                PlayerStateManager.Instance.ChangeState(PlayerState.Fight);
                Instance.Locked = false;
                break;
            case NodeType.Boss:
                UIManager.Instance.HidePanel<MapPanel>();
                UIManager.Instance.ShowPanel<PreFightPanel>();
                PlayerStateManager.Instance.ChangeState(PlayerState.Boss);
                Instance.Locked = false;
                break;
            case NodeType.Crystal:
                Instance.Locked = false;
                //未实现
                break;
            case NodeType.Store:
                UIManager.Instance.HidePanel<MapPanel>();
                UIManager.Instance.ShowPanel<StorePanel>();
                PlayerStateManager.Instance.ChangeState(PlayerState.Store);
                Instance.Locked = false;
                break;
            case NodeType.Forge:
                UIManager.Instance.HidePanel<MapPanel>();
                UIManager.Instance.ShowPanel<ForgePanel>();
                PlayerStateManager.Instance.ChangeState(PlayerState.Forge);
                Instance.Locked = false;
                break;
            case NodeType.Treasure:
                Instance.Locked = false;
                UIManager.Instance.HidePanel<MapPanel>();
                UIManager.Instance.ShowPanel<RewardPanel>().SetReward("宝箱！", new List<RewardData>()
                {
                    new RewardData(RewardType.Taixu,40,60),
                    new RewardData(RewardType.Item,0,0),
                    new RewardData(RewardType.Item,0,0)
                });
                PlayerStateManager.Instance.ChangeState(PlayerState.Select);
                break;
            case NodeType.Event:
                UIManager.Instance.HidePanel<MapPanel>();
                UIManager.Instance.ShowPanel<EventPanel>();
                PlayerStateManager.Instance.ChangeState(PlayerState.Event);
                Instance.Locked = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    //玩家选择节点失败
    private void PlayWarningThatNodeCannotBeAccessed()
    {
        Debug.Log("当前选择的节点不可达！！！");
    }
}
