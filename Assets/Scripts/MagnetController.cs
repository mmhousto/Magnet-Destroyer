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
        private float magnetTime = 7.5f;
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
            CheckForPause();
        }

        public void SetVelocity()
        {
            verticalSpeed = 15;
        }

        private void CheckForPause()
        {
            if(inputs.pause == true && GameManager._instance.isPaused == false && GameManager._instance.gameOver == false)
            {
                GameManager._instance.PauseGame();
            }
            else if(inputs.pause == false && GameManager._instance.isPaused == true && GameManager._instance.gameOver == false)
            {
                GameManager._instance.ResumeGame();
            }
        }

        private void IncreaseSpeed()
        {
            if(verticalSpeed < 50f)
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
            if(canMagnetize)
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
                magnetTime = 0;
                canMagnetize = false;
                inputs.magnetize = false;
                magnet.NorthPole = true;
            }
            else if(magnetTime >= 15)
            {
                magnetTime = 15;
                canMagnetize = false;
                inputs.magnetize = true;
                magnet.NorthPole = false;
                
            }
            else { canMagnetize = true; }

            magnetPowerSlider.value = magnetTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("CeilingFan"))
            {
                other.GetComponent<MagneticTool>().AffectByMagnetism = true;

                if(other.GetComponentInChildren<Light>() != null)
                    other.GetComponentInChildren<Light>().intensity = 0;

                if(other.GetComponent<RotateObject>() != null)
                    other.GetComponent<RotateObject>().enabled = false;
            }

            if (other.CompareTag("Magnetic"))
            {
                other.GetComponent<MagneticTool>().AffectByMagnetism = true;

                if (other.GetComponentInChildren<Light>() != null)
                    other.GetComponentInChildren<Light>().intensity = 0;

                if (other.GetComponent<RotateObject>() != null)
                    other.GetComponent<RotateObject>().enabled = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("MetalicCube2") && collision.gameObject.GetComponentInChildren<MagneticTool>().NorthPole == true)
            {
                if (magnet.NorthPole == true)
                {
                    magnetTime -= 5f;
                }
                else if (magnet.NorthPole == false)
                {
                    verticalSpeed = 0f;
                    inputs.enabled = false;
                    GameManager._instance.GameOver();
                }

            }

            if (collision.transform.CompareTag("MetalicCube2") && collision.gameObject.GetComponentInChildren<MagneticTool>().NorthPole == false)
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
