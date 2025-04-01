using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum PlayerState
{
    Map, //��ͼ
    Fight, //ս��
    Select, //ѡ��
    Store, //�̵�
    Forge, //����
    Event, //�¼�
    Crystal, //ˮ��
    Boss, //�յ�
    Menu, //�˵�
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
