using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    public Material northMaterial;
    public Material southMaterial;
    // Update is called once per frame
    void Update()
    {
        var script = gameObject.GetComponent<MagneticTool>();
        if (!script)
        {
            var script2 = gameObject.GetComponent<MagneticTool2D>();

            if (script2.NorthPole) gameObject.GetComponent<MeshRenderer>().material = northMaterial;
            else gameObject.GetComponent<MeshRenderer>().material = southMaterial;
        }
        else
        {
            if (script.NorthPole) gameObject.GetComponent<MeshRenderer>().material = northMaterial;
            else gameObject.GetComponent<MeshRenderer>().material = southMaterial;
        }
    }
}
