using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestStatus : MonoBehaviour
{
    [SerializeField] Image[] statusImage;
    NpcQuestStatus questStatus;

    internal void SetQuestStatus(NpcQuestStatus status)
    {
        questStatus = status;
        for(int i = 0; i < 4; i++)
        {
            if (statusImage[i] != null)
                statusImage[i].gameObject.SetActive(i == (int)status);
        }
    }
}
