using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ��ͼ�����������������Ҳ���
/// </summary>
public class MapPlayerTracker : MonoBehaviour
{
    public bool lockAfterSelecting = false; //ѡ��ڵ���Ƿ���ס��������Ϊtrue���ǵ���ɽڵ���¼���Locked��Ϊfalse��
    public float enterNodeDelay = 1f; //����ڵ��¼����ӳ�ʱ��
    public MapManager mapManager;
    public MapView view;

    public static MapPlayerTracker Instance;

    public bool Locked { get; set; } //��ǰ�����Ƿ���ס

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// ѡ��ڵ����
    /// </summary>
    public void SelectNode(MapNode mapNode)
    {
        if (Locked) return;

        if (mapManager.CurrentMap.path.Count == 0)
        {
            // ���û��ѡ���·�������Դӵ�һ�������ڵ㿪ʼ
            if (mapNode.Node.point.y == 0)
                SendPlayerToNode(mapNode);
            else
                PlayWarningThatNodeCannotBeAccessed();
        }
        else
        {
            Vector2Int currentPoint = mapManager.CurrentMap.path[mapManager.CurrentMap.path.Count - 1];
            Node currentNode = mapManager.CurrentMap.GetNode(currentPoint);

            // ������ѡ����ǵ�ǰ·���ɴ����һ�ڵ㣬��ɹ�
            if (currentNode != null && currentNode.outgoing.Any(point => point.Equals(mapNode.Node.point)))
                SendPlayerToNode(mapNode);
            else
                PlayWarningThatNodeCannotBeAccessed();
        }
    }

    //�����ǰ���ڵ�
    private void SendPlayerToNode(MapNode mapNode)
    {
        Locked = lockAfterSelecting;
        mapManager.CurrentMap.path.Add(mapNode.Node.point);
        view.SetAttainableNodes();
        view.SetLineColors();
        mapNode.ShowSwirlAnimation();

        DOTween.Sequence().AppendInterval(enterNodeDelay).OnComplete(() => EnterNode(mapNode)); //�ȴ�һ��ʱ������ڵ�
    }

    /// <summary>
    /// ����ڵ㣬ִ�нڵ�Ĳ���
    /// </summary>
    private static void EnterNode(MapNode mapNode)
    {
        Debug.Log("����ڵ�: " + mapNode.Node.blueprintName + " �ڵ�����: " + mapNode.Node.nodeType);
        //��������ڵ��¼�
        EventCenter.Instance.EventTrigger(EventType.EnterMapNode);
        //��lockAfterSelectingΪtrue���ǵ���ɽڵ���¼���Locked��Ϊfalse
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
                //δʵ��
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
                UIManager.Instance.ShowPanel<RewardPanel>().SetReward("���䣡", new List<RewardData>()
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

    //���ѡ��ڵ�ʧ��
    private void PlayWarningThatNodeCannotBeAccessed()
    {
        Debug.Log("��ǰѡ��Ľڵ㲻�ɴ����");
    }
}
