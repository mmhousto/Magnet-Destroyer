using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LootLocker.Requests;

namespace Com.MorganHouston.MagnetDestroyer
{

    public class LootLockerManager : MonoBehaviour
    {
        private static LootLockerManager _instance;

        public static LootLockerManager Instance { get { return _instance; } }

        public GameManager gameManager;

        public TextMeshProUGUI highscoresLabel;

        private string highscores;
        int memberID;
        string leaderboardID = "highscore-key";
        int score = 1000;
        int count = 50;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(this.gameObject);
            else
                _instance = this;
        }

        void Start()
        {
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (!response.success)
                {
                    Debug.Log("error starting LootLocker session");

                    return;
                }

                Debug.Log("successfully started LootLocker session");
                memberID = response.player_id;
                gameManager.GameOver();
            });
        }

        public void SubmitScore()
        {
            LootLockerSDKManager.SubmitScore(memberID.ToString(), score, leaderboardID, (response) =>
            {
                if (response.statusCode == 200)
                {
                    Debug.Log("Successful");
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
                }
                else
                {
                    Debug.Log("failed: " + response.Error);
                }
            });
        }
    }

}
