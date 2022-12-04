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


    // Update is called once per frame
    void Update()
    {
        
    }
    void OpenDoor()
    {
        door.transform.position = Vector3.Lerp(door.transform.position, OpenPos.transform.position, Time.deltaTime * openSpd);
    }

    void CloseDoor()
    {
        door.transform.position = Vector3.Lerp(door.transform.position, ClosePos.transform.position, Time.deltaTime * closeSpd);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(gameManager.instance.enemyCount <=0)
        {
            OpenDoor();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        CloseDoor();
    }

}
