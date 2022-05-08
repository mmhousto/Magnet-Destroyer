using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class BlockerEnabler : MonoBehaviour
    {
        public GameObject blocker;
        private void OnEnable()
        {
            blocker.SetActive(true);
        }
    }
}
