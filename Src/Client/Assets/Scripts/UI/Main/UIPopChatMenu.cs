using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopChatMenu : UIWindow, IDeselectHandler
{
    public int targetId;
    public string targetName;

    private void OnEnable()
    {
        GetComponent<Selectable>().Select();
        this.Root.transform.position = Input.mousePosition + new Vector3(80, 0, 0);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        var pointerEventData = eventData as PointerEventData;
        if (pointerEventData.hovered.Contains(this.gameObject))
            return;
        this.Close();
    }

    public void OnClickChat()
    {
        ChatManager.Instance.StartPrivateChat(targetId, targetName);
        this.Close(WindowResult.no);
    }

    public void OnClickAddFriend()
    {
        this.Close(WindowResult.no);
    }

    public void OnClickTeamInvite()
    {
        this.Close(WindowResult.no);
    }
}
