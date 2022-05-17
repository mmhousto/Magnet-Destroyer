using System.Text.RegularExpressions;
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

        private TextAsset textAssetBlockList;
        [SerializeField] string[] strBlockList;

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
            textAssetBlockList = new TextAsset("dumbass,shithead,dumbfuck,jackass,asshead,dumbshit,asshead,fuckhead,ass,hoe,slut,whore,pussy,nigga,nigger,bitch,cunt,shit," +
                "fuck,fucker,arse,damn,tits,boob,titties,bastard,cock,dick,prick,punani,twat,piss,bltch,nlgger,nlgga,shlt,tlttles,tlts,dlck,prlck,punanl,plss,suck,sex");
            strBlockList = textAssetBlockList.text.Split(new string[] { ",", "\n" }, StringSplitOptions.RemoveEmptyEntries);
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

            if (GameManager._instance.gameStarted == false && GameManager._instance.gameOver == false && GameManager._instance.hiScoreScreen != null && GameManager._instance.hiScoreScreen.activeInHierarchy)
            {
                if(memberIDLabel == null)
                {
                    memberIDLabel = GameObject.FindWithTag("MemberLabel").GetComponent<TextMeshProUGUI>();
                }
                if(memberIDLabel != null)
                    SetMemberIDLabel();
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
                    for (int i = signInAttempts; i < MAX_SIGN_IN_ATTEMPTS; i++)
                    {
                        signInAttempts++;
                        SignInGuest();
                        return;
                    }
                    GameManager._instance.ShowOfflineContinue();

                    return;
                }

                memberID = response.player_id;
                GetPlayerName();
                SetMemberIDLabel();
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
            signInAttempts++;
            GameManager._instance.ShowErrorScreen();
            isSignedIn = false;
            if(signInAttempts < 3)
            {
                SignIn(playerID, userName);
                return;
            }
            GameManager._instance.ShowOfflineContinue();
            return;
        }

        public void SetMemberIDLabel()
        {
            if((userName != null && userName != "") && memberIDLabel.text != "User: " + userName)
                memberIDLabel.text = "User: " + userName;
            else if(memberIDLabel.text != "User: " + memberID.ToString() && (userName == null || userName == ""))
                memberIDLabel.text = "User: " + memberID.ToString();
        }

        private string FilterPlayerName(string textToCheck)
        {
            for(int i = 0; i < strBlockList.Length; i++)
            {
                string profanity = strBlockList[i];
                Regex word = new Regex(@"(?i)(" + profanity + ")");
                string temp = word.Replace(textToCheck, "***");
                textToCheck = temp;
            }
            return textToCheck;
        }

        public void UpdatePlayerName(string userName)
        {
            LootLockerSDKManager.SetPlayerName(userName, (response) =>
            {
                if (response.success)
                {
                    if(response.name == "")
                    {
                        this.userName = null;
                    }
                    else
                    {
                        this.userName = FilterPlayerName(response.name);
                    }

                    SetMemberIDLabel();
                }
                else
                {
                    
                }
            });
        }

        public void GetPlayerName()
        {
            LootLockerSDKManager.GetPlayerName((response) =>
            {
                if (response.success)
                {
                    userName = FilterPlayerName(response.name);
                }
                else
                {
                    Debug.Log("Error getting player name");
                }
            });
        }

        public void SignIn(string playerID, string userName)
        {
            LootLockerSDKManager.StartSession(playerID, (response) =>
            {
                if (response.success)
                {
                    GetPlayerName();
                    if(this.userName == null)
                        UpdatePlayerName(userName);
                    memberID = response.player_id;
                    SetMemberIDLabel();
                    ShowTopScores();
                    Login();
                }
                else
                {
                    
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
                        highscores += $"{playerData.rank}\t\t\t{((playerData.player.name != String.Empty) ? FilterPlayerName(playerData.player.name) : playerData.member_id)}\t\t\t{playerData.score}\n";
                    }
                    highscoresLabel.text = highscores;
                    
                }
                else
                {
                    
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
                                gameOverHighscores += $"{playerData.rank}\t\t\t{((playerData.player.name != String.Empty) ? FilterPlayerName(playerData.player.name) : playerData.member_id)}\t\t\t{playerData.score}\n";
                            }
                            gameOverHighscoresLabel.text = gameOverHighscores;
                            
                        }
                        else
                        {
                            
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
