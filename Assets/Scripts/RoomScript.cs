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

      if(!roomSpawned)
        {
            SpawnRoom();
            roomSpawned = true;
        }
      

    }


    void SpawnRoom()
    {
        Instantiate(roomPref, spawnPos.transform.position, transform.rotation);
    }


    public void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
    }

}
