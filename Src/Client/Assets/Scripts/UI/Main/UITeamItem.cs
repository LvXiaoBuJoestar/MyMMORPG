using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeamItem : ListView.ListViewItem
{
    [SerializeField] Text name;
    [SerializeField] Text level;
    [SerializeField] Image classIcon;
    [SerializeField] Image leaderIcon;

    [SerializeField] Image background;
    private Color normalColor;
    private Color selectedColor;

    private void Awake()
    {
        normalColor = background.color;
        selectedColor = background.color;
        selectedColor.a = 1f;
    }

    public override void onSelected(bool selected)
    {
        background.color = selected ? selectedColor : normalColor;
    }

    [HideInInspector] public int index;
    [HideInInspector] public NCharacterInfo info;
    public void SetMemberInfo(int index, NCharacterInfo info, bool isLeader)
    {
        this.index = index;
        this.info = info;
        if (name != null) name.text = info.Name;
        if (level != null) level.text = info.Level.ToString();
        if (classIcon != null) classIcon.sprite = SpriteManager.Instance.classIcons[(int)this.info.Class - 1];
        if (leaderIcon != null) leaderIcon.gameObject.SetActive(isLeader);
    }
}
