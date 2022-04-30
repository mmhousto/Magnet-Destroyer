using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class GameManager : MonoBehaviour
    {
        public GameObject menuCam, gameCam, magnet;

        // Update is called once per frame
        void Update()
        {

        }

        // Start is called before the first frame update
        public void GameOver()
        {
            LootLockerManager.Instance.SubmitScore();
            LootLockerManager.Instance.ShowTopScores();
        }

        public void StartGame()
        {
            menuCam.SetActive(false);
            magnet.SetActive(true);
            gameCam.SetActive(true);
            SpawnManager._sharedInstance.SpawnFirstRoom();
        }

        
    }
}
