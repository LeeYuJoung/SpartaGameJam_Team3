using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Team.manager
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        [SerializeField] kmu.AntContoller[] antController;

        public bool isGameStart = false;

        [SerializeField] private Button[] stageButtons; // �������� ��ư
        [SerializeField] private Sprite[] stageLockedSprites; // �������� ��� �̹���
        [SerializeField] private Sprite[] stageSprites; // �������� ��� ���� �̹���

        [SerializeField] private Button rightButton;
        [SerializeField] private Button leftButton;

        private int currentLevelIndex = 0; // ���� �������� ���� �ε���
        private int currentPage = 0;

        [SerializeField] private GameObject[] chapter;

        public GameObject gameOverPanel;

        public static GameManager Instance { get { return instance; } }

        public Goal[] goals;

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
            foreach (Goal goal in goals)
            {
                if (goal.antCount > 0)
                {
                    return;
                }
            }

            gameOverPanel.SetActive(true);

            currentLevelIndex++;
            PlayerPrefs.SetInt("UnlockedLevel", currentLevelIndex);
            PlayerPrefs.Save();

        }

        public void GameOver()
        {
            gameOverPanel.SetActive(true);
        }

        public void OnclickMain()
        {
            SceneManager.LoadScene("Main");
        }

        public void OnClickRetry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnClickExit()
        {
#if UNITY_WEBGL
            Application.OpenURL("about:blank"); 
#elif UNITY_EDITOR
            EditorApplication.isPlaying = false; 
#else
            Application.Quit();
#endif
        }

    }


}
