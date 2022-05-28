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
        private MagneticTool magneticTool;
        public bool hasHit = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            magneticTool = GetComponent<MagneticTool>();
        }

        private void OnEnable()
        {
            magneticTool = GetComponent<MagneticTool>();

            if (rb != null)
                rb.velocity = Vector3.zero;

            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.identity;


            if (magneticTool != null)
            {
                magneticTool.AffectByMagnetism = false;
                magneticTool.ConstantMagneticForce = false;
            }

            if (GetComponentInChildren<Light>() != null)
            {
                GetComponentInChildren<Light>().intensity = 90;
            }


            if (GetComponent<RotateObject>() != null)
            {
                GetComponent<RotateObject>().enabled = true;
                magneticTool.AffectByMagnetism = false;
            }
        }

        private void OnDisable()
        {
            hasHit = false;
            if (rb != null)
                rb.velocity = Vector3.zero;

            transform.localPosition = new Vector3(0, 0, 0);
            transform.rotation = Quaternion.identity;

            

            if(magneticTool != null)
            {
                magneticTool.AffectByMagnetism = false;
                magneticTool.ConstantMagneticForce = false;

            }
                

            if(GetComponentInChildren<Light>() != null)
            {
                GetComponentInChildren<Light>().intensity = 0;
            }
                

            if(GetComponent<RotateObject>() != null)
            {
                GetComponent<RotateObject>().enabled = false;
                magneticTool.AffectByMagnetism = false;
            }
                
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && hasHit == false)
            {
                hasHit = true;
                ScoreManager._instance.AddScore(1f);
                //magneticTool.AddToMagneticObjectsList(gameObject);
            }
        }
    }
}
