using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    [SerializeField] GameObject[] easyRooms;
    [SerializeField] GameObject[] allRooms;
    [SerializeField] GameObject spawnPos;
    [SerializeField] bool roomSpawned;
    [SerializeField] bool isStartRoom;
    [SerializeField] bool hasPushedPlayer;

    // Start is called before the first frame update
    void Start()
    {
        hasPushedPlayer = false;

    }

    // Update is called once per frame
    void Update()
    {
        

        if(gameManager.instance.enemyCount <= 0 && gameManager.instance.itemCount <= 0)
        {
            gameManager.instance.pushRoomsBack(gameObject);
            if(!hasPushedPlayer)
                StartCoroutine(PushPlayer());
           
        }


        if (!roomSpawned && gameManager.instance.enemyCount <= 0 && gameManager.instance.itemCount <= 0)
        {
            SpawnRoom();
        }


    }


    void SpawnRoom()
    {
        int index = 0;
        gameManager.instance.addWaveCount(1);


        roomSpawned = true;
        if(gameManager.instance.waveCount <= 5)
        {
            index = Random.Range(0, easyRooms.Length - 1);
            while (index == gameManager.instance.curRoomIndex)//Checks to make sure rooms are not spawning itself
            {
                index = Random.Range(0, easyRooms.Length - 1);
            }
            Debug.Log(index);
            gameManager.instance.curRoomIndex = index;
            Instantiate(easyRooms[index], spawnPos.transform.position, transform.rotation); 
        }
        else
        {
            index = Random.Range(0, allRooms.Length - 1);
            while (index == gameManager.instance.curRoomIndex)//Checks to make sure rooms are not spawning itself
            {
                index = Random.Range(0, allRooms.Length - 1);
            }
            Debug.Log(index);
            gameManager.instance.curRoomIndex = index;
            GameObject room =  Instantiate(allRooms[index], spawnPos.transform.position, transform.rotation);

        }


    }


    public void OnTriggerExit(Collider other)
    {

       if(other.CompareTag("Player"))
        {
 //
            Destroy(gameObject);
        }

    }

    IEnumerator PushPlayer()
    {
        gameManager.instance.playerScript.isDisabled = true;
        gameManager.instance.player.transform.position = new Vector3(gameManager.instance.player.transform.position.x - 25, gameManager.instance.player.transform.position.y, gameManager.instance.player.transform.position.z);
        yield return new WaitForSeconds(0.05f);
        hasPushedPlayer = true;
        gameManager.instance.playerScript.isDisabled = false;
    }
}
