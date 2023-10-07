using Common.Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Teleporter : MonoBehaviour
{
    public int id;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1f, EventType.Repaint);
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        PlayerInputController controller = other.GetComponent<PlayerInputController>();
        if (controller != null && controller.isActiveAndEnabled)
        {
            TeleporterDefine td = DataManager.Instance.Teleporters[id];
            if (td == null)
            {
                Debug.LogErrorFormat("TeleporterObject:Character:[{0}] Enter Teleporter:[{1}], But TeleporterDefine Not Existed.", controller.character.Name, id);
                return;
            }
            Debug.LogFormat("TeleporterObject:Character:[{0}] Enter Teleporter:[{1},{2}]", controller.character.Name, td.ID, td.Name);
            if(td.LinkTo > 0)
            {
                if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
                    MapService.Instance.SendMapTeleport(id);
                else
                    Debug.LogErrorFormat("Teleport: ID: {0} LinkId {1} error", td.ID, td.LinkTo);
            }
        }
    }
}
