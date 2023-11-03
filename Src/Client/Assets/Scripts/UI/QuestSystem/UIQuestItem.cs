using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem
{
    [SerializeField] Text title;
    [SerializeField] Image background;
    private Color normalColor;
    private Color selectedColor;

    public Quest quest;

    private void Awake()
    {
        if (background == null) return;
        normalColor = background.color;
        selectedColor = background.color;
        selectedColor.a = 1f;
    }

    public override void onSelected(bool selected)
    {
        background.color = selected ? selectedColor : normalColor;
    }

    public void SetQuestInfo(Quest quest)
    {
        this.quest = quest;
        title.text = quest.Define.Name;
    }
}
