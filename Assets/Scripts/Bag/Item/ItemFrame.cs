using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemFrame : MonoBehaviour
{
    void Start()
    {
        (transform as RectTransform).sizeDelta = Vector2.one * Defines.cellSize;
    }
}
