using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [SerializeField] InputField accountInputField;
    [SerializeField] InputField passwordInputField;

    [SerializeField] Button loginButton;

    private void Start()
    {
        loginButton.onClick.AddListener(Login);
        UserService.Instance.OnLogin = OnLogin;
    }

    void Login()
    {
        if (string.IsNullOrEmpty(accountInputField.text))
        {
            MessageBox.Show("�������˺�");
            return;
        }
        if (string.IsNullOrEmpty(passwordInputField.text))
        {
            MessageBox.Show("����������");
            return;
        }

        SoundManager.Instance.PlaySfx(SoundDefine.SFX_UI_Click);
        UserService.Instance.SendLogin(accountInputField.text, passwordInputField.text);
    }

    void OnLogin(Result result, string message)
    {
        if (result == Result.Success)
        {
            SceneManager.Instance.LoadScene("CharacterSelect");
            SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
        }
        else
        {
            MessageBox.Show(message, "����", MessageBoxType.Error);
        }
    }
}
