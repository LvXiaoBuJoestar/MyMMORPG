using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    [HideInInspector] public TabView tabView;
    [HideInInspector] public int index;

    [SerializeField] private Sprite activeImage;
    Sprite normalImage;

    Button button;
    Image image;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();  
        image = GetComponentInChildren<Image>();

        normalImage = image.sprite;
        button.onClick.AddListener(OnClick);
    }

    public void Select(bool isSelect)
    {
        image.sprite = isSelect ? activeImage : normalImage;
    }

    private void OnClick()
    {
        tabView.SelectTab(index);
    }
}
