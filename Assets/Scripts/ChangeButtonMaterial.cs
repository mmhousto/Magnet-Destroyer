using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class ChangeButtonMaterial : MonoBehaviour
    {
        public Image button;
        public MagneticTool magnet;

        private Color northColor, southColor;

        private void Awake()
        {
            if (button != null)
            {
                southColor = new Color(244, 1, 1);
                ColorUtility.TryParseHtmlString("#F50101", out southColor);
                northColor = new Color(0, 184, 255);
                ColorUtility.TryParseHtmlString("#00B8FF", out northColor);
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (magnet.NorthPole == true && button != null)
            {
                button.color = northColor;
            }
            else if (magnet.NorthPole == false && button != null)
            {
                button.color = southColor;
            }
        }
    }
}
