using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class ControlsManager : MonoBehaviour
    {

        private TextMeshProUGUI controlsLabel;
        public 

        // Start is called before the first frame update
        void Start()
        {
            controlsLabel = GetComponent<TextMeshProUGUI>();
#if (UNITY_ANDROID || UNITY_IOS)
            controlsLabel.text = "Move: Swipe\nMagnetize: Tap\nDe-Magnetize: Tap\nSelect: Tap";
#elif UNITY_WSA
            controlsLabel.text = "Move: Left Stick\nMagnetize: Right Trigger/Button South\nDe-Magnetize: Right Trigger/Button South\nSelect: Button South";
#else
            controlsLabel.text = "Move: A/D\nMagnetize: Spacebar/Enter/LMB\nDe-Magnetize: Spacebar/Enter/LMB\nSelect: Spacebar/Enter";
#endif
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
