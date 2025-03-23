using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ¡¨Ω”œﬂ
/// </summary>
public class LineConnection
{
    public LineRenderer lr;
    public MapNode from;
    public MapNode to;

    public LineConnection(LineRenderer lr, MapNode from, MapNode to)
    {
        this.lr = lr;
        this.from = from;
        this.to = to;
    }

    public void SetColor(Color color)
    {
        if (lr != null)
        {
            Gradient gradient = lr.colorGradient;
            GradientColorKey[] colorKeys = gradient.colorKeys;
            for (int j = 0; j < colorKeys.Length; j++)
            {
                colorKeys[j].color = color;
            }

            gradient.colorKeys = colorKeys;
            lr.colorGradient = gradient;
        }
    }
}
