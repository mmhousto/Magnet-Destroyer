using System;
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
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {

            if (rb != null)
                rb.velocity = Vector3.zero;

            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.identity;


            if (GetComponent<MagneticTool>() != null)
            {
                MagneticTool mag = GetComponent<MagneticTool>();
                mag.AffectByMagnetism = false;
                mag.ConstantMagneticForce = false;
            }

            if (GetComponentInChildren<Light>() != null)
            {
                GetComponentInChildren<Light>().intensity = 90;
            }


            if (GetComponent<RotateObject>() != null)
            {
                GetComponent<RotateObject>().enabled = true;
                GetComponent<MagneticTool>().AffectByMagnetism = false;
            }
        }

        private void OnDisable()
        {
            hasHit = false;
            if (rb != null)
                rb.velocity = Vector3.zero;

            transform.localPosition = new Vector3(0, 0, 0);
            transform.rotation = Quaternion.identity;

            

            if(GetComponent<MagneticTool>() != null)
            {
                MagneticTool mag = GetComponent<MagneticTool>();
                mag.AffectByMagnetism = false;
                mag.ConstantMagneticForce = false;

            }
                

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

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Player") && hasHit == false)
            {
                hasHit = true;
                ScoreManager._instance.AddScore(1f);
            }
        }
    }
}
