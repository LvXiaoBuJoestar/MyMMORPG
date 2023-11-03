using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestDialog : UIWindow
{
    [SerializeField] UIQuestInfo questInfo;
    [SerializeField] GameObject openButtons;
    [SerializeField] GameObject submitButtons;

    public Quest quest;

    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();

        if(quest.Info == null)
        {
            openButtons.SetActive(true); submitButtons.SetActive(false);
        }
        else
        {
            if(quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                openButtons.SetActive(false); submitButtons.SetActive(true);
            }
            else
            {
                openButtons.SetActive(true); submitButtons.SetActive(false);
            }
        }
    }

    private void UpdateQuest()
    {
        if (quest != null && questInfo != null)
            questInfo.SetQuestInfo(quest);
    }
}
