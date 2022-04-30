using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchStop : MonoBehaviour
{
    [SerializeField] private MagneticTool magneticTool;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent< MagneticTool>() && !magneticTool.IsMetallic)
        {
            magneticTool.AffectByMagnetism = false;
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.GetComponent<MagneticTool>().TurnOnMagnetism)
        {
            magneticTool.AffectByMagnetism = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<MagneticTool>() && !magneticTool.IsMetallic)
        {
            magneticTool.AffectByMagnetism = true;
        }
    }
}
