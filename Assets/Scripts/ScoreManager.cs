using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager _instance;
        public float Score { get; private set; }

        private TextMeshProUGUI scoreText;

        private void Awake()
        {
            _instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            scoreText = GetComponent<TextMeshProUGUI>();
            Score = 0f;
            scoreText.text = Score.ToString();
        }

        // Update is called once per frame
        void Update()
        {
            if(GameManager._instance.gameStarted == true && GameManager._instance.gameOver == false)
            {
                Score += Time.deltaTime;
                if(scoreText.text != Convert.ToInt32(Score).ToString())
                    scoreText.text = Convert.ToInt32(Score).ToString();
            }
        }

        public void AddScore(float scoreToAdd)
        {
            Score += scoreToAdd;
        }
    }
}
