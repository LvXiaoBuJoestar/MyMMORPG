using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TabView : MonoBehaviour
{
    [SerializeField] TabButton[] tabs;
    [SerializeField] GameObject[] pages;

    private int lastIndex = -1;

    IEnumerator Start()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].index = i;
            tabs[i].tabView = this;
        }

        yield return new WaitForEndOfFrame();
        SelectTab(0);
    }

    public void SelectTab(int index)
    {
        if (index != lastIndex)
        {
            tabs[index].Select(true);
            pages[index].SetActive(true);

            if (lastIndex > -1)
            {
                tabs[lastIndex].Select(false);
                pages[lastIndex].SetActive(false);
            }

            lastIndex = index;
        }
    }
}
