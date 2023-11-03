using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestSystem : UIWindow
{
    [SerializeField] GameObject itemPrefab;

    [SerializeField] ListView mainListView;
    [SerializeField] ListView BranchListView;
    [SerializeField] UIQuestInfo questInfo;

    private void Start()
    {
        mainListView.onItemSelected += OnQuestSelected;
        BranchListView.onItemSelected += OnQuestSelected;
        RefreshUI();
    }

    private void OnQuestSelected(ListView.ListViewItem item)
    {
        UIQuestItem questItem = item as UIQuestItem;
        questInfo.SetQuestInfo(questItem.quest);
    }

    void RefreshUI()
    {
        ClearAllQuestList();
        InitQuestItems();
    }

    void ClearAllQuestList()
    {
        mainListView.RemoveAll();
        BranchListView.RemoveAll();
    }

    void InitQuestItems()
    {

    }
}
