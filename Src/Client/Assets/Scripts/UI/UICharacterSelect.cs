using Models;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelect : MonoBehaviour
{
    [Header("Character Creat")]
    [SerializeField] GameObject backButton;
    [SerializeField] Button[] switchButtons;
    [SerializeField] Image titleImage;
    [SerializeField] Sprite[] titleSprites;
    [SerializeField] Text description;
    [SerializeField] Sprite[] originalButtonSprites;
    [SerializeField] Sprite[] selectedButtonSprites;

    [SerializeField] InputField nameInputField;
    [SerializeField] Button enterGameButton;

    [Header("Panel")]
    [SerializeField] GameObject characterSelectPanel;
    [SerializeField] GameObject characterCreatPanel;

    [Header("Character Select")]
    [SerializeField] Transform characterItemContent;
    [SerializeField] GameObject characterItem;
    [SerializeField] Button creatNewCharButton;
    [SerializeField] Button enterGameButton1;

    List<Image> characterItemImages;

    CharacterClass selectedCharacterClass;
    int selectedCharacterIndex = 0;

    private void Start()
    {
        DataManager.Instance.LoadData();
        UserService.Instance.OnCreateCharacter += this.OnCharacterCreate;

        Debug.Log(User.Instance.Info.Player.Characters.Count);

        backButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            SetTwoPanelActive(false);
            backButton.SetActive(false);
        });
        backButton.SetActive(false);

        if (User.Instance.Info.Player.Characters.Count > 0)
        {
            SetTwoPanelActive(false);
            RefreshCharacterList();
        }
        else
        {
            SetTwoPanelActive(true);
        }

        switchButtons[0].onClick.AddListener(() => { SwitchCharacter(0); });
        switchButtons[1].onClick.AddListener(() => { SwitchCharacter(1); });
        switchButtons[2].onClick.AddListener(() => { SwitchCharacter(2); });

        enterGameButton.onClick.AddListener(CreatCharacter);
        enterGameButton1.onClick.AddListener(() =>
        {
            UserService.Instance.SendGameEnter(selectedCharacterIndex);
        });

        creatNewCharButton.onClick.AddListener(() =>
        {
            SetTwoPanelActive(true);
            backButton.SetActive(true);
        });
    }

    void SetTwoPanelActive(bool isCreatePanel)
    {
        characterCreatPanel.SetActive(isCreatePanel);
        characterSelectPanel.SetActive(!isCreatePanel);
    }

    void SwitchCharacter(int index)
    {
        selectedCharacterClass = (CharacterClass)(index + 1);

        titleImage.sprite = titleSprites[index];
        description.text = DataManager.Instance.Characters[index + 1].Description;

        for (int i = 0; i < 3; i++)
        {
            switchButtons[i].GetComponent<Image>().sprite = originalButtonSprites[i];
            switchButtons[i].gameObject.transform.localScale = Vector3.one;
        }

        CharacterView.Instance.RefreshCharacterView(index);

        switchButtons[index].GetComponent<Image>().sprite = selectedButtonSprites[index];
        switchButtons[index].gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    void CreatCharacter()
    {
        if (string.IsNullOrEmpty(nameInputField.text))
        {
            MessageBox.Show("请为自己取一个响彻大陆的名字吧");
            return;
        }

        UserService.Instance.SendCharacterCreate(nameInputField.text, this.selectedCharacterClass);

        selectedCharacterIndex = User.Instance.Info.Player.Characters.Count;
        UserService.Instance.SendGameEnter(selectedCharacterIndex);
    }

    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {

        }
        else
        {
            MessageBox.Show(message, "错误", MessageBoxType.Error);
        }
    }

    void RefreshCharacterList()
    {
        Color color = Color.white;
        color.a = 0.4f;
        characterItemImages = new List<Image>();

        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            GameObject go = Instantiate(characterItem, characterItemContent);
            UICharacterInfo uICharacterInfo = go.GetComponent<UICharacterInfo>();
            uICharacterInfo.info = User.Instance.Info.Player.Characters[i];

            int index = i;

            characterItemImages.Add(go.GetComponent<Image>());
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach(Image image in characterItemImages)
                {
                    image.color = color;
                }
                go.GetComponent<Image>().color = Color.white;
                CharacterView.Instance.RefreshCharacterView((int)uICharacterInfo.info.Class - 1);

                selectedCharacterIndex = index;
            });
        }
    }
}
