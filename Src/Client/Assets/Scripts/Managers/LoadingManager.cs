using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] GameObject UIWarnTips;
    [SerializeField] GameObject UILoading;
    [SerializeField] GameObject UILogin;

    [SerializeField] Slider progressBar;
    [SerializeField] Text progressText;

    IEnumerator Start()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager Start");

        UIWarnTips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);

        yield return new WaitForSeconds(2f);
        UILoading.SetActive(true);
        yield return new WaitForSeconds(1f);
        UIWarnTips.SetActive(false);

        yield return DataManager.Instance.LoadData();

        UserService.Instance.Init();
        MapService.Instance.Init();
        ShopManager.Instance.Init();
        FriendService.Instance.Init();
        GuildService.Instance.Init();
        TeamService.Instance.Init();
        ChatService.Instance.Init();

        for (float i = 50; i < 100;)
        {
            i += Random.Range(0.1f, 0.5f);
            progressBar.value = i;
            progressText.text = i.ToString("F0") + "%";
            yield return new WaitForEndOfFrame();
        }

        UILoading.SetActive(false);
        UILogin.SetActive(true);

        yield return null;
    }
}
