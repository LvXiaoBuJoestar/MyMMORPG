using UnityEngine;
using UnityEngine.UI;

public class CheckFrame : MonoBehaviour
{
    Button button;
    GameObject checkSign;

    private void Awake()
    {
        button = GetComponent<Button>();
        checkSign = transform.GetChild(0).gameObject;
        checkSign.SetActive(false);

        button.onClick.AddListener(() => { checkSign.SetActive(!checkSign.activeSelf); });
    }
}