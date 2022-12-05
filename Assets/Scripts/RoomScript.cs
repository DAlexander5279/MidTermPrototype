using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    [SerializeField] GameObject roomPref;
    [SerializeField] GameObject spawnPos;
    [SerializeField] bool roomSpawned;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (!roomSpawned && gameManager.instance.enemyCount <= 0)
        {
            SpawnRoom();
        }


    }


    void SpawnRoom()
    {
        Instantiate(roomPref, spawnPos.transform.position, transform.rotation); 
        roomSpawned = true;
    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
    }

}
