using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��������Ϣ��ť
/// </summary>
public class TowerInfoBtn : MonoBehaviour
{
    public Button btn;
    public Image icon;
    private TowerData data;

    /// <summary>
    /// ��ʼ����ť
    /// </summary>
    /// <param name="data">����������</param>
    public void InitInfo(TowerData data)
    {
        btn.onClick.RemoveAllListeners();
        this.data = data;
        icon.sprite = data.towerIcon;
        btn.onClick.AddListener(() =>
        {
            BagPanel panel = UIManager.Instance.GetPanel<BagPanel>();
            panel.UpdateTowerInfo(data.towerName); //���������Ϣ
            panel.nowTowerInfoName = data.towerName; //���µ�ǰչʾ�ķ�������
        });
    }

    /// <summary>
    /// ��ȡ���
    /// </summary>
    public float GetWeight()
    {
        return btn.image.rectTransform.sizeDelta.x;
    }
}
