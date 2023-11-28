using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour
{
    [SerializeField] Text title;
    [SerializeField] Text description;
    [SerializeField] Text targetText;
    [SerializeField] Text rewardText;

    [SerializeField] Button navButton;
    private int npc = 0;

    internal void SetQuestInfo(Quest quest)
    {
        title.text = string.Format("[{0}] {1}", quest.Define.Type, quest.Define.Name);
        if (quest.Info == null)
            description.text = quest.Define.Dialog;
        else
        {
            if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
                description.text = quest.Define.DialogFinish;
        }

        rewardText.text = string.Format("½ð±Ò£º{0}\t\t\t¾­Ñé£º{1}", quest.Define.RewardGold, quest.Define.RewardExp);

        if (quest.Info == null)
            this.npc = quest.Define.AcceptNPC;
        else if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            this.npc = quest.Define.SubmitNPC;
        if (navButton != null)
        {
            this.navButton.gameObject.SetActive(this.npc > 0);
            this.navButton.onClick.RemoveAllListeners();
            this.navButton.onClick.AddListener(OnClickNav);
        }
    }   

    void OnClickNav()
    {
        Vector3 pos = NpcManager.Instance.GetNpcPosition(this.npc);
        User.Instance.currentCharacter.StartNav(pos);
        UIManager.Instance.Close(typeof(UIQuestSystem));
    }
}
