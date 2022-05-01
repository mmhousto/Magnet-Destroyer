using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class MagneticObject : MonoBehaviour
    {
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private Rigidbody rb;
        private bool hasHit = false;

        private void Start()
        {
            spawnPosition = transform.parent.localPosition;
            spawnRotation = transform.parent.rotation;
            rb = GetComponent<Rigidbody>();
        }

        private void OnDisable()
        {
            hasHit = false;
            transform.localPosition = spawnPosition;
            transform.rotation = spawnRotation;

            if(rb != null)
                rb.velocity = Vector3.zero;

            if(GetComponent<MagneticTool>() != null)
                GetComponent<MagneticTool>().AffectByMagnetism = true;

            if(GetComponentInChildren<Light>() != null)
            {
                GetComponentInChildren<Light>().intensity = 0;
            }
                

            if(GetComponent<RotateObject>() != null)
            {
                GetComponent<RotateObject>().enabled = false;
                GetComponent<MagneticTool>().AffectByMagnetism = false;
            }
                
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Hit");
                hasHit = true;
                ScoreManager._instance.AddScore(1f);
            }
        }
    }
}
