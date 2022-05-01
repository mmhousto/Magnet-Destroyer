using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using StarterAssets;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager _instance;

        public StarterAssetsInputs inputs;
        public GameObject menuCam, gameCam, magnet, gameOverScreen, tryAgainButton, pauseMenu, resumeGameButton;
        public bool gameStarted = false;
        public bool gameOver = false;

        private void Awake()
        {
            _instance = this;
            inputs.DisableInput();
        }

        // Update is called once per frame
        void Update()
        {
            if(gameOver == true)
            {
                pauseMenu.SetActive(false);
                gameOverScreen.SetActive(true);
            }
        }

        // Start is called before the first frame update
        public void GameOver()
        {
            gameOverScreen.SetActive(true);
            LootLockerManager.Instance.SubmitScore();
            gameOver = true;
            Time.timeScale = 0;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(tryAgainButton);
        }

        public void StartGame()
        {
            menuCam.SetActive(false);
            magnet.SetActive(true);
            gameCam.SetActive(true);
            SpawnManager._sharedInstance.SpawnFirstRoom();
            gameStarted = true;
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            inputs.enabled = false;
            pauseMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(resumeGameButton);
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            inputs.enabled = true;
            inputs.pause = false;
            pauseMenu.SetActive(false);
        }

        public void RestartGame()
        {
            gameStarted = false;
            gameOver = false;
            inputs.cursorLocked = true;
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        
    }
}
