using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

namespace Com.MorganHouston.MagnetDestroyer
{
    public class LoginManager : MonoBehaviour
    {

        public bool loginSuccessful;
        public static string playerID;
        public static string userName;

        private void Awake()
        {
#if UNITY_ANDROID
            InitializePlayGamesLogin();
#endif
        }

        // Start is called before the first frame update
        void Start()
        {
            AuthenticateUser();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void InitializePlayGamesLogin()
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }

        void AuthenticateUser()
        {
#if UNITY_IOS
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    loginSuccessful = true;
                    playerID = Social.localUser.id;
                    userName = Social.localUser.userName;
                    LootLockerManager.Instance.SignIn(playerID, userName);
                }
                else
                {
                    Debug.Log("unsuccessful");
                }
            });
#elif UNITY_ANDROID
            PlayGamesPlatform.Instance.Authenticate((SignInStatus success) =>
            {
                if (success == SignInStatus.Success)
                {
                    loginSuccessful = true;
                    playerID = Social.localUser.id;
                    userName = Social.localUser.userName;
                    LootLockerManager.Instance.SignIn(playerID, userName);
                }
                else
                {
                    Debug.Log("unsuccessful");
                }
            });
#endif
        }
    }
}
