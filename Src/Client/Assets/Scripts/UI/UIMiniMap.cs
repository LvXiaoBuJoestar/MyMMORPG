using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour
{
    [SerializeField] BoxCollider cityBox;

    [SerializeField] Image minimapImage;
    [SerializeField] Image arrowImage;
    [SerializeField] Text mapText;

    Transform playerTransform;

    float realWidth;
    float realHeight;
    float minX;
    float minZ;

    private void Start()
    {
        RefreshMapData();
    }

    private void Update()
    {
        float relativeX = playerTransform.position.x - minX;
        float relativeY = playerTransform.position.z - minZ;

        minimapImage.rectTransform.pivot = new Vector2(relativeX / realWidth, relativeY / realHeight);
        minimapImage.rectTransform.localPosition = Vector2.zero;

        arrowImage.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
    }

    void RefreshMapData()
    {
        mapText.text = User.Instance.currentMapData.Name;
        minimapImage.overrideSprite = MiniMapManager.Instance.LoadCurrentMiniMapImage();

        minimapImage.SetNativeSize();
        minimapImage.transform.localPosition = Vector3.zero;

        playerTransform = User.Instance.currentCharacter.transform;

        realWidth = cityBox.bounds.size.x;
        realHeight = cityBox.bounds.size.z;
        minX = cityBox.bounds.min.x;
        minZ = cityBox.bounds.min.z;
    }
}
