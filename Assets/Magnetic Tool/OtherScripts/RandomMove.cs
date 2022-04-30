using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : MonoBehaviour
{
    public float velocidad;
    public int NewPositionsRadius;

    private bool finish;
    private Vector3 destiny;

    private void Start()
    {
        finish = true;
    }

    private void FixedUpdate()
    {
        if (finish) destiny = RandomPosition();
        GoToPoint(destiny);
    }

    private Vector3 RandomPosition()
    {
        Vector3 result = new Vector3();

        result.x = Random.Range(-NewPositionsRadius, NewPositionsRadius);
        result.y = transform.position.y; 
        result.z = Random.Range(-NewPositionsRadius, NewPositionsRadius);

        return result;
    }

    private void GoToPoint(Vector3 newPosition)
    {
        Vector3 vectorDir = newPosition - transform.position;
        vectorDir.Normalize();

        float distance = Vector3.Distance(newPosition, transform.position);
        finish = false;

        if (distance > 0.1)
        {
            transform.position += vectorDir * velocidad;
        }
        else 
        {
            finish = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Finish") finish = true;
    }
}
