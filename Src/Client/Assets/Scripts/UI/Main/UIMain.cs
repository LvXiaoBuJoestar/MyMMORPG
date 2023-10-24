using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{
    [SerializeField] Text avaterName;
    [SerializeField] Text avatarLevel;

    protected override void OnStart()
    {
        UpdateAvatar();
    }

    private void UpdateAvatar()
    {
        avaterName.text = User.Instance.CurrentCharacter.Name;
        avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }

    public void BackToCharacterSelect()
    {
        SceneManager.Instance.LoadScene("CharacterSelect");
        Services.UserService.Instance.SendGameLeave();
    }

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }
}
