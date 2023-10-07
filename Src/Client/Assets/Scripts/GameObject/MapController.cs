using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] BoxCollider boxCollider;

    void Start()
    {
        MiniMapManager.Instance.UpdateMiniMap(boxCollider);
    }
}
