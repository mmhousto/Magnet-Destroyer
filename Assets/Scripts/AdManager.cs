using System;
using Unity.Services.Core;
using Unity.Services.Mediation;
using UnityEngine;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class AdManager : MonoBehaviour
    {
        IRewardedAd ad;
        string androidAdUnitId = "Rewarded_Android";
        string androidGameId = "4748335";
        string iosAdUnitId = "iOS_Rewarded";
        string iosGameId = "4748334";
        string gameId;
        string adUnitId;

        private void Awake()
        {
            InitServices();
        }

        public async void InitServices()
        {
            try
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    gameId = androidGameId;
                    adUnitId = androidAdUnitId;
                    InitializationOptions initializationOptions = new InitializationOptions();
                    initializationOptions.SetGameId(gameId);
                    await UnityServices.InitializeAsync(initializationOptions);
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    gameId = iosGameId;
                    adUnitId = iosAdUnitId;
                    InitializationOptions initializationOptions = new InitializationOptions();
                    initializationOptions.SetGameId(gameId);
                    await UnityServices.InitializeAsync(initializationOptions);
                }
#if UNITY_EDITOR
                else
                {
                    adUnitId = "myExampleAdUnitId";
                    await UnityServices.InitializeAsync();
                }
#endif


                InitializationComplete();
            }
            catch (Exception e)
            {
                InitializationFailed(e);
            }
        }

        public void SetupAd()
        {
            //Create ad
            ad = MediationService.Instance.CreateRewardedAd(adUnitId);

            //Subscribe to events
            ad.OnLoaded += AdLoaded;
            ad.OnFailedLoad += AdFailedLoad;

            ad.OnShowed += AdShown;
            ad.OnFailedShow += AdFailedShow;
            ad.OnClosed += AdClosed;
            ad.OnClicked += AdClicked;
            ad.OnUserRewarded += UserRewarded;

            // Impression Event
            MediationService.Instance.ImpressionEventPublisher.OnImpression += ImpressionEvent;
        }

        public void ShowAd()
        {
            if (ad.AdState == AdState.Loaded)
            {
                ad.Show();
            }
            else
            {
                GameManager._instance.Continue();
            }
        }

        void InitializationComplete()
        {
            SetupAd();
            ad.Load();
        }

        void InitializationFailed(Exception e)
        {
            Debug.Log("Initialization Failed: " + e.Message);
        }

        void AdLoaded(object sender, EventArgs args)
        {
            Debug.Log("Ad loaded");
        }

        void AdFailedLoad(object sender, LoadErrorEventArgs args)
        {
            Debug.Log("Failed to load ad");
            Debug.Log(args.Message);
        }

        void AdShown(object sender, EventArgs args)
        {
            Debug.Log("Ad shown!");
        }

        void AdClosed(object sender, EventArgs e)
        {

            GameManager._instance.Continue();
            Debug.Log("Ad has closed");
            // Execute logic after an ad has been closed.
        }

        void AdClicked(object sender, EventArgs e)
        {
            Debug.Log("Ad has been clicked");
            // Execute logic after an ad has been clicked.
        }

        void AdFailedShow(object sender, ShowErrorEventArgs args)
        {
            Debug.Log(args.Message);
            GameManager._instance.Continue();
        }

        void ImpressionEvent(object sender, ImpressionEventArgs args)
        {
            var impressionData = args.ImpressionData != null ? JsonUtility.ToJson(args.ImpressionData, true) : "null";
            Debug.Log("Impression event from ad unit id " + args.AdUnitId + " " + impressionData);
        }

        void UserRewarded(object sender, RewardEventArgs e)
        {
            Debug.Log($"Received reward: type:{e.Type}; amount:{e.Amount}");
            //GameManager._instance.Continue();
        }
    }

}