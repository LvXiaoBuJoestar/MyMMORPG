using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnPoint : MonoBehaviour
{
    Mesh mesh = null;
    public int Id;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position + Vector3.up * transform.localScale.y * 0.5f;
        Gizmos.color = Color.red;
        if(mesh != null )
            Gizmos.DrawWireMesh(mesh, pos, transform.rotation, transform.localScale);
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.ArrowHandleCap(0, pos, transform.rotation, 1f, EventType.Repaint);
        UnityEditor.Handles.Label(pos, "SpawnPoint£º" + Id);
    }
#endif
}
