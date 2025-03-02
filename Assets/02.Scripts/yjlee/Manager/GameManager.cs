using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Team.manager
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        [SerializeField] private Button[] stageButtons; // �������� ��ư
        [SerializeField] private Sprite[] stageLockedSprites; // �������� ��� �̹���
        [SerializeField] private Sprite[] stageSprites; // �������� ��� ���� �̹���

        [SerializeField] private Button rightButton;
        [SerializeField] private Button leftButton;

        private int currentLevelIndex = 0; // ���� �������� ���� �ε���
        private int totalChapterPage = 2;
        private int currentPage = 0;

        [SerializeField] private GameObject[] chapter;

        public static GameManager Instance { get { return instance; } }

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(instance);
            }
            else
            {
                instance = this;
            }

            Application.targetFrameRate = 65;

            currentLevelIndex = PlayerPrefs.GetInt("UnlockedLevel" , 0);
            UpdateStageButtons();
        }

        // �������� ���� ��ư ������Ʈ
        private void UpdateStageButtons()
        {
            for (int i = 0;  i < stageButtons.Length; i++)
            {
                if (i <= currentLevelIndex)
                {
                    stageButtons[i].interactable = true;
                    stageButtons[i].GetComponent<Image>().sprite = stageSprites[i];
                }
                else
                {
                    stageButtons[i].interactable = false;
                    stageButtons[i].GetComponent<Image>().sprite = stageLockedSprites[i];
                }
            }
        }

        // ��ư Ŭ������ �� ��ȯ
        public void OnClickSceneChange(string sceneName)
        {
            switch (sceneName)
            {
                case "Main":
                    SceneManager.LoadScene("Main");
                    break;
                case "Stage1":
                    SceneManager.LoadScene("Stage1");
                    break;
                case "Stage2":
                    SceneManager.LoadScene("Stage2");
                    break;
                case "Stage3":
                    SceneManager.LoadScene("Stage3");
                    break;
                case "Stage4":
                    SceneManager.LoadScene("Stage4");
                    break;
                case "Stage5":
                    SceneManager.LoadScene("Stage5");
                    break;
            }
        }

        public void OnclickChapterMove(bool isRight)
        {
            chapter[currentPage].SetActive(false);

            if (isRight)
            {
                currentPage++;
            }
            else
            {
                currentPage--;
            }

            currentPage = Mathf.Clamp(currentPage, 0, chapter.Length - 1);

            chapter[currentPage].SetActive(true);

            
        }

        // ���� �¸��� ȣ��
        public void GameClear()
        {
            if (currentLevelIndex < stageButtons.Length - 1)
            {
                currentLevelIndex++;
                PlayerPrefs.SetInt("UnlockedLevel", currentLevelIndex);
                PlayerPrefs.Save();
                UpdateStageButtons();
            }
        }
    }

    
}
