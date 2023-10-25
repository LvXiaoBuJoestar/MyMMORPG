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

    private void Start()
    {
        animator = GetComponent<Animator>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        originalColor = skinnedMeshRenderer.material.color;

        npcDefine = NpcManager.Instance.GetNpcDefine(npcId);

        StartCoroutine(nameof(Actions));
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
        Debug.LogWarning(User.Instance.currentCharacter.transform.position);
        Debug.LogWarning(transform.position);
        while (Vector3.Angle(transform.forward, faceTo) > 5f)
        {
            transform.forward = Vector3.Lerp(transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    IEnumerator DoInteractive()
    {
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
