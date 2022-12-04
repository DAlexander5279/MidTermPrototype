using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{

    [SerializeField] GameObject door;
    [SerializeField] GameObject OpenPos;
    [SerializeField] GameObject ClosePos;
    [SerializeField] float doorSpeedOpen;
    [SerializeField] float doorSpeedClose;
    [SerializeField] int enemyTemp;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(enemyTemp <= 0)
        {
            CloseDoor();
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
}
