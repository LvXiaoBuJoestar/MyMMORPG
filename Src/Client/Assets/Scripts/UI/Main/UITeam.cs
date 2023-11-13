using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeam : MonoBehaviour
{
    [SerializeField] Text teamTitle;
    [SerializeField] UITeamItem[] Members;
    [SerializeField] ListView listView;

    private void OnEnable()
    {
        UpdateTeamUI();
    }

    private void Start()
    {
        if(User.Instance.TeamInfo == null)
        {
            gameObject.SetActive(false);
            return;
        }
        foreach(var item in Members)
        {
            listView.AddItem(item);
        }
    }

    public void ShowTeam(bool show)
    {
        gameObject.SetActive(show);
        if (show)
            UpdateTeamUI();
    }

    private void UpdateTeamUI()
    {
        if (User.Instance.TeamInfo == null) return;
        teamTitle.text = string.Format("�ҵĶ���({0}/5)", User.Instance.TeamInfo.Members.Count);

        for(int i = 0; i < 5; i++)
        {
            if(i < User.Instance.TeamInfo.Members.Count)
            {
                Members[i].SetMemberInfo(i, User.Instance.TeamInfo.Members[i], User.Instance.TeamInfo.Members[i].Id == User.Instance.TeamInfo.Leader);
                Members[i].gameObject.SetActive(true);
            }
            else
                Members[i].gameObject.SetActive(false);
        }
    }

    public void OnClickLeave()
    {
        MessageBox.Show("ȷ��Ҫ�뿪������", "�뿪����", MessageBoxType.Confirm, "ȷ��", "ȡ��").OnYes = () =>
        {
            TeamService.Instance.SendTeamLeaveRequest();
        };
    }
}
