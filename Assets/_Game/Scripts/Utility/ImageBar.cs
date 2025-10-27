using UnityEngine;
using UnityEngine.UI;

public class ImageBar : Image
{
    public float minSize;
    public float maxSize;

    void Update()
    {
        rectTransform.sizeDelta = new Vector2(Mathf.Lerp(minSize, maxSize,fillAmount), rectTransform.sizeDelta.y); 
    }
}
