using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using LootLocker.Requests;
using System;

namespace Com.MorganHouston.MagnetDestroyer
{

    public class LootLockerManager : MonoBehaviour
    {
        private static LootLockerManager _instance;

        public static LootLockerManager Instance { get { return _instance; } }

        private TextMeshProUGUI highscoresLabel, gameOverHighscoresLabel, memberIDLabel;

        public bool isSignedIn;

        private string highscores, gameOverHighscores;
        int memberID;
        string userName;
        string leaderboardID = "highscore-key";
        int count = 10;
        int signInAttempts = 0;
        const int MAX_SIGN_IN_ATTEMPTS = 3;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(this.gameObject);
            else
                _instance = this;
 
            DontDestroyOnLoad(this.gameObject);

            highscoresLabel = GameObject.FindWithTag("HiScores").GetComponent<TextMeshProUGUI>();
            gameOverHighscoresLabel = GameObject.FindWithTag("GameOverHiScores").GetComponent<TextMeshProUGUI>();
            memberIDLabel = GameObject.FindWithTag("MemberLabel").GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            
        }

        private void Update()
        {
            if (GameManager._instance.gameStarted == false && GameManager._instance.hiScoreScreen != null)
            {
                if (highscoresLabel == null && GameManager._instance.hiScoreScreen.activeInHierarchy && GameManager._instance.gameStarted == false)
                {
                    highscoresLabel = GameObject.FindWithTag("HiScores").GetComponent<TextMeshProUGUI>();
                }

                if (highscoresLabel.text != highscores && highscoresLabel != null)
                {
                    highscoresLabel.text = highscores;
                }

            }
            else if(GameManager._instance.gameOver == true && GameManager._instance.gameOverScreen != null)
            {
                if (gameOverHighscoresLabel == null && GameManager._instance.gameOverScreen.activeInHierarchy)
                {
                    gameOverHighscoresLabel = GameObject.FindWithTag("GameOverHiScores").GetComponent<TextMeshProUGUI>();
                }

                if (gameOverHighscoresLabel.text != gameOverHighscores && gameOverHighscoresLabel != null)
                {
                    gameOverHighscoresLabel.text = gameOverHighscores;
                }
            }

            
        }

        public void SignInGuest()
        {
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (!response.success)
                {
                    GameManager._instance.ShowErrorScreen();
                    isSignedIn = false;
                    for (signInAttempts = 0; signInAttempts < MAX_SIGN_IN_ATTEMPTS; signInAttempts++)
                    {
                        SignInGuest();
                        return;
                    }
                    GameManager._instance.ShowOfflineContinue();

                    return;
                }

                Debug.Log("successfully started LootLocker session");

                memberID = response.player_id;
                memberIDLabel.text = "User: " + memberID.ToString();
                ShowTopScores();
                Login();

            });
        }
        private void OnApplicationQuit()
        {
            isSignedIn = false;
        }

        private void Login()
        {
            isSignedIn = true;
            GameManager._instance.ShowMainMenu();
        }

        private void ErrorTryAgainPlatform(string playerID, string userName)
        {
            GameManager._instance.ShowErrorScreen();
            isSignedIn = false;
            for (signInAttempts = 0; signInAttempts < MAX_SIGN_IN_ATTEMPTS; signInAttempts++)
            {
                SignIn(playerID, userName);
                return;
            }
            GameManager._instance.ShowOfflineContinue();
            return;
        }

        public void UpdatePlayerName(string userName)
        {
            LootLockerSDKManager.SetPlayerName(userName, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("Successfully set player name");
                    this.userName = response.name;
                    memberIDLabel.text = "User: " + userName;
                }
                else
                {
                    Debug.Log("Error setting player name");
                }
            });
        }

        public void SignIn(string playerID, string userName)
        {
            LootLockerSDKManager.StartSession(playerID, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("session with LootLocker started");
                    this.userName = userName;
                    memberID = response.player_id;
                    memberIDLabel.text = "User: " + userName;
                    UpdatePlayerName(userName);
                    ShowTopScores();
                    Login();
                }
                else
                {
                    Debug.Log("failed to start sessions" + response.Error);
                    ErrorTryAgainPlatform(playerID, userName);
                }
            });
        }

        public void SubmitScore()
        {
            LootLockerSDKManager.SubmitScore(memberID.ToString(), Convert.ToInt32(ScoreManager._instance.Score), leaderboardID, (response) =>
            {
                if (response.statusCode == 200)
                {
                    Debug.Log("Successful");
                    ShowScore();
                }
                else
                {
                    Debug.Log("failed: " + response.Error);
                }
            });
        }

        public void ShowTopScores()
        {
            highscores = "";
            LootLockerSDKManager.GetScoreList(2677, count, (response) =>
            {
                if (response.statusCode == 200)
                {
                    foreach (LootLockerLeaderboardMember playerData in response.items)
                    {
                        highscores += $"{playerData.rank}\t\t\t{((playerData.player.name != String.Empty) ? playerData.player.name : playerData.member_id)}\t\t\t{playerData.score}\n";
                    }
                    highscoresLabel.text = "RANK\t\t\tMEMBER\t\t\tSCORE\n" +
                                            highscores;
                    Debug.Log(highscores);
                }
                else
                {
                    Debug.Log("failed: " + response.Error);
                    highscores = "Failed to load...";
                }
            });
        }

        public void ShowScore()
        {
            gameOverHighscores = "";
            LootLockerSDKManager.GetMemberRank(leaderboardID, memberID, (response) =>
            {
                if (response.statusCode == 200)
                {
                    int rank = response.rank;
                    int count = 5;
                    int after = rank < 6 ? 0 : rank - 5;

                    LootLockerSDKManager.GetScoreListMain(2677, count, after, (response) =>
                    {
                        if (response.statusCode == 200)
                        {
                            foreach (LootLockerLeaderboardMember playerData in response.items)
                            {
                                gameOverHighscores += $"{playerData.rank}\t\t\t{((playerData.player.name != String.Empty) ? playerData.player.name : playerData.member_id)}\t\t\t{playerData.score}\n";
                            }
                            gameOverHighscoresLabel.text = "RANK\t\tMEMBER\t\tSCORE\n" +
                                                    gameOverHighscores;
                            Debug.Log(gameOverHighscores);
                            Debug.Log("Successful");
                        }
                        else
                        {
                            Debug.Log("failed: " + response.Error);
                            gameOverHighscores = "Failed to Load...";
                        }
                    });
                }
                else
                {
                    Debug.Log("failed: " + response.Error);
                }
            });
        }

    }

}
