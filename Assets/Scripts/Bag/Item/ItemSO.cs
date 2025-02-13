using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/// <summary>
/// ��Ʒ����
/// </summary>
[CreateAssetMenu(fileName = "ItemSO",menuName = "ScriptableObject/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("��������")]
    public int id; //��Ʒid
    public string itemName; //��Ʒ����
    public string description; //��Ʒ����
    public ItemShape shape; // ��Ʒռ����״  
    public Sprite icon; //ͼƬ

    [Header("��Ʒ��ǩ")]
    public string[] itemTags; // ��Ʒ��ǩ����"��","ҩˮ"�ȣ�  
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

    // ������ת��90��Ϊ������ ����ȡ��ת�����״��Ϣ 
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

