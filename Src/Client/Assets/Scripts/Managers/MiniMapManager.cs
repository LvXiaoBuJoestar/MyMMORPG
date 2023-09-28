using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MiniMapManager : Singleton<MiniMapManager>
    {
        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.currentCharacter == null)
                    return null;
                return User.Instance.currentCharacter.transform;
            }
        }

        public Sprite LoadCurrentMiniMapImage()
        {
            return Resloader.Load<Sprite>("UI/MiniMap/" + User.Instance.currentMapData.MiniMap);
        }
    }
}
