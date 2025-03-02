using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����һЩ����
/// </summary>
public static class Defines
{
    //������س���
    public static readonly float cellSize = 48f; //ÿ�����Ӵ�С
    public static readonly float spacing = 2f; //ÿ�����Ӽ��
    public static readonly Color invalidColor = new Color32(200,200,200,150); //������ռ��ʱ����ɫ
    public static readonly Color validColor = Color.white; //������δռ��ʱ����ɫ
    public static readonly Color previewInvalidColor = Color.red; //Ԥ���Ƿ���ɫ
    public static readonly Color previewValidColor = Color.green; //Ԥ���Ϸ���ɫ
    public static readonly Vector2Int nullValue = new Vector2Int(-1,-1); //��ʼ��ֵ���൱��null

    //������س���
    public static readonly Color validRangeColor = new Color(0, 0, 0, 0.2f); //�Ϸ���Χ����ɫ
    public static readonly Color invalidRangeColor = new Color(1f, 0f, 0f, 0.2f); //��Ч��Χ����ɫ

    //������س���
    public static readonly float waitTime = 5f; //ÿ��֮�����Ϣʱ��
}
