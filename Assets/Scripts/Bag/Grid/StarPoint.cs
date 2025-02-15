using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ¿ØÖÆÐÇÐÇµÄÏÔÒþ
/// </summary>
public class StarPoint : MonoBehaviour
{
    public GameObject starShow;
    public GameObject starHide;

    public void SetStarActive(bool active)
    {
        if (active)
        {
            starHide.SetActive(false);
            starShow.SetActive(true);
        }
        else
        {
            starHide.SetActive(true);
            starShow.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
