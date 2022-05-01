using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class MagnetController : MonoBehaviour
    {
        public static MagnetController _instance;

        public Slider magnetPowerSlider;

        public StarterAssetsInputs inputs;
        public Rigidbody magnetRigidbody;
        private MagneticTool magnet;
        private float horizontal, vertical;
        public float verticalSpeed = 12.5f;
        private float movementSpeed = 15f;
        [Range(0,15)]
        private float magnetTime = 15f;
        private bool canMagnetize = true;

        private void Awake()
        {
            _instance = this;
            gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            magnetRigidbody = GetComponent<Rigidbody>();
            magnet = GetComponent<MagneticTool>();
        }

        // Update is called once per frame
        void Update()
        {
            MoveMagnet();
            CheckCanManetize();
            Magnetize();
            IncreaseSpeed();
        }

        private void IncreaseSpeed()
        {
            if(verticalSpeed < 60f)
                verticalSpeed += Time.deltaTime / 2;
        }

        private void MoveMagnet()
        {
            var move = inputs.move.normalized;
            horizontal = move.x;
            //vertical = move.y;
            Vector3 movement = new Vector3(horizontal * movementSpeed, 0, verticalSpeed) * Time.deltaTime;
            transform.Translate(movement);

            if(transform.position.x < 3)
            {
                transform.position = new Vector3(3, transform.position.y, transform.position.z);
            }
            if(transform.position.x > 47)
            {
                transform.position = new Vector3(47, transform.position.y, transform.position.z);
            }
        }

        private void Magnetize()
        {
            if(magnet.NorthPole == inputs.magnetize && canMagnetize)
                magnet.NorthPole = !inputs.magnetize;
        }

        private void CheckCanManetize()
        {
            if (magnet.NorthPole == true && magnetTime < 15)
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

                if(other.GetComponentInChildren<Light>() != null)
                    other.GetComponentInChildren<Light>().intensity = 0;

                if(other.GetComponent<RotateObject>() != null)
                    other.GetComponent<RotateObject>().enabled = false;
            }
            else if (other.CompareTag("Magnetic") && magnet.NorthPole == true)
            {
                other.GetComponent<MagneticTool>().AffectByMagnetism = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("MetalicCube2"))
            {
                if (magnet.NorthPole == true)
                {
                    magnetTime += 5f;
                }
                else if (magnet.NorthPole == false)
                {
                    verticalSpeed = 0f;
                    inputs.enabled = false;
                    GameManager._instance.GameOver();
                }

            }

            if (collision.transform.CompareTag("MetalicCube1"))
            {
                if (magnet.NorthPole == true)
                {
                    verticalSpeed = 0f;
                    inputs.enabled = false;
                    GameManager._instance.GameOver();
                    
                }
                else if (magnet.NorthPole == false)
                {
                    magnetTime += 5f;
                }

            }
        }

    }
}
