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

        public GameManager gameManager;

        public GameObject okayBadButton, errorScreen, mainMenuScreen, signInScreen;

        public TextMeshProUGUI highscoresLabel, gameOverHighscoresLabel;

        public static bool isSignedIn;

        private string highscores, gameOverHighscores;
        int memberID;
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
        }

        void Start()
        {
            if (isSignedIn)
            {
                Login();
            }
        }

        public void SignInGuest()
        {
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (!response.success)
                {
                    errorScreen.SetActive(true);
                    signInScreen.SetActive(false);
                    isSignedIn = false;
                    for(int i = signInAttempts; i < MAX_SIGN_IN_ATTEMPTS; i++)
                    {
                        SignInGuest();
                        return;
                    }
                    okayBadButton.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(okayBadButton);
                    return;
                }

                Debug.Log("successfully started LootLocker session");
                
                memberID = response.player_id;
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
            signInScreen.SetActive(false);
            errorScreen.SetActive(false);
            mainMenuScreen.SetActive(true);
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

            LootLockerSDKManager.GetScoreList(2677, count, (response) =>
            {
                if (response.statusCode == 200)
                {
                    foreach (LootLockerLeaderboardMember player in response.items)
                    {
                        highscores = $"{player.rank}\t\t\t{player.member_id}\t\t\t{player.score}\n";
                    }
                    highscoresLabel.text = "RANK\t\t\tMEMBER\t\t\tSCORE\n" +
                                            highscores;
                    Debug.Log(highscores);
                }
                else
                {
                    Debug.Log("failed: " + response.Error);
                }
            });
        }

        public void ShowScore()
        {
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
                            foreach (LootLockerLeaderboardMember player in response.items)
                            {
                                gameOverHighscores = $"{player.rank}\t\t\t{player.member_id}\t\t\t{player.score}\n";
                            }
                            gameOverHighscoresLabel.text = "RANK\t\tMEMBER\t\tSCORE\n" +
                                                    gameOverHighscores;
                            Debug.Log(gameOverHighscores);
                            Debug.Log("Successful");
                        }
                        else
                        {
                            Debug.Log("failed: " + response.Error);
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
