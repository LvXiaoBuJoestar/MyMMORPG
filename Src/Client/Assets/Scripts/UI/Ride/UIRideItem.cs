using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRideItem : ListView.ListViewItem
{
    [SerializeField] Image icon;
    [SerializeField] Text title;
    [SerializeField] Text level;

    [SerializeField] Image background;
    private Color normalColor;
    private Color selectedColor;

    private void Awake()
    {
        normalColor = background.color;
        selectedColor = background.color;
        selectedColor.a = 0.25f;
    }

    public override void onSelected(bool selected)
    {
        background.color = selected ? selectedColor : normalColor;
    }

    public Item item;
    public void SetRideItem(Item item, UIRide owner, bool equiped)
    {
        this.item = item;
        if (this.title != null) this.title.text = this.item.itemDefine.Name;
        if (this.level != null) this.level.text = this.item.itemDefine.Level.ToString();
        if (this.icon != null) this.icon.sprite = Resloader.Load<Sprite>(this.item.itemDefine.Icon);
    }
}
