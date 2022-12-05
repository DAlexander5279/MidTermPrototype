using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    [SerializeField] GameObject roomPref;
    [SerializeField] GameObject spawnPos;
    [SerializeField] bool roomSpawned;

    [SerializeField] bool isStartRoom;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
 

        if(gameManager.instance.enemyCount <= 0)
        {
            Debug.Log("moving");
            gameManager.instance.pushRoomsBack(gameObject);
        }


        if (!roomSpawned && gameManager.instance.enemyCount <= 0)
        {
            SpawnRoom();
        }




    }


    void SpawnRoom()
    {      
        roomSpawned = true;
        Instantiate(roomPref, spawnPos.transform.position, transform.rotation); 

    }


    public void OnTriggerExit(Collider other)
    {

       if(other.CompareTag("Player"))
        {
 //
            Destroy(gameObject);
        }

    }


}
