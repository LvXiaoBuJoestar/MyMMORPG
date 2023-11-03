using Common.Data;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public class Quest
    {
        public QuestDefine Define;
        public NQuestInfo Info;

        public Quest()
        {

        }

        public Quest(QuestDefine define)
        {
            Define = define;
            Info = null;
        }

        public Quest(NQuestInfo info)
        {
            Info = info;
            Define = DataManager.Instance.Quests[info.QuestId];
        }
    }
}
