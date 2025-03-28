using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 锻造管理器
/// </summary>
public class ForgeManager : Singleton<ForgeManager>
{
    public SynthesisManagerSO data; //合成数据
    private ForgeManager() 
    {
        if (data == null)
        {
            data = Resources.Load<SynthesisManagerSO>("Data/SynthesisManagerSO");
            if (data == null)
                Debug.LogError("加载SynthesisManagerSO失败！");
        }
    }

    ///// <summary>
    ///// 获取合成配方数据
    ///// </summary>
    ///// <param name="id">配方id</param>
    ///// <returns></returns>
    //public SynthesisSO GetSynthesisData(int id)
    //{
    //    return data.synthesisSOList.Find(synthesis => synthesis.id == id);
    //}

    ///// <summary>
    ///// 获取合成配方数据
    ///// </summary>
    ///// <param name="productName">成品名称</param>
    ///// <returns></returns>
    //public SynthesisSO GetSynthesisData(string productName)
    //{
    //    return data.synthesisSOList.Find(synthesis => synthesis.product.itemName == productName);
    //}

}
