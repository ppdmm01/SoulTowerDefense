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
    public string itemName; //��Ʒ����
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
    private static int MatrixLen = 4; //����߳�

    [Tooltip("4x4����״����true��ʾռλ")]
    public bool[] shapeMatrix = new bool[MatrixLen * MatrixLen]; // ���л�Ϊһά���鷽��༭  

    [Tooltip("�����Զ���ת")]
    public bool allowRotation = true;

    // ��ȡ��Ʒʵ��ռ�óߴ� 
    public Vector2Int GetEffectiveSize(int rotation)
    {
        // ������ת�Ƕȼ�����Ч�ߴ�  
        bool[,] matrix = GetRotatedMatrix(rotation);
        return CalculateBounds(matrix);
    }

    //������Ʒ��Чռ�óߴ�
    private Vector2Int CalculateBounds(bool[,] matrix)
    {
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

    // ������ת��90��Ϊ������ ����ȡ��ת�����״��Ϣ 
    private bool[,] GetRotatedMatrix(int rotation)
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
                    case 1: rotated[y, MatrixLen - x - 1] = shapeMatrix[x * MatrixLen + y]; break; // ˳ʱ��90  
                    case 2: rotated[MatrixLen - x - 1, MatrixLen - y - 1] = shapeMatrix[x * MatrixLen + y]; break; // 180  
                    case 3: rotated[MatrixLen - y - 1, x] = shapeMatrix[x * MatrixLen + y]; break; // ��ʱ��90��˳ʱ��270��
                }
            }
        }
        return rotated;
    }
}

