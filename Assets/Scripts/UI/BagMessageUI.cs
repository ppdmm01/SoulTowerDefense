using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 测试用：查看背包信息
/// </summary>
public class BagMessageUI : MonoBehaviour
{
    public TextMeshProUGUI bagItemInfo;
    public TextMeshProUGUI boxItemInfo;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string bagInfo = "";
            string boxInfo = "";
            foreach (Item item in BagManager.Instance.BagDic["bag"].items)
            {
                bagInfo += " -- " + item.name + " -- gridPos:" + (item.gridPos) + "\n";
            }
            foreach (Item item in BagManager.Instance.BagDic["storageBox"].items)
            {
                boxInfo += " -- " + item.name + " -- gridPos:" + (item.gridPos) + "\n";
            }
            bagItemInfo.text = bagInfo;
            boxItemInfo.text = boxInfo;
        }
    }
}
