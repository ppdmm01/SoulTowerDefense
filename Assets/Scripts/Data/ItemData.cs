using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��Ʒ����
public class ItemData
{
    public int id; //��ƷId
    public int currentRotation; //��ǰ��ת����
    public Vector2Int gridPos; //����λ��
    public int growSpeed; //�ɳ��ٶ�
    public List<ItemAttribute> itemAttributes; //��Ʒ��ǰ���ԣ������гɳ�����Ҫ��¼��
    public List<BuffType> itemBuffs; //��Ʒ��ǰӵ�е�buff����Է��������ߣ�

    public ItemData() { }
    public ItemData(int id, int currentRotation, Vector2Int gridPos,int growSpeed, 
        List<ItemAttribute> itemAttributes, List<BuffType> itemBuffs)
    {
        this.id = id;
        this.currentRotation = currentRotation;
        this.gridPos = gridPos;
        this.growSpeed = growSpeed;
        this.itemAttributes = itemAttributes;
        this.itemBuffs = itemBuffs;
    }

    //������Ʒ�ɳ�
    public void Grow()
    {
        foreach (ItemAttribute itemAttribute in itemAttributes)
        {
            itemAttribute.Grow(growSpeed);
        }
    }
}
