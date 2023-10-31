using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image icon;
    [SerializeField] Text title;
    [SerializeField] Text level;
    [SerializeField] Text limitClass;

    [SerializeField] Image background;
    private Color normalColor;
    private Color selectedColor;

    private void Awake()
    {
        if (background == null) return;
        normalColor = background.color;
        selectedColor = background.color;
        selectedColor.a = 0.4f;
    }

    private bool isSelected;
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            if (background != null)
                background.color = isSelected ? selectedColor : normalColor;
        }
    }

    public int index;
    private UICharEquip owner;

    private Item item;
    private bool isEquiped = false;

    public void SetEquipItem(int index, Item item, UICharEquip uICharEquip, bool equiped)
    {
        this.index = index;
        this.item = item;
        this.owner = uICharEquip;
        this.isEquiped = equiped;

        if (this.icon != null) icon.sprite = Resloader.Load<Sprite>(item.itemDefine.Icon);
        if (this.title != null) title.text = item.itemDefine.Name;
        if (this.level != null) level.text = item.itemDefine.Level.ToString();
        if (this.limitClass != null) limitClass.text = item.itemDefine.LimitClass.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEquiped)
            UnEquip();
        else
        {
            if (isSelected)
            {
                DoEquip();
                IsSelected = false;
            }
            else
                IsSelected = true;
        }
    }

    private void DoEquip()
    {
        owner.DoEquip(item);
    }

    private void UnEquip()
    {
        owner.UnEquip(item);
    }
}
