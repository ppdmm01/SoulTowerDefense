using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum PlayerState
{
    Map, //地图
    Fight, //战斗
    Select, //选择
    Store, //商店
    Forge, //锻造
    Event, //事件
    Crystal, //水晶
    Boss, //终点
    Menu, //菜单
}
public class PlayerStateManager : Singleton<PlayerStateManager>
{
    private PlayerStateManager() { }

    public PlayerState CurrentState { get; private set; }

    public void ChangeState(PlayerState state)
    {
        CurrentState = state;
    }
}
