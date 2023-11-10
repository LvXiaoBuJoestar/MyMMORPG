using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : ListView.ListViewItem
{
    [SerializeField] Text name;
    [SerializeField] Text level;
    [SerializeField] Text @class;
    [SerializeField] Text status;

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

    public NFriendInfo info;
    public void SetFriendInfo(NFriendInfo friendInfo)
    {
        info = friendInfo;
        if (name != null) name.text = info.friendInfo.Name;
        if (level != null) level.text = info.friendInfo.Level.ToString();
        if (@class != null) @class.text = info.friendInfo.Class.ToString();
        if (status != null) status.text = info.Status == 1 ? "‘⁄œﬂ" : "¿Îœﬂ";
    }
}
