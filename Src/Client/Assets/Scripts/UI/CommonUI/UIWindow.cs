using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public delegate void CloseHandler(UIWindow uIWindowCom, WindowResult windowResult);
    public event CloseHandler OnClose;

    public virtual Type Type { get { return this.GetType(); } }

    public GameObject Root;

    public enum WindowResult
    {
        none = 0,
        yes,
        no
    }

    public void Close(WindowResult windowResult = WindowResult.none)
    {
        SoundManager.Instance.PlaySfx(SoundDefine.SFX_UI_Win_Close);
        UIManager.Instance.Close(Type);
        if(OnClose != null)
            OnClose(this, windowResult);
        OnClose = null;
    }

    public virtual void OnCloseClike()
    {
        Close();
    }

    public virtual void OnYesClike()
    {
        Close(WindowResult.yes);
    }

    public virtual void OnNoClike()
    {
        Close(WindowResult.no);
    }
}
