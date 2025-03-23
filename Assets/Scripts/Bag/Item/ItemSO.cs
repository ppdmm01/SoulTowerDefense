using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/// <summary>
/// ��Ʒ����
/// </summary>
[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObject/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("��������")]
    public int id; //��Ʒid
    public string itemName; //��Ʒ����
    public string itemChineseName; //��Ʒ��������
    [Header("�۸�")]
    public int price; //�۸�
    [TextArea]
    public string description; //��Ʒ����
    [Header("��Ʒռ����ɫ����")]
    public Defines.SlotColorType slotColorType;
    [Header("��Ʒ��״")]
    public ItemShape shape; // ��Ʒռ����״  
    [Header("����")]
    public List<DetectionPoint> detectionPoints; //����
    [Header("��Ʒ����")]
    public List<ItemAttribute> itemAttributes; //��Ʒ����
    public Sprite icon; //ͼƬ

    [Header("��Ʒ��ǩ")]
    public List<ItemTag> itemTags; // ��Ʒ��ǩ

    //��ȡ��Ʒ�����б�
    public List<ItemAttribute> GetItemAttributes()
    {
        List<ItemAttribute> list = new List<ItemAttribute>();
        for (int i = 0; i < itemAttributes.Count; i++)
            list.Add(new ItemAttribute(itemAttributes[i]));
        return list;
    }
}

/// <summary>
/// ��Ʒ��״
/// </summary>
[Serializable]
public class ItemShape
{
    public static int MatrixLen = 4; //����߳�

    [Tooltip("4x4����״����true��ʾռλ")]
    public bool[] shapeMatrix = new bool[MatrixLen * MatrixLen]; // ���л�Ϊһά���鷽��༭  

    [Tooltip("�����Զ���ת")]
    public bool allowRotation = true;

    //��ȡ��Ʒʵ��ռ�óߴ磨������Ʒ��ת�Ƕȣ���ȡ�ߴ��С��
    public Vector2Int GetEffectiveSize(int rotation)
    {
        // ������ת�Ƕȼ�����Ч�ߴ�  
        bool[,] matrix = GetRotatedMatrix(rotation);
        int minX = MatrixLen, minY = MatrixLen;
        int maxX = 0, maxY = 0;

        for (int y = 0; y < MatrixLen; y++)
        {
            for (int x = 0; x < MatrixLen; x++)
            {
                if (matrix[x, y])
                {
                    minX = Mathf.Min(minX, x);
                    minY = Mathf.Min(minY, y);
                    maxX = Mathf.Max(maxX, x);
                    maxY = Mathf.Max(maxY, y);
                }
            }
        }

        // ������Ч�ߴ�  
        return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
    }

    //��ȡ��Ʒ������ʼ�㣨���½ǵ㣩��ԭ��(0,0)��ƫ��
    public Vector2Int GetOriginOffset(int rotation)
    {
        bool[,] matrix = GetRotatedMatrix(rotation);

        int minX = MatrixLen, minY = MatrixLen;
        for (int x = 0; x < MatrixLen; x++)
        {
            for (int y = 0; y < MatrixLen; y++)
            {
                if (matrix[x, y])
                {
                    minX = Mathf.Min(minX, x);
                    minY = Mathf.Min(minY, y);
                }
            }
        }
        return new Vector2Int(minX, minY);
    }

    //��ȡ��Ʒ�������Ͻǵ���ԭ��(0,0)��ƫ��
    public Vector2Int GetEndOffset(int rotation)
    {
        bool[,] matrix = GetRotatedMatrix(rotation);

        int MaxX = 0, MaxY = 0;
        for (int x = 0; x < MatrixLen; x++)
        {
            for (int y = 0; y < MatrixLen; y++)
            {
                if (matrix[x, y])
                {
                    MaxX = Mathf.Max(MaxX, x);
                    MaxY = Mathf.Max(MaxY, y);
                }
            }
        }
        return new Vector2Int(MaxX, MaxY);
    }

    // ������ת��90��Ϊ����������ȡ��ת�����״��Ϣ 
    public bool[,] GetRotatedMatrix(int rotation)
    {
        bool[,] rotated = new bool[MatrixLen, MatrixLen];
        int steps = (rotation / 90) % 4;
        for (int x = 0; x < MatrixLen; x++)
        {
            for (int y = 0; y < MatrixLen; y++)
            {
                switch (steps)
                {
                    case 0: rotated[x, y] = shapeMatrix[x * MatrixLen + y]; break; //0
                    case 1: rotated[MatrixLen - y - 1, x] = shapeMatrix[x * MatrixLen + y]; break; // ��ʱ��90��˳ʱ��270��
                    case 2: rotated[MatrixLen - x - 1, MatrixLen - y - 1] = shapeMatrix[x * MatrixLen + y]; break; // 180  
                    case 3: rotated[y, MatrixLen - x - 1] = shapeMatrix[x * MatrixLen + y]; break; // ��ʱ��270��˳ʱ��90��
                }
            }
        }
        return rotated;
    }
}

/// <summary>
/// ���㣬���ڼ����Ʒ��Χָ�������Ʒ
/// </summary>
[Serializable]
public class DetectionPoint
{
    [Tooltip("�����Ʒԭ���ƫ����")]
    public Vector2Int pos;

    [Tooltip("��������")]
    public PointType pointType;

    public enum PointType
    {
        Star, //����ͼ��
        Fire, //����ͼ��
    }

    //��ȡָ���Ƕȵļ������ԭ��λ��
    public Vector2Int GetRotatePoint(int rotation)
    {
        int steps = (rotation / 90) % 4;
        switch (steps)
        {
            case 0: return pos; //0
            case 1: return new Vector2Int(ItemShape.MatrixLen - pos.y - 1, pos.x); // ��ʱ��90��˳ʱ��270��
            case 2: return new Vector2Int(ItemShape.MatrixLen - pos.x - 1, ItemShape.MatrixLen - pos.y - 1); // 180  
            case 3: return new Vector2Int(pos.y, ItemShape.MatrixLen - pos.x - 1); // ��ʱ��270��˳ʱ��90��
            default: return pos;
        }
    }
}
