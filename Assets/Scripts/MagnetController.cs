using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class MagnetController : MonoBehaviour
    {
        public Slider magnetPowerSlider;

        private StarterAssetsInputs inputs;
        private Rigidbody magnetRigidbody;
        private MagneticTool magnet;
        private float horizontal, vertical;
        private float verticalSpeed = 12.5f;
        private float movementSpeed = 15f;
        [Range(0,20)]
        private float magnetTime = 20f;
        private bool canMagnetize = true;

        // Start is called before the first frame update
        void Start()
        {
            inputs = GetComponent<StarterAssetsInputs>();
            magnetRigidbody = GetComponent<Rigidbody>();
            magnet = GetComponent<MagneticTool>();
        }

        // Update is called once per frame
        void Update()
        {
            MoveMagnet();
            CheckCanManetize();
            Magnetize();
        }

        private void MoveMagnet()
        {
            var move = inputs.move.normalized;
            horizontal = move.x;
            //vertical = move.y;
            Vector3 movement = new Vector3(horizontal * movementSpeed, 0, verticalSpeed) * Time.deltaTime;
            transform.Translate(movement);
        }

        private void Magnetize()
        {
            if(magnet.NorthPole == inputs.magnetize && canMagnetize)
                magnet.NorthPole = !inputs.magnetize;
        }

        private void CheckCanManetize()
        {
            if (magnet.NorthPole == true && magnetTime < 20)
            {
                magnetTime += Time.deltaTime;
            }
            else if (magnet.NorthPole == false && magnetTime > 0)
            {
                magnetTime -= Time.deltaTime;
            }

            if (magnetTime <= 0)
            {
                canMagnetize = false;
                inputs.magnetize = false;
                magnet.NorthPole = true;
            }
            else
                canMagnetize = true;

            magnetPowerSlider.value = magnetTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Magnetic") && magnet.NorthPole == false)
            {
                other.GetComponent<MagneticTool>().AffectByMagnetism = true;
                other.GetComponentInChildren<Light>().intensity = 0;
                other.GetComponent<RotateObject>().enabled = false;
                other.transform.parent = null;
            }
            else if (other.CompareTag("Magnetic") && magnet.NorthPole == true)
            {
                other.GetComponent<MagneticTool>().AffectByMagnetism = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Room"))
            {
                SpawnManager._sharedInstance.SpawnNextRoom();
            }
            if (other.CompareTag("Reset"))
            {
                SpawnManager._sharedInstance.ResetCanSpawn(other.gameObject);
            }
        }

    }
}
