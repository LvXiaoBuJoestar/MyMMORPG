using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MiniMapManager : Singleton<MiniMapManager>
    {
        public Sprite LoadCurrentMiniMapImage()
        {
            return Resloader.Load<Sprite>("UI/MiniMap/" + User.Instance.currentMapData.MiniMap);
        }
    }
}
