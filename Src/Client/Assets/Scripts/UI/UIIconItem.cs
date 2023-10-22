using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour
{
    [SerializeField] Image mainImage;
    [SerializeField] Text mainText;

    public void SetMainIcon(string iconName, string text)
    {
        mainImage.overrideSprite = Resloader.Load<Sprite>(iconName);
        mainText.text = text;
    }
}
