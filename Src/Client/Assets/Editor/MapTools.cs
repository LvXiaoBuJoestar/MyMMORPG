using Common.Data;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTools
{
    [MenuItem("MapTools/Export Teleporters")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();

        Scene currentScene = EditorSceneManager.GetActiveScene();
        string currentSceneName = currentScene.name;
        if (currentScene.isDirty)
        {
            EditorUtility.DisplayDialog("��ʾ", "���ȱ��泡���ٽ��иò���", "ȷ��");
            return;
        }

        List<Teleporter> teleporters = new List<Teleporter>();

        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFilePath = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!File.Exists(sceneFilePath))
            {
                Debug.LogWarningFormat("Scene {0} not existed", sceneFilePath);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFilePath, OpenSceneMode.Single);

            Teleporter[] currentSceneTeles = GameObject.FindObjectsOfType<Teleporter>();
            foreach (var currentSceneTele in currentSceneTeles)
            {
                if (!DataManager.Instance.Teleporters.ContainsKey(currentSceneTele.id))
                {
                    EditorUtility.DisplayDialog("����", string.Format("��ͼ��{0}�������õ�Teleporter��{1}������", map.Value.Resource, currentSceneTele.id), "ȷ��");
                    return;
                }

                TeleporterDefine teleporterDefine = DataManager.Instance.Teleporters[currentSceneTele.id];
                if (teleporterDefine.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("����", string.Format("��ͼ��{0}�������õ�Teleporter��{1}����", map.Value.Resource, currentSceneTele.id), "ȷ��");
                    return;
                }

                teleporterDefine.Position = GameObjectTool.WorldToLogicN(currentSceneTele.transform.position);
                teleporterDefine.Direction = GameObjectTool.WorldToLogicN(currentSceneTele.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentSceneName + ".unity");
        EditorUtility.DisplayDialog("��ʾ", "���͵㵼���ɹ�", "ȷ��");
    }
}
