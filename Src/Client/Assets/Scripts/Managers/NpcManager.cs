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

        public NpcDefine GetNpcDefine(int npcId)
        {
            return DataManager.Instance.Npcs[npcId];
        }

        public void RegisterNpcEvent(NpcFunction npcFunction, NpcActionHandler npcActionHandler)
        {
            if(actionHandlers.ContainsKey(npcFunction))
                actionHandlers[npcFunction] = npcActionHandler;
            else
                actionHandlers[npcFunction] += npcActionHandler;
        }

        public bool Interactive(NpcDefine npcDefine)
        {
            if(npcDefine.Type == NpcType.Task)
                return DoTaskInteractive(npcDefine);
            else if(npcDefine.Type == NpcType.Functional)
                return DoFunctionInteractive(npcDefine);
            return false;

        }

        bool DoTaskInteractive(NpcDefine npcDefine)
        {
            MessageBox.Show("点击了NPC：" + npcDefine.Name, "NPC对话");
            return true;
        }

        bool DoFunctionInteractive(NpcDefine npcDefine)
        {
            if (actionHandlers.ContainsKey(npcDefine.Function))
                return false;
            return actionHandlers[npcDefine.Function](npcDefine);
        }
    }
}
