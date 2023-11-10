using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInputBox : MonoBehaviour
{
    [SerializeField] Text title;
    [SerializeField] Text message;
    [SerializeField] Text tips;
    [SerializeField] InputField inputField;
    [SerializeField] Button buttonYes;
    [SerializeField] Button buttonNo;

    [SerializeField] Text buttonYesTitle;
    [SerializeField] Text buttonNoTitle;

    [SerializeField] string emptyTips;

    public delegate bool SubmitHandler(string inputText, out string tips);
    public event SubmitHandler OnSubmit;
    public UnityAction OnCancel;

    public void Init(string title, string message, string btnOK = "", string btnCancel = "", string emptyTips = "")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message;
        this.tips.text = null;
        this.OnSubmit = null;
        this.emptyTips = emptyTips;

        if (!string.IsNullOrEmpty(btnOK)) this.buttonYesTitle.text = btnOK;
        if (!string.IsNullOrEmpty(btnCancel)) this.buttonNoTitle.text = btnCancel;

        this.buttonYes.onClick.AddListener(OnClickYes);
        this.buttonNo.onClick.AddListener(OnClickNo);
    }

    void OnClickYes()
    {
        this.tips.text = null;
        if(string.IsNullOrEmpty(inputField.text))
        {
            this.tips.text = this.emptyTips;
            return;
        }
        if (OnSubmit != null)
        {
            string tips;
            if (!OnSubmit(inputField.text, out tips))
            {
                this.tips.text = tips;
                return;
            }
        }
        Destroy(this.gameObject);
    }

    void OnClickNo()
    {
        if (this.OnCancel != null)
            OnCancel();
        Destroy(this.gameObject);
    }
}
