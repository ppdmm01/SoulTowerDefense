using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/// <summary>
/// 物品数据
/// </summary>
[CreateAssetMenu(fileName = "ItemSO",menuName = "ScriptableObject/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("基础属性")]
    public int id; //物品id
    public string itemName; //物品名称
    public string description; //物品描述
    public ItemShape shape; // 物品占格形状  
    public Sprite icon; //图片

    [Header("物品标签")]
    public string[] itemTags; // 物品标签（如"力","药水"等）  
}

/// <summary>
/// 物品形状
/// </summary>
[Serializable]
public class ItemShape 
{
    public static int MatrixLen = 4; //矩阵边长

    [Tooltip("4x4的形状矩阵，true表示占位")]
    public bool[] shapeMatrix = new bool[MatrixLen * MatrixLen]; // 序列化为一维数组方便编辑  

    [Tooltip("允许自动旋转")]
    public bool allowRotation = true;

    //获取物品实际占用尺寸（传入物品旋转角度，获取尺寸大小）
    public Vector2Int GetEffectiveSize(int rotation)
    {
        // 根据旋转角度计算有效尺寸  
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

        // 返回有效尺寸  
        return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
    }

    //获取物品矩形起始点（左下角点）离原点(0,0)的偏移
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

    //获取物品矩形右上角点离原点(0,0)的偏移
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

    // 矩阵旋转（90度为步长） ，获取旋转后的形状信息 
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
                    case 1: rotated[MatrixLen - y - 1, x] = shapeMatrix[x * MatrixLen + y]; break; // 逆时针90（顺时针270）
                    case 2: rotated[MatrixLen - x - 1, MatrixLen - y - 1] = shapeMatrix[x * MatrixLen + y]; break; // 180  
                    case 3: rotated[y, MatrixLen - x - 1] = shapeMatrix[x * MatrixLen + y]; break; // 逆时针270（顺时针90）
                }
            }
        }
        return rotated;
    }
}

