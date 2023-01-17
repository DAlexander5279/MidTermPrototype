using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveManager : MonoBehaviour
{


    [SerializeField] GameObject[] positions;
    [SerializeField] GameObject[] enemies;



    [SerializeField] int WaveEnemyCount;
    [SerializeField] int EnemiesSpawned;
    public int AliveEnemyCount;
    [SerializeField] int wave;
    public bool isWaveOver;
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] int waveBreakTimer;

    // Start is called before the first frame update
    private void Start()
    {
        wave = 0;
        isWaveOver = false;

    }

    // Update is called once per frame
    void Update()
    {


        //checks if wave is over, and if enemies need to be spawned
        if(!isWaveOver && EnemiesSpawned < WaveEnemyCount)
        {
           StartCoroutine(spawnEnemies());
        }

        //checks if any enemies are alive
        //if(AliveEnemyCount == 0)
        if (gameManager.instance.enemyCount == 0)
        {
            isWaveOver = true;
            StartCoroutine(waveBreak());
        }
    }


    IEnumerator spawnEnemies()
    {

        int indexEnemy = Random.Range(0, enemies.Length );
        int indexPos = Random.Range(0, positions.Length );
        //spawns random enemy in a random location
       
        Instantiate(enemies[indexEnemy], positions[indexPos].transform);
        
        //increments the number of enemies that have been spawned
        EnemiesSpawned++;

        //waits x amount of time in between each spawn
        yield return new WaitForSeconds(timeBetweenSpawns);
    }

    IEnumerator waveBreak()
    {
        //THIS IS WHEN THE SHOP CAN BE ACCESSED


        //waits x amount of time, then resets variables for new wave
        yield return new WaitForSeconds(waveBreakTimer);
        EnemiesSpawned = 0;
        //increments enemy o
        WaveEnemyCount += 3;
        isWaveOver = false;
      
    }


}

