using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class VersionHandler : MonoBehaviour
    {

        private TextMeshProUGUI versionLabel;

        // Start is called before the first frame update
        void Start()
        {
            versionLabel = GetComponent<TextMeshProUGUI>();
            versionLabel.text = "Version: " + Application.version;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
