using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour, ISelectHandler
{
    [SerializeField] Image icon;
    [SerializeField] Text title;
    [SerializeField] Text count;
    [SerializeField] Text price;

    [SerializeField] Image background;
    private Color normalColor;
    private Color selectedColor;

    private void Awake()
    {
        normalColor = background.color;
        selectedColor = background.color;
        selectedColor.a = 0.4f;
    }

    private bool isSelected;
    public bool IsSelected {
        get { return isSelected; }
        set
        {
            isSelected = value;
            background.color = isSelected ? selectedColor : normalColor;
        }
    }

    public int ShopItemId { get; set; }
    private UIShop uIShop;

    private ItemDefine itemDefine;
    private ShopItemDefine ShopItemDefine {  get; set; }

    public void SetShopItem(int id, ShopItemDefine shopItemDefine, UIShop owner)
    {
        this.uIShop = owner;
        this.ShopItemId = id;
        this.ShopItemDefine = shopItemDefine;
        this.itemDefine = DataManager.Instance.Items[ShopItemDefine.ItemID];

        this.title.text = this.itemDefine.Name;
        this.count.text = this.ShopItemDefine.Count.ToString();
        this.price.text = this.ShopItemDefine.Price.ToString();
        this.icon.sprite = Resloader.Load<Sprite>(itemDefine.Icon);
    }

    public void OnSelect(BaseEventData eventData)
    {
        IsSelected = true;
        uIShop.SelectShopItem(this);
    }
}
