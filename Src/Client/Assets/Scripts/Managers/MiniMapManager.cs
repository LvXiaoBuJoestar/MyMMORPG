using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MiniMapManager : Singleton<MiniMapManager>
    {
        public UIMiniMap miniMap;

        private BoxCollider miniMapBondingBox;
        public BoxCollider MiniMapBondingBox
        {
            get { return miniMapBondingBox; }
        }

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

        public void UpdateMiniMap(BoxCollider boxCollider)
        {
            this.miniMapBondingBox = boxCollider;
            if(miniMap != null )
            {
                miniMap.RefreshMapData();
            }
        }
    }
}
