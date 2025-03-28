using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ���������
/// </summary>
public class ForgeManager : Singleton<ForgeManager>
{
    public SynthesisManagerSO data; //�ϳ�����
    private ForgeManager() 
    {
        if (data == null)
        {
            data = Resources.Load<SynthesisManagerSO>("Data/SynthesisManagerSO");
            if (data == null)
                Debug.LogError("����SynthesisManagerSOʧ�ܣ�");
        }
    }

    ///// <summary>
    ///// ��ȡ�ϳ��䷽����
    ///// </summary>
    ///// <param name="id">�䷽id</param>
    ///// <returns></returns>
    //public SynthesisSO GetSynthesisData(int id)
    //{
    //    return data.synthesisSOList.Find(synthesis => synthesis.id == id);
    //}

    ///// <summary>
    ///// ��ȡ�ϳ��䷽����
    ///// </summary>
    ///// <param name="productName">��Ʒ����</param>
    ///// <returns></returns>
    //public SynthesisSO GetSynthesisData(string productName)
    //{
    //    return data.synthesisSOList.Find(synthesis => synthesis.product.itemName == productName);
    //}

}
