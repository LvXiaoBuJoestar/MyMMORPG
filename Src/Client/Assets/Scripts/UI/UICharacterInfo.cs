using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterInfo : MonoBehaviour
{
    public SkillBridge.Message.NCharacterInfo info;

    [SerializeField] Text characterClassText;
    [SerializeField] Text characterNameText;

    private void Start()
    {
        if (info != null)
        {
            characterClassText.text = info.Class.ToString();
            characterNameText.text = info.Name;
        }
    }
}
