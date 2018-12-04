using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private List<List<Transform>> spawnPoints;
    [SerializeField]
    private int numberOfWaves;
    [SerializeField]
    private int currentWave;
    public List<GenericEnemyScript> listOfLiveEnemies;
    public List<GameObject> listOfDeadEnemies;
    public bool allDead;
    private bool delayedEnemyIsCountingDown = false;



    // Use this for initialization
    void Start()
    {
        spawnPoints = new List<List<Transform>>();
        for (int wave = 0; wave < transform.childCount; wave++)
        {
            Transform child = transform.GetChild(wave);

            spawnPoints.Add(new List<Transform>());
            for (int enemy = 0; enemy < child.childCount; enemy++)
            {
                spawnPoints[wave].Add(child.GetChild(enemy));
            }
        }
        numberOfWaves = transform.childCount;
        currentWave = 0;
        allDead = true;
    }

    public IEnumerator SpawnWaves()
    {
        if (listOfLiveEnemies.Count != 0)
        {
            for (int i = 0; i < listOfLiveEnemies.Count; i++)
            {
                if (listOfLiveEnemies[i].isDead == true)
                {
                    listOfDeadEnemies.Add(listOfLiveEnemies[i].gameObject);
                    listOfLiveEnemies.Remove(listOfLiveEnemies[i]);
                }
            }
        }
        else
        {
            if (delayedEnemyIsCountingDown == false)
            {
                allDead = true;
            }
        }

        while (currentWave < numberOfWaves && allDead == true)
        {
            allDead = false;
            // StartCoroutine(DelayAllDeadCheck(FindMaxSpawnDelay(currentWave)));
            for (int point = 0; point < spawnPoints[currentWave].Count; point++)
            {
                // print("current wave:" + currentWave + " and point: " + point);
                // float spawnDelay = spawnPoints[currentWave][point].GetComponent<SpawnPoint>().spawnDelay;
                // StartCoroutine(Spawn(currentWave, point, spawnDelay));
                GameObject enemy = spawnPoints[currentWave][point].GetComponent<SpawnPoint>().Spawn();
                listOfLiveEnemies.Add(enemy.GetComponent<GenericEnemyScript>());
            }
            currentWave++;
            yield return null;
        }

    }

    public bool IsDone()
    {
        return (numberOfWaves == currentWave && allDead);
    }

    public void Reset()
    {
        foreach (GameObject go in listOfDeadEnemies)
        {
            Destroy(go);
        }
        foreach (GenericEnemyScript go in listOfLiveEnemies)
        {
            Destroy(go.gameObject);
        }
        listOfDeadEnemies.Clear();
        listOfLiveEnemies.Clear();
        currentWave = 0;
        allDead = true;
    }

    private IEnumerator Spawn(int wave, int point, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject enemy = spawnPoints[currentWave][point].GetComponent<SpawnPoint>().Spawn();
        listOfLiveEnemies.Add(enemy.GetComponent<GenericEnemyScript>());
    }
    /*
    private float FindMaxSpawnDelay(int wave)
    {
        float max = 0;
        for (int point = 0; point < spawnPoints[wave].Count; point++)
        {
            float spawnDelay = spawnPoints[currentWave][point].GetComponent<SpawnPoint>().spawnDelay;
            if (spawnDelay > max)
            {
                max = spawnDelay;
            }

        }

        return max;
    }
    */
    private IEnumerator DelayAllDeadCheck(float maxDelay)
    {
        delayedEnemyIsCountingDown = true;
        yield return new WaitForSeconds(maxDelay);
        delayedEnemyIsCountingDown = false;
    }
}
