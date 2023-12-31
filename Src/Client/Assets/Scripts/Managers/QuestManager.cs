using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public enum NpcQuestStatus
    {
        None = 0,
        Complete,
        Available,
        Incomplete
    }

    public class QuestManager : Singleton<QuestManager>
    {
        public List<NQuestInfo> questInfos;
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();

        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public UnityAction<Quest> onQuestStatusChanged;

        public void Init(List<NQuestInfo> quests)
        {
            questInfos = quests;
            allQuests.Clear();
            npcQuests.Clear();
            InitQuests();
        }

        private void CheckAvailableQuests()
        {
            foreach (var kv in DataManager.Instance.Quests)
            {
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
                    continue;
                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
                    continue;
                if (allQuests.ContainsKey(kv.Key))
                    continue;

                if (kv.Value.PreQuest > 0)
                {
                    Quest preQuest;
                    if (allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))
                    {
                        if (preQuest.Info == null || preQuest.Info.Status != QuestStatus.Finished)
                            continue;
                    }
                    else
                        continue;
                }

                Quest quest = new Quest(kv.Value);
                allQuests[quest.Define.ID] = quest;
            }
        }

        private void InitQuests()
        {
            foreach(var info in questInfos)
            {
                Quest quest = new Quest(info);
                allQuests[quest.Info.QuestId] = quest;
            }

            CheckAvailableQuests();

            foreach(var kv in allQuests)
            {
                AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        private void AddNpcQuest(int npcId, Quest quest)
        {
            if (!npcQuests.ContainsKey(npcId))
                npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();

            List<Quest> availables;
            List<Quest> completes;
            List<Quest> incompletes;

            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.Available, out availables))
            {
                availables = new List<Quest>();
                npcQuests[npcId][NpcQuestStatus.Available] = availables;
            }
            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out completes))
            {
                completes = new List<Quest>();
                npcQuests[npcId][NpcQuestStatus.Complete] = completes;
            }
            if (!npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomplete, out incompletes))
            {
                incompletes = new List<Quest>();
                npcQuests[npcId][NpcQuestStatus.Incomplete] = incompletes;
            }

            if(quest.Info == null)
            {
                if(npcId == quest.Define.AcceptNPC && !npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
                    npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
            }
            else
            {
                if(npcId == quest.Define.SubmitNPC && quest.Info.Status == QuestStatus.Complated)
                {
                    if (!npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                        npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                }
                if (npcId == quest.Define.SubmitNPC && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest))
                        npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
                }
            }
        }

        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if(npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return NpcQuestStatus.Complete;
                if (status[NpcQuestStatus.Available].Count > 0)
                    return NpcQuestStatus.Available;
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return NpcQuestStatus.Incomplete;
            }
            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                if (status[NpcQuestStatus.Available].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
            }
            return false;
        }

        bool ShowQuestDialog(Quest quest)
        {
            if(quest.Info == null || quest.Info.Status == QuestStatus.Complated)
            {
                UIQuestDialog dialog = UIManager.Instance.Show<UIQuestDialog>();
                dialog.SetQuest(quest);
                dialog.OnClose += OnQuestDialogClose;
                return true;
            }
            if (quest.Info != null || quest.Info.Status == QuestStatus.Complated)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                    MessageBox.Show(quest.Define.DialogIncomplete);
            }
            return true;
        }

        private void OnQuestDialogClose(UIWindow uIWindowCom, UIWindow.WindowResult windowResult)
        {
            UIQuestDialog dialog = uIWindowCom as UIQuestDialog;

            if (windowResult == UIWindow.WindowResult.yes)
            {
                if(dialog.quest.Info == null)
                    QuestService.Instance.SendQuestAccept(dialog.quest);
                else if(dialog.quest.Info.Status == QuestStatus.Complated)
                    QuestService.Instance.SendQuestSubmit(dialog.quest);
            }
            else if (windowResult == UIWindow.WindowResult.no)
                MessageBox.Show(dialog.quest.Define.DialogDeny);
        }

        Quest RefreshQuestStatus(NQuestInfo quest)
        {
            npcQuests.Clear();
            Quest result;
            if (allQuests.ContainsKey(quest.QuestId))
            {
                allQuests[quest.QuestId].Info = quest;
                result = allQuests[quest.QuestId];
            }
            else
            {
                result = new Quest(quest);
                allQuests[quest.QuestId] = result;
            }

            CheckAvailableQuests();

            foreach(var kv in allQuests)
            {
                AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }

            if(onQuestStatusChanged != null)
                onQuestStatusChanged(result);
            return result;
        }

        public void OnQuestAccept(NQuestInfo questInfo)
        {
            Quest quest = RefreshQuestStatus(questInfo);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        internal void OnQuestSubmited(NQuestInfo questInfo)
        {
            Quest quest = RefreshQuestStatus(questInfo);
            MessageBox.Show(quest.Define.DialogFinish);
        }
    }
}
