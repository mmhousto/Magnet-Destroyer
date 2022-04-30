using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource audioSource;
        private bool musicOn;

        public TextMeshProUGUI musicButtonText;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            musicOn = Convert.ToBoolean(PlayerPrefs.GetInt("Music", 1));

            if (musicOn == true)
            {
                musicButtonText.text = "MUSIC ON";
                audioSource.volume = 1;
            }
            else
            {
                musicButtonText.text = "MUSIC OFF";
                audioSource.volume = 0;
            }

        }

        public void MusicOnOff()
        {
            musicOn = !musicOn;
            

            if (musicOn == true)
            {
                PlayerPrefs.SetInt("Music", 1);
                musicButtonText.text = "MUSIC ON";
                audioSource.volume = 1;
            }
            else
            {
                PlayerPrefs.SetInt("Music", 0);
                musicButtonText.text = "MUSIC OFF";
                audioSource.volume = 0;
            }
                
        }
    }
}
