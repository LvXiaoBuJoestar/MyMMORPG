using Common.Data;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    [SerializeField] int npcId;

    Animator animator;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Color originalColor;

    NpcDefine npcDefine;

    NpcQuestStatus questStatus;

    private void Start()
    {
        animator = GetComponent<Animator>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        originalColor = skinnedMeshRenderer.material.color;

        npcDefine = NpcManager.Instance.GetNpcDefine(npcId);
        NpcManager.Instance.UpdateNpcPosition(npcId, transform.position);

        StartCoroutine(nameof(Actions));
        RefreshNpcStatus();
        QuestManager.Instance.onQuestStatusChanged += OnQuestStatusChanged;
    }

    void OnQuestStatusChanged(Quest quest)
    {
        RefreshNpcStatus();
    }

    private void RefreshNpcStatus()
    {
        questStatus = QuestManager.Instance.GetQuestStatusByNpc(npcId);
        UIWorldElementManager.Instance.AddNpcQuestStatus(transform, questStatus);
    }

    private void OnDestroy()
    {
        QuestManager.Instance.onQuestStatusChanged -= OnQuestStatusChanged;
        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(transform);

    }

    private void OnMouseEnter()
    {
        HighLight(true);
    }
    private void OnMouseExit()
    {
        HighLight(false);
    }
    private void OnMouseDown()
    {
        if (Vector3.Distance(this.transform.position, User.Instance.currentCharacter.transform.position) > 2f)
            User.Instance.currentCharacter.StartNav(transform.position);
        StartCoroutine(nameof(DoInteractive));
    }

    IEnumerator Actions()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 10f));
            Relax();
        }
    }

    void Relax()
    {
        animator.SetTrigger("Relax");
    }

    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.currentCharacter.transform.position - transform.position).normalized;
        while (Vector3.Angle(transform.forward, faceTo) > 5f)
        {
            transform.forward = Vector3.Lerp(transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    IEnumerator DoInteractive()
    {
        while (User.Instance.currentCharacter.autoNav)
        {
            yield return null;
        }
        yield return StartCoroutine(nameof(FaceToPlayer));
        animator.SetTrigger("Talk");
        NpcManager.Instance.Interactive(npcDefine);
    }

    void HighLight(bool height)
    {
        if (height)
            skinnedMeshRenderer.material.color = Color.white;
        else
            skinnedMeshRenderer.material.color = originalColor;
    }
}
