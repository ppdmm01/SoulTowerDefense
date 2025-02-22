using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : BasePanel
{
    public TextMeshProUGUI qiNumText; //��������Դ�����ı�
    public HealthBar hpBar; //����Ѫ��
    public Transform towerBtnContent; //���ð�ť��λ��
    private List<CreateTowerBtn> towerBtnList = new List<CreateTowerBtn>(); //�洢���÷������İ�ť
    public override void Init()
    {
        ChangeQiNum(0);
    }

    /// <summary>
    /// ��ʼ�����������ð�ť
    /// </summary>
    public void InitTowerBtn()
    {
        foreach (TowerData towerData in TowerManager.Instance.towers.Values)
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("UI/UIObj/CreateTowerBtn"));
            obj.transform.SetParent(towerBtnContent, false);
            CreateTowerBtn btn = obj.GetComponent<CreateTowerBtn>();
            btn.SetTower(towerData);
            towerBtnList.Add(btn);
        }
    }

    /// <summary>
    /// �ݻ����з��÷�������ť
    /// </summary>
    public void DestroyAllTowerBtn()
    {
        foreach (CreateTowerBtn btn in towerBtnList)
            Destroy(btn);
        towerBtnList.Clear();
    }

    public void ChangeQiNum(int num)
    {
        qiNumText.text = num.ToString();    
    }
}
