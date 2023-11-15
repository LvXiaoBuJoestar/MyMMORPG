using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildPopCreate : UIWindow
{
    [SerializeField] InputField inputField_Name;
    [SerializeField] InputField inputField_Notice;

    private void Start()
    {
        GuildService.Instance.OnGuildCreateResult = OnGuildCreated;
    }
    private void OnDestroy()
    {
        GuildService.Instance.OnGuildCreateResult = null;
    }

    public override void OnYesClike()
    {
        if (string.IsNullOrEmpty(inputField_Name.text))
        {
            MessageBox.Show("请输入公会名称", "错误", MessageBoxType.Error); return;
        }
        if(inputField_Name.text.Length < 4 || inputField_Name.text.Length > 10)
        {
            MessageBox.Show("公会名称应在4-10个字符内", "错误", MessageBoxType.Error); return;
        }
        if (string.IsNullOrEmpty(inputField_Notice.text))
        {
            MessageBox.Show("请输入公会宣言", "错误", MessageBoxType.Error); return;
        }
        if (inputField_Notice.text.Length < 4 || inputField_Name.text.Length > 50)
        {
            MessageBox.Show("公会宣言应在4-50个字符内", "错误", MessageBoxType.Error); return;
        }

        GuildService.Instance.SendGuildCreate(inputField_Name.text, inputField_Notice.text);
    }

    void OnGuildCreated(bool result)
    {
        if (result) this.Close(WindowResult.yes);
    }
}
