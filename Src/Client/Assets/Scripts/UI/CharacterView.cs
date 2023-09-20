using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoSingleton<CharacterView>
{
    [SerializeField] GameObject[] characterObjects;

    [Range(0, 2)] int lastIndex = 1;

    private void Start()
    {
        RefreshCharacterView(lastIndex);
    }

    public void RefreshCharacterView(int index)
    {
        if (lastIndex != index)
        {
            characterObjects[lastIndex].SetActive(false);
            characterObjects[index].SetActive(true);
            lastIndex = index;
        }
    }
}
