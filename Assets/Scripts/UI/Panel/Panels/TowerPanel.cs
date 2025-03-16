using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : BasePanel
{
    public TextMeshProUGUI qiNumText; //��������Դ�����ı�
    public TextMeshProUGUI waveNumText; //����
    public TextMeshProUGUI nowEnemyNumText; //ʣ���������
    public Transform towerBtnContent; //���ð�ť��λ��
    private List<CreateTowerBtn> towerBtnList = new List<CreateTowerBtn>(); //�洢���÷������İ�ť
    public override void Init()
    {

    }

    /// <summary>
    /// ��ʼ�����������ð�ť
    /// </summary>
    public void InitTowerBtn()
    {
        Debug.Log(TowerManager.Instance.towerDatas.Count);
        foreach (TowerData towerData in TowerManager.Instance.towerDatas.Values)
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

    /// <summary>
    /// ��������Դ����
    /// </summary>
    /// <param name="num"></param>
    public void UpdateQiNum(int num)
    {
        qiNumText.text = num.ToString();    
    }

    /// <summary>
    /// ���²���
    /// </summary>
    /// <param name="waveNum"></param>
    public void UpdateWaveInfo(int waveNum,int totalWaveNum)
    {
        waveNumText.text = $"��{waveNum}/{totalWaveNum}��";
    }

    /// <summary>
    /// ���µ�������
    /// </summary>
    /// <param name="num"></param>
    public void UpdateEnemyNum(int num)
    {
        nowEnemyNumText.text = num.ToString();
    }
}
