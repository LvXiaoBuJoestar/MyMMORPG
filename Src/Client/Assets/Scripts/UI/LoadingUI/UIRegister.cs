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
            MessageBox.Show("�������˺�");
            return;
        }
        if (string.IsNullOrEmpty(passwordInputField.text))
        {
            MessageBox.Show("����������");
            return;
        }
        if (string.IsNullOrEmpty(passwordConfirmField.text))
        {
            MessageBox.Show("���ٴ�ȷ������");
            return;
        }
        if (passwordInputField.text != passwordConfirmField.text)
        {
            MessageBox.Show("������������벻һ��");
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
            MessageBox.Show(message, "����", MessageBoxType.Error);
        }
    }
}
