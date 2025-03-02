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

        [SerializeField] private Button[] stageButtons; // 스테이지 버튼
        [SerializeField] private Sprite[] stageLockedSprites; // 스테이지 잠김 이미지
        [SerializeField] private Sprite[] stageSprites; // 스테이지 잠김 해제 이미지
        [SerializeField] private Sprite stageClearSpretes;

        [SerializeField] private Button rightButton;
        [SerializeField] private Button leftButton;

        public int currentLevelIndex = 0; // 현재 스테이지 레벨 인덱스
        private int currentPage = 0;

        [SerializeField] private GameObject[] chapter;

        public GameObject gameOverPanel;

        public static GameManager Instance { get { return instance; } }

        public Goal[] goals;

        public GameObject clearTitle;
        public GameObject gameOverTitle;

        public AudioSource audioSource;

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

        // 스테이지 레벨 버튼 업데이트
        private void UpdateStageButtons()
        {
            for (int i = 0;  i < stageButtons.Length; i++)
            {
                if (i <= currentLevelIndex)
                {
                    stageButtons[i].interactable = true;
                    stageButtons[i].GetComponent<Image>().sprite = stageSprites[i];
                    stageButtons[i].onClick.AddListener(() => AudioManager.Instance.PlaySFX(audioSource, SFXType.StageClick));
                }
                else
                {
                    stageButtons[i].interactable = false;
                    stageButtons[i].GetComponent<Image>().sprite = stageLockedSprites[i];
                    stageButtons[i].onClick.AddListener(() => AudioManager.Instance.PlaySFX(audioSource, SFXType.StageLock));
                }
            }

            for (int i = 0; i < Mathf.Min(currentLevelIndex, stageButtons.Length); i++)
            {
                stageButtons[i].GetComponent<Image>().sprite = stageClearSpretes;
            }

            for (int i = 2; i < stageButtons.Length; i++)
            {
                stageButtons[i].interactable = false;
                stageButtons[i].GetComponent<Image>().sprite = stageLockedSprites[i];
            }

        }

        // 버튼 클릭으로 씬 전환
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

        // 게임 승리시 호출
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
            clearTitle.SetActive(true);
            AudioManager.Instance.PlaySFX(audioSource, SFXType.GameClear);

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (currentLevelIndex < currentSceneIndex)
            {
                currentLevelIndex++;
                PlayerPrefs.SetInt("UnlockedLevel", currentLevelIndex);
                PlayerPrefs.Save();
            }

        }

        public void GameOver()
        {
            gameOverTitle.SetActive(true);
            gameOverPanel.SetActive(true);
            AudioManager.Instance.PlaySFX(audioSource, SFXType.GameOver);

        }

        public void OnclickMain()
        {
            SceneManager.LoadScene("Main");
        }

        public void OnClickRetry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            AudioManager.Instance.PlaySFX(audioSource, SFXType.GameRestart);

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
