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
        string iosAdUnitId = "Rewarded_iOS";
        string iosGameId = "4748334";
        string gameId;
        string adUnitId;

        private void Awake()
        {
            InitServices();
        }

        public async void InitServices()
        {
#if UNITY_ANDROID
            gameId = androidGameId;
            adUnitId = androidAdUnitId;
#elif UNITY_IOS
            gameId = iosGameId;
            adUnitId = iosAdUnitId;
#endif

            try
            {
#if (UNITY_ANDROID || UNITY_IOS)
                InitializationOptions initializationOptions = new InitializationOptions();
                initializationOptions.SetGameId(gameId);
                await UnityServices.InitializeAsync(initializationOptions);
#elif UNITY_EDITOR
                await UnityServices.InitializeAsync();
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
#if (UNITY_ANDROID || UNITY_IOS)
            //Create
            ad = MediationService.Instance.CreateRewardedAd(adUnitId);
#elif UNITY_EDITOR
            //Create
            ad = MediationService.Instance.CreateRewardedAd("myExampleAdUnit");
#endif

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
            // Pre-load the next ad
            ad.Load();
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