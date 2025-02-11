using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagManager : SingletonMono<BagManager>
{
    private BagManager() {}

    protected override void Awake()
    {
        base.Awake();
        BagDic = new Dictionary<string, BagGrid>();
        for (int i = 0; i < bags.Count; i++)
        {
            Debug.Log(bags[i].bagName);
            Debug.Log(bags[i]);
            BagDic.Add(bags[i].bagName, bags[i]);
        }
    }

    [SerializeField] private List<BagGrid> bags; //�����б������������ж��������һ��������һ�������䣩
    public Dictionary<string, BagGrid> BagDic; //ͨ���ֵ���Ҷ�Ӧ����
}
