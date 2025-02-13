using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class BagManager : SingletonMono<BagManager>
{
    private BagManager() {}

    protected override void Awake()
    {
        base.Awake();
        BagDic = new Dictionary<string, BagGrid>();
    }

    [HideInInspector] public Transform itemsTrans; //������Ʒ�ĸ�����
    public Dictionary<string, BagGrid> BagDic; //ͨ���ֵ䣨���������ж��������һ��������һ�������䣩

    //��������Ƿ���ָ��������
    public bool IsInsideBag(Vector2 screenPoint,string bagName)
    {
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(
           BagDic[bagName].transform as RectTransform,
           screenPoint,
           null
        );

       return isInside;
    }

    //��ָ���������ָ����Ʒ
    public void AddItemByName(string name,BagGrid bag)
    {
        GameObject itemObj = Instantiate(Resources.Load<GameObject>("Items/"+name));
        itemObj.transform.SetParent(itemsTrans,false);
        Item item = itemObj.GetComponent<Item>();
        item.Init(bag);
        if (!bag.TryAutoPlaceItem(item))
        {
            Debug.LogError($"��Ʒ {item.data.itemName} �޷����ã�������������");
            //TODO:��������ɾ��������һҳ�ȣ�
            bag.items.Remove(item);
            Destroy(itemObj);
        }
    }
}
