using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MorganHouston.MagnetDestroyer
{
    public class SpawnManager : MonoBehaviour
    {

        public static SpawnManager _sharedInstance;
        public List<GameObject> pooledObjects;
        public GameObject[] objectsToPool;
        public GameObject spawnRoom;
        public int amountToPool;

        private GameObject roomToRemove, firstRoomSpawned;
        private bool canSpawn = true;
        private bool firstRoomRemoved = false;
        [SerializeField]
        private Vector3 spawnPosition;
        [SerializeField]
        private Vector3 offset;


        private void Awake()
        {
            _sharedInstance = this;
            offset = new Vector3(0, 0, 50);
            spawnPosition = new Vector3(0, 0, -11.75f);
        }

        // Start is called before the first frame update
        void Start()
        {
            pooledObjects = new List<GameObject>();
            GameObject tmp;
            for(int i = 0; i < amountToPool; i++)
            {
                tmp = Instantiate(objectsToPool[i]);
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }

        public GameObject GetPooledObject()
        {
            for(int i = 0; i < amountToPool; i++)
            {
                if (!pooledObjects[i].activeInHierarchy)
                {
                    return pooledObjects[i];
                }
            }
            return null;
        }

        public void SpawnFirstRoom()
        {
            GameObject room = GetPooledObject();
            firstRoomSpawned = room;
            if (room != null)
            {
                room.transform.position = spawnPosition;
                room.transform.rotation = Quaternion.identity;
                room.SetActive(true);
                spawnPosition += offset;
            }
        }

        public void SpawnNextRoom()
        {
            if (canSpawn)
            {
                DestroyPrevRoom();
                GameObject room = GetPooledObject();
                if (room != null)
                {
                    room.transform.position = spawnPosition;
                    room.transform.rotation = Quaternion.identity;
                    room.SetActive(true);
                    spawnPosition += offset;
                }
                canSpawn = false;
            }
        }

        public void ResetCanSpawn(GameObject room)
        {
            canSpawn = true;
            roomToRemove = room;
        }

        public void DestroyPrevRoom()
        {
            if(spawnRoom != null)
            {
                Destroy(spawnRoom);
            } else if (firstRoomRemoved == false)
            {
                firstRoomRemoved = true;
                firstRoomSpawned.SetActive(false);
            }
            else
            {
                roomToRemove.SetActive(false);
            }
        }
    }
}
