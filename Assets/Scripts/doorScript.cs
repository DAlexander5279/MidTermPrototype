using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorScript : MonoBehaviour
{

    [SerializeField] GameObject door;
    [SerializeField] GameObject OpenPos;
    [SerializeField] GameObject ClosePos;
    [SerializeField] float closeSpd;
    [SerializeField] float openSpd;
    [SerializeField] bool roomClear;
    [SerializeField] bool activeRoom;

    private void Start()
    {
        activeRoom = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (gameManager.instance.enemyCount <= 0)
        {
            roomClear = true;
        }
        if (roomClear && activeRoom)
            OpenDoor();
    }
    void OpenDoor()
    {
        door.transform.position = Vector3.Lerp(door.transform.position, OpenPos.transform.position, Time.deltaTime * openSpd);
    }

    void CloseDoor()
    {
        while(door.transform.position != ClosePos.transform.position)
            door.transform.position = Vector3.Lerp(door.transform.position, ClosePos.transform.position, Time.deltaTime * closeSpd);
    }

    public void OnTriggerEnter(Collider other)
    {
        activeRoom = true;
        OpenDoor();
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            activeRoom = false;
            CloseDoor();
        }

    }

}
