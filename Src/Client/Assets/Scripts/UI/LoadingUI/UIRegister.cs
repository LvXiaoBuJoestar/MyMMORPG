using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;

public class UIRegister : MonoBehaviour
{
    [SerializeField] InputField accountInputField;
    [SerializeField] InputField passwordInputField;
    [SerializeField] InputField passwordConfirmField;

    [SerializeField] Button enterGameButton;

    private void Start()
    {
        enterGameButton.onClick.AddListener(Register);
        UserService.Instance.OnRegister = OnRegister;
    }

    void Register()
    {
        if (string.IsNullOrEmpty(accountInputField.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(passwordInputField.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        if (string.IsNullOrEmpty(passwordConfirmField.text))
        {
            MessageBox.Show("请再次确认密码");
            return;
        }
        if (passwordInputField.text != passwordConfirmField.text)
        {
            MessageBox.Show("两次输入的密码不一致");
            return;
        }

        SoundManager.Instance.PlaySfx(SoundDefine.SFX_UI_Click);
        UserService.Instance.SendRegister(accountInputField.text, passwordConfirmField.text);
    }

    void OnRegister(Result result, string message)
    {
        if (result == Result.Success)
        {
            UserService.Instance.SendLogin(accountInputField.text, passwordConfirmField.text);
        }
        else
        {
            MessageBox.Show(message, "错误", MessageBoxType.Error);
        }
    }
}
