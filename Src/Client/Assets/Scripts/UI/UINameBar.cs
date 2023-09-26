using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour
{
    [SerializeField] Text avatarName;
    public Character character;

    private void Update()
    {
        RefreshInfo();
    }

    void RefreshInfo()
    {
        if(character != null)
        {
            string name = character.Name + "Lv." + character.Info.Level;

            if(name != avatarName.text )
            {
                avatarName.text = name;
            }
        }
    }
}
