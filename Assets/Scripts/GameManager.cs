using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using StarterAssets;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager _instance;

        public PlayerInput playerInput;
        public StarterAssetsInputs inputs;
        public GameObject menuCam, gameCam, magnet, mainMenuCanvas, gameOverScreen, tryAgainButton, pauseMenu, resumeGameButton;
        public GameObject okayBadButton, errorScreen, mainMenuScreen, signInScreen, destroyButton, hiScoreScreen,
            hud, onScreenControls, signingInScreen;
        public bool gameStarted = false;
        public bool gameOver = false;
        public bool isPaused = false;

        public static bool restarted = false;

        private void Awake()
        {
            _instance = this;
            inputs.EnableMouseInput();
        }

        private void Start()
        {
            SpawnManager._sharedInstance.PoolObjects();
            hiScoreScreen.SetActive(false);
            gameOverScreen.SetActive(false);
            onScreenControls.SetActive(false);
            hud.SetActive(false);
            gameOver = false;
            isPaused = false;
            if(restarted == true && LootLockerManager.Instance.isSignedIn == true)
            {
                
                StartGame();
            }else if(LootLockerManager.Instance.isSignedIn == true)
            {
                LootLockerManager.Instance.SetMemberIDLabel();
                ShowMainMenu();
                
            }
        }

        public void UpdateTopScores()
        {
            LootLockerManager.Instance.ShowTopScores();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowMainMenu()
        {
            signInScreen.SetActive(false);
            signingInScreen.SetActive(false);
            errorScreen.SetActive(false);
            mainMenuScreen.SetActive(true);
            onScreenControls.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(destroyButton);
        }

        public void ShowOfflineContinue()
        {
            okayBadButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(okayBadButton);
        }

        public void ShowErrorScreen()
        {
            errorScreen.SetActive(true);
            signInScreen.SetActive(false);
        }

        // Start is called before the first frame update
        public void GameOver()
        {
            inputs.EnableMouseInput();
            playerInput.enabled = false;
            pauseMenu.SetActive(false);
            gameOverScreen.SetActive(true);
            onScreenControls.SetActive(false);
            LootLockerManager.Instance.SubmitScore();
            gameOver = true;
            isPaused = false;
            Time.timeScale = 0;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(tryAgainButton);
        }

        public void StartGame()
        {
            inputs.DisableInput();
            menuCam.SetActive(false);
            Destroy(mainMenuCanvas);
            magnet.SetActive(true);
            gameCam.SetActive(true);
            hud.SetActive(true);
            onScreenControls.SetActive(true);
            SpawnManager._sharedInstance.SpawnFirstRoom();
            EventSystem.current.SetSelectedGameObject(null);
            gameStarted = true;
            restarted = false;
        }

        public void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0;
            playerInput.enabled = false;
            inputs.EnableMouseInput();
            pauseMenu.SetActive(true);
            onScreenControls.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(resumeGameButton);
        }

        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1;
            playerInput.enabled = true;
            //inputs.DisableInput();
            inputs.pause = false;
            pauseMenu.SetActive(false);
            onScreenControls.SetActive(true);
        }

        public void RestartGame()
        {
            restarted = true;
            gameStarted = true;
            gameOver = false;
            playerInput.enabled = true;
            onScreenControls.SetActive(true);
            inputs.DisableInput();
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        public void ToMainMenu()
        {
            restarted = false;
            gameStarted = false;
            gameOver = false;
            playerInput.enabled = true;
            onScreenControls.SetActive(false);
            inputs.EnableMouseInput();
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        
    }
}
