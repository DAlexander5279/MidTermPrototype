using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    [SerializeField] GameObject roomPref;
    [SerializeField] GameObject door;
    [SerializeField] GameObject OpenPos;
    [SerializeField] GameObject ClosePos;
    [SerializeField] GameObject spawnPos;
    [SerializeField] float doorSpeedOpen;
    [SerializeField] float doorSpeedClose;
    [SerializeField] int enemyTemp;
    [SerializeField] bool roomSpawned;
    [SerializeField] bool doorOpened;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if(enemyTemp <= 0)
        {
            OpenDoor();

            if(!roomSpawned)
            {
                SpawnRoom();
                roomSpawned = true;
            }
        }


    }

    void OpenDoor()
    {
        door.transform.position = Vector3.Lerp(door.transform.position, OpenPos.transform.position, Time.deltaTime * doorSpeedOpen);
    }

    void CloseDoor()
    {
        door.transform.position = Vector3.Lerp(door.transform.position, ClosePos.transform.position, Time.deltaTime * doorSpeedClose);
    }
    void SpawnRoom()
    {
        Instantiate(roomPref, spawnPos.transform.position, transform.rotation);
    }

    public void OnTriggerEnter(Collider other)
    {

    }

    public void OnTriggerExit(Collider other)
    {
        CloseDoor();
        new WaitForSeconds(2);
        Destroy(gameObject);
    }

}
