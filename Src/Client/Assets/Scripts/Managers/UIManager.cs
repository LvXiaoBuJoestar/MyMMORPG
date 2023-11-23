using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    class UIElement
    {
        public string resources;
        public bool cache;
        public GameObject instance;
    }

    private Dictionary<Type, UIElement> uIElements = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        uIElements.Add(typeof(UIBag), new UIElement() { resources = "UI/UIBag", cache = false });
        uIElements.Add(typeof(UIShop), new UIElement() { resources = "UI/UIShop", cache = false });
        uIElements.Add(typeof(UICharEquip), new UIElement() { resources = "UI/UICharEquip", cache = true });
        uIElements.Add(typeof(UIQuestSystem), new UIElement() { resources = "UI/UIQuestSystem", cache = false });
        uIElements.Add(typeof(UIQuestDialog), new UIElement() { resources = "UI/UIQuestDialog", cache = false });
        uIElements.Add(typeof(UIFriends), new UIElement() { resources = "UI/UIFriends", cache = true });
        uIElements.Add(typeof(UIGuild), new UIElement() { resources = "UI/UIGuild", cache = true });
        uIElements.Add(typeof(UIGuildList), new UIElement() { resources = "UI/UIGuildList", cache = true });
        uIElements.Add(typeof(UIGuildPopCreate), new UIElement() { resources = "UI/UIGuildPopCreate", cache = true });
        uIElements.Add(typeof(UIGuildPopNoGuild), new UIElement() { resources = "UI/UIGuildPopNoGuild", cache = true });
        uIElements.Add(typeof(UIGuildApplyList), new UIElement() { resources = "UI/UIGuildApplyList", cache = true });
        uIElements.Add(typeof(UISetting), new UIElement() { resources = "UI/UISetting", cache = true });
        uIElements.Add(typeof(UIPopChatMenu), new UIElement() { resources = "UI/UIPopChatMenu", cache = true });
    }

    public T Show<T>()
    {
        Type type = typeof(T);
        if (uIElements.ContainsKey(type))
        {
            UIElement element = uIElements[type];
            if (element.instance != null)
            {
                element.instance.SetActive(true);
            }
            else
            {
                GameObject uIPrefab = Resources.Load(element.resources) as GameObject;
                element.instance = GameObject.Instantiate(uIPrefab);
            }
            return element.instance.GetComponent<T>();
        }
        return default(T);
    }

    public void Close(Type type)
    {
        if(uIElements.ContainsKey(type))
        {
            UIElement uIElement = uIElements[type];
            if (uIElement.instance != null)
            {
                if (uIElement.cache)
                {
                    uIElement.instance.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(uIElement.instance);
                    uIElement.instance = null;
                }
            }
        }
    }
}
