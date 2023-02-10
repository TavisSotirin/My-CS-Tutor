using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPanel : MonoBehaviour
{
    public RectTransform rect;
    public Image image;

    public Color color
    {
        get { return image.color; }
        set { image.color = value; }
    }

    public void SetRandomColor()
    {
        image.color = new(Random.value, Random.value, Random.value);
    }
}
