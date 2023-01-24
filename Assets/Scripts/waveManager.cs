using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveManager : MonoBehaviour
{


    [SerializeField] GameObject[] positions;
    [SerializeField] GameObject[] enemies;
    [SerializeField] GameObject[] bossList;
    [SerializeField] GameObject healthPickUp;



    [SerializeField] int WaveEnemyCount;
    [SerializeField] int WaveBossEnemyCount;
    [SerializeField] int EnemiesSpawned;
    public int AliveEnemyCount;
    [SerializeField] int wave;
    public bool isWaveOver;
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] float waveBreakTimer;
    public bool isSpawningEnemies;
    public bool isBossRound;
    public bool isHigherDiff;

    // Start is called before the first frame update
    private void Start()
    {
        wave = 0;
        gameManager.instance.addWaveCount(0);
        isWaveOver = false;          // set to true to allow some time before the first wave starts
        isSpawningEnemies = true;  // using this will allow the game to make use of the spawner delay
        isBossRound = false;
        isHigherDiff = false;
        StartCoroutine(waveBreak());

    }

    // Update is called once per frame
    void Update()
    {

        AliveEnemyCount = gameManager.instance.enemyCount;
        //checks if wave is over, and if enemies need to be spawned
        if (!isWaveOver && (wave > 0))
        {
            if (!isSpawningEnemies)
            {
                if ((wave > 15) && (wave % 5 != 0) && (EnemiesSpawned < (WaveEnemyCount + WaveBossEnemyCount))) //boss + regular enemies
                {
                    isHigherDiff = true;
                    isBossRound = false;
                    isSpawningEnemies = true;
                    StartCoroutine(spawnEnemies());
                    StartCoroutine(spawnBossEnemies());
                }
                else if ((wave % 5 == 0) && (EnemiesSpawned < WaveBossEnemyCount))  // boss round
                {
                    isHigherDiff = false;
                    isBossRound = true;
                    isSpawningEnemies = true;
                    StartCoroutine(spawnBossEnemies());
                }
                else if ((wave % 5 != 0) && EnemiesSpawned < WaveEnemyCount)    // regular sub-15 rounds
                {
                    isHigherDiff = false;
                    isBossRound = false;
                    isSpawningEnemies = true;
                    StartCoroutine(spawnEnemies());
                }

            }

        }

        //checks if any enemies are alive
        //if(AliveEnemyCount == 0)
        if (gameManager.instance.enemyCount == 0 && !isWaveOver)
        {
            if (isHigherDiff && !isBossRound)
            {
                if (EnemiesSpawned >= (WaveEnemyCount + WaveBossEnemyCount))
                {
                    isWaveOver = true;

                    StartCoroutine(waveBreak());

                }
            }
            else if (isBossRound)
            {
                if (EnemiesSpawned >=  WaveBossEnemyCount)
                {
                    isWaveOver = true;

                    //reward player with more health for beating a boss round
                    float healthPercent = gameManager.instance.playerScript.HP / gameManager.instance.playerScript.HPOriginal;
                    gameManager.instance.playerScript.HPOriginal = gameManager.instance.scalingFunction(gameManager.instance.playerScript.HPOriginal);
                    gameManager.instance.playerScript.HP = Mathf.FloorToInt(gameManager.instance.playerScript.HPOriginal * healthPercent);
                    Instantiate(healthPickUp, gameManager.instance.player.transform.position, gameManager.instance.player.transform.rotation);
                    StartCoroutine(waveBreak());

                }
            }
            else
            {
                if (EnemiesSpawned >= WaveEnemyCount)
                {
                    isWaveOver = true;

                    StartCoroutine(waveBreak());

                }
            }

        }
    }


    IEnumerator spawnEnemies()
    {

        int indexEnemy = Random.Range(0, enemies.Length);
        int indexPos = Random.Range(0, positions.Length);
        //spawns random enemy in a random location

        Instantiate(enemies[indexEnemy], positions[indexPos].transform.position, Quaternion.identity);

        //increments the number of enemies that have been spawned
        EnemiesSpawned++;

        //waits x amount of time in between each spawn
        yield return new WaitForSeconds(timeBetweenSpawns);
        isSpawningEnemies = false;
    }

    IEnumerator waveBreak()
    {
        //THIS IS WHEN THE SHOP CAN BE ACCESSED
        EnemiesSpawned = 0;
        //increments enemy o
        if (wave > 0)
        {
            WaveEnemyCount += Mathf.FloorToInt(gameManager.instance.scalingFunction(3 + Mathf.FloorToInt(wave * 0.2f)));
            WaveBossEnemyCount += Mathf.FloorToInt(gameManager.instance.scalingFunction(Mathf.FloorToInt(wave * 0.2f)));
        }
        //waits x amount of time, then resets variables for new wave
        yield return new WaitForSeconds(waveBreakTimer);

        wave++;
        gameManager.instance.addWaveCount(wave - gameManager.instance.waveCount);
        isSpawningEnemies = false;
        isWaveOver = false;

    }
    IEnumerator spawnBossEnemies()
    {

        int indexEnemy = Random.Range(0, bossList.Length);
        int indexPos = Random.Range(0, positions.Length);
        //spawns random enemy in a random location

        Instantiate(bossList[indexEnemy], positions[indexPos].transform.position, Quaternion.identity);

        //increments the number of enemies that have been spawned
        EnemiesSpawned++;

        //waits x amount of time in between each spawn
        yield return new WaitForSeconds(timeBetweenSpawns * 3.0f);

        isSpawningEnemies = false;
    }

}

