using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class SigningIn : MonoBehaviour
    {

        public TextMeshProUGUI signingInLabel;
        private int dots = 1;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(SignIn());
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        IEnumerator SignIn()
        {
            switch (dots)
            {
                case 1:
                    signingInLabel.text = "Signing In.";
                    break;
                case 2:
                    signingInLabel.text = "Signing In..";
                    break;
                case 3:
                    signingInLabel.text = "Signing In...";
                    break;
                default:
                    signingInLabel.text = "Signing In...";
                    break;
            }
            if(dots < 3)
            {
                dots++;
            }
            else
            {
                dots = 1;
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }
}
