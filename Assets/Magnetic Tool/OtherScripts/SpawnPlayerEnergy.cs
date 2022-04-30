using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerEnergy : MonoBehaviour
{
    [SerializeField] private GameObject objectRespawn;
    [SerializeField] private float respawnTime;
    [SerializeField] private int maxNumberObjects;
    [SerializeField] private int spawnRadius;

    private float timer;

    private void Start()
    {
        timer = 0;
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (maxNumberObjects >= transform.childCount)
        {
            if (timer >= respawnTime)
            {
                Instantiate(objectRespawn, RandomPosition(), Quaternion.identity, transform);
                timer = 0;
            }
        }
    }

    private Vector3 RandomPosition()
    {
        Vector3 result = new Vector3();

        result.x = Random.Range(-spawnRadius, spawnRadius);
        result.y = objectRespawn.transform.localScale.y/2;
        result.z = Random.Range(-spawnRadius, spawnRadius);

        return result;
    }
}
