using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCity : MonoBehaviour
{
    [SerializeField] Text avaterName;
    [SerializeField] Text avatarLevel;

    void Start()
    {
        UpdateAvatar();
    }

    private void UpdateAvatar()
    {
        avaterName.text = User.Instance.CurrentCharacter.Name;
        avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }
}
