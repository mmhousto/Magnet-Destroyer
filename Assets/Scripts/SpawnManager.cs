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

        private Queue<GameObject> roomsToRemove;
        private GameObject roomToRemove, firstRoomSpawned;
        private bool canSpawn = true;
        private bool firstRoomRemoved = false;
        [SerializeField]
        private Vector3 spawnPosition;
        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private Vector3 endOffSet;
        private Vector3 endPosition;


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
            roomsToRemove = new Queue<GameObject>();
            GameObject tmp;
            for (int i = 0; i < amountToPool; i++)
            {
                tmp = Instantiate(objectsToPool[i]);
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }

        private void Update()
        {
            if (MagnetController._instance.transform.position.z > endPosition.z && GameManager._instance.gameStarted == true && GameManager._instance.gameOver == false)
            {
                SpawnNextRoom();
            }
        }

        public GameObject GetPooledObject()
        {
            int rand = Random.Range(0, 8);

            for (int i = rand; i < amountToPool; i = Random.Range(0, 8))
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
                endPosition = spawnPosition + endOffSet;
                room.transform.position = spawnPosition;
                room.transform.rotation = Quaternion.identity;
                room.SetActive(true);
                roomsToRemove.Enqueue(room);
                spawnPosition += offset;
            }
        }

        public void SpawnNextRoom()
        {
            if (spawnRoom != null)
            {
                Destroy(spawnRoom);
            }

            GameObject room = GetPooledObject();
            
            if (room != null)
            {
                endPosition = spawnPosition + endOffSet;
                room.transform.position = spawnPosition;
                room.transform.rotation = Quaternion.identity;
                room.SetActive(true);
                roomsToRemove.Enqueue(room);
                spawnPosition += offset;
            }
            
            if(roomsToRemove.Count > 2)
            {
                DestroyPrevRoom();
            }
        }

        public void ResetCanSpawn(GameObject room)
        {
            canSpawn = true;
            roomToRemove = room;
        }

        public void DestroyPrevRoom()
        {
            roomToRemove = roomsToRemove.Dequeue();
            roomToRemove.SetActive(false);
        }
    }
}
