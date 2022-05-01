using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class BlockerManager : MonoBehaviour
    {
        private void OnEnable()
        {
            gameObject.SetActive(true);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("Magnetic2") || collision.transform.CompareTag("Magnetic"))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
