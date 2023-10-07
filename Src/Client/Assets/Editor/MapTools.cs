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
            EditorUtility.DisplayDialog("提示", "请先保存场景再进行该操作", "确定");
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
                    EditorUtility.DisplayDialog("错误", string.Format("地图【{0}】中配置的Teleporter：{1}不存在", map.Value.Resource, currentSceneTele.id), "确定");
                    return;
                }

                TeleporterDefine teleporterDefine = DataManager.Instance.Teleporters[currentSceneTele.id];
                if (teleporterDefine.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图【{0}】中配置的Teleporter：{1}错误", map.Value.Resource, currentSceneTele.id), "确定");
                    return;
                }

                teleporterDefine.Position = GameObjectTool.WorldToLogicN(currentSceneTele.transform.position);
                teleporterDefine.Direction = GameObjectTool.WorldToLogicN(currentSceneTele.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentSceneName + ".unity");
        EditorUtility.DisplayDialog("提示", "传送点导出成功", "确定");
    }
}
