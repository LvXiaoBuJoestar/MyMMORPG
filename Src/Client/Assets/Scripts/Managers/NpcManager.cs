using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class NpcManager : Singleton<NpcManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npcDefine);

        Dictionary<NpcFunction, NpcActionHandler> actionHandlers = new Dictionary<NpcFunction, NpcActionHandler>();
        Dictionary<int, Vector3> npcPositions = new Dictionary<int, Vector3>();

        public NpcDefine GetNpcDefine(int npcId)
        {
            return DataManager.Instance.Npcs[npcId];
        }

        public void RegisterNpcEvent(NpcFunction npcFunction, NpcActionHandler npcActionHandler)
        {
            if(!actionHandlers.ContainsKey(npcFunction))
                actionHandlers[npcFunction] = npcActionHandler;
            else
                actionHandlers[npcFunction] += npcActionHandler;
        }

        public bool Interactive(NpcDefine npcDefine)
        {
            if (DoTaskInteractive(npcDefine))
                return true;
            else if (npcDefine.Type == NpcType.Functional)
                return DoFunctionInteractive(npcDefine);
            return false;

        }

        bool DoTaskInteractive(NpcDefine npcDefine)
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npcDefine.ID);
            if(status == NpcQuestStatus.None) return false;
            return QuestManager.Instance.OpenNpcQuest(npcDefine.ID);
        }

        bool DoFunctionInteractive(NpcDefine npcDefine)
        {
            if (!actionHandlers.ContainsKey(npcDefine.Function))
                return false;
            return actionHandlers[npcDefine.Function](npcDefine);
        }

        internal void UpdateNpcPosition(int npc, Vector3 position)
        {
            this.npcPositions[npc] = position;
        }
        internal Vector3 GetNpcPosition(int npc)
        {
            return this.npcPositions[npc];
        }
    }
}
