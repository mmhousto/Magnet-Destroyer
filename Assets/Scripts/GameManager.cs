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
            hud, onScreenControls, signingInScreen, continueButton, signInGood;
        public bool gameStarted = false;
        public bool gameOver = false;
        public bool isPaused = false;

        private bool hasContinued = false;

        public static bool restarted = false;

        private void Awake()
        {
            _instance = this;
            inputs.EnableMouseInput();
        }

        private void Start()
        {
            SpawnManager._sharedInstance.PoolObjects();
            LootLockerManager.Instance.SetMemberIDLabel();
            hiScoreScreen.SetActive(false);
            gameOverScreen.SetActive(false);

            if (onScreenControls != null)
                onScreenControls.SetActive(false);

            SetPlayerInput(false);

            hasContinued = false;
            hud.SetActive(false);
            gameOver = false;
            isPaused = false;
            if(restarted == true && LootLockerManager.Instance.isSignedIn == true)
            {
                
                StartGame();
            }else if(LootLockerManager.Instance.isSignedIn == true)
            {
                
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

            if (onScreenControls != null)
                onScreenControls.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(destroyButton);
        }

        public void ShowOfflineContinue()
        {
            LootLockerManager.Instance.isSignedIn = true;
            okayBadButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(okayBadButton);
        }

        public void ShowErrorScreen()
        {
            errorScreen.SetActive(true);
            signInGood.SetActive(false);
            signInScreen.SetActive(true);
            signingInScreen.SetActive(false);
        }

        // Start is called before the first frame update
        public void GameOver()
        {
            inputs.EnableMouseInput();
            SetPlayerInput(false);
            pauseMenu.SetActive(false);
            gameOverScreen.SetActive(true);

            if (onScreenControls != null)
                onScreenControls.SetActive(false);

            if (hasContinued == true)
                continueButton.SetActive(false);

            LootLockerManager.Instance.SubmitScore();
            gameOver = true;
            isPaused = false;
            Time.timeScale = 0;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(tryAgainButton);
        }

        public void Continue()
        {
            magnet.GetComponent<MagnetController>().SetVelocity();

            hasContinued = true;
            inputs.DisableInput();
            SetPlayerInput(true);
            pauseMenu.SetActive(false);
            gameOverScreen.SetActive(false);

            if(onScreenControls != null)
                onScreenControls.SetActive(true);

            gameOver = false;
            isPaused = false;
            Time.timeScale = 1;

            EventSystem.current.SetSelectedGameObject(null);
        }

        public void StartGame()
        {
            inputs.DisableInput();
            SetPlayerInput(true);
            menuCam.SetActive(false);
            Destroy(mainMenuCanvas);
            magnet.SetActive(true);
            gameCam.SetActive(true);
            hud.SetActive(true);

            if (onScreenControls != null)
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
            SetPlayerInput(false);
            inputs.EnableMouseInput();
            pauseMenu.SetActive(true);

            if (onScreenControls != null)
                onScreenControls.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(resumeGameButton);
        }

        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1;
            SetPlayerInput(true);
            inputs.DisableInput();
            inputs.pause = false;
            pauseMenu.SetActive(false);

            if (onScreenControls != null)
                onScreenControls.SetActive(true);
        }

        public void RestartGame()
        {
            restarted = true;
            gameStarted = true;
            gameOver = false;
            SetPlayerInput(true);

            if (onScreenControls != null)
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
            SetPlayerInput(false);

            if (onScreenControls != null)
                onScreenControls.SetActive(false);

            inputs.EnableMouseInput();
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void SetPlayerName(string userName)
        {
            LootLockerManager.Instance.UpdatePlayerName(userName);
        }

        private void SetPlayerInput(bool enabled)
        {
#if UNITY_EDITOR
            playerInput.enabled = enabled;
#elif (UNITY_IOS || UNITY_ANDROID)
            playerInput.enabled = false;
#else
            playerInput.enabled = enabled;
#endif


        }

        
    }
}
