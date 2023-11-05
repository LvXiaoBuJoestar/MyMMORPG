using Managers;
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

    bool showAvailableList = false;
    void InitQuestItems()
    {
        foreach (var kv in QuestManager.Instance.allQuests)
        {
            if (showAvailableList)
            {
                if (kv.Value.Info != null) continue;
            }
            else
            {
                if (kv.Value.Info == null) continue;
            }

            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == Common.Data.QuestType.Main ? mainListView.transform : BranchListView.transform);
            UIQuestItem questItem = go.GetComponent<UIQuestItem>();
            questItem.SetQuestInfo(kv.Value);

            if(kv.Value.Define.Type == Common.Data.QuestType.Main)
                mainListView.AddItem(questItem);
            else
                BranchListView.AddItem(questItem);
        }
    }
}
