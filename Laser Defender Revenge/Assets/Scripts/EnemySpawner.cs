using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // configuration parameters
    [SerializeField] List<WaveConfig> waveConfigs = default;

    // state variables
    int waveIndex = 0;
    bool spawningCoroutineRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartNextWave();
    }

    public void NotifyEnemiesClear()
    {
        if(!spawningCoroutineRunning)
            StartNextWave();
    }

    private void StartNextWave()
    {
        spawningCoroutineRunning = true; // hotfix: Sometimes this function gets called twice, cannot see the mistake I made
        if (waveIndex < waveConfigs.Count)
            StartCoroutine(SpawnAllWaves());
        else
            FindObjectOfType<Level>().LoadNextLevel();
    }
    
    private IEnumerator SpawnAllWaves()
    {
        spawningCoroutineRunning = true;
        WaveConfig currentWave;
        do
        {
            currentWave = waveConfigs[waveIndex];

            if (currentWave.GetEnemyPrefab().tag == "Asteroid")
            {
                if (currentWave.WaitUntilClear())
                    yield return FindObjectOfType<AsteroidSpawner>().EnterAsteroidBelt(currentWave);
                else
                    yield return StartCoroutine(FindObjectOfType<AsteroidSpawner>().EnterAsteroidBelt(currentWave));
            }
            else
            {
                yield return StartCoroutine(SpawnAllEnemiesInWave(currentWave));
            }


            waveIndex++;
        } while (waveIndex < waveConfigs.Count && !currentWave.WaitUntilClear());
        spawningCoroutineRunning = false;

        // bug fix: Check if all enemies were killed while waiting
        if (FindObjectsOfType<Enemy>().Length == 0)
            NotifyEnemiesClear();
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        for (int enemyCount = 0; enemyCount < waveConfig.GetNumberOfEnemies(); enemyCount++)
        {
            var newEnemy = Instantiate(
                waveConfig.GetEnemyPrefab(),
                waveConfig.GetWaypoints()[0].transform.position,
                Quaternion.identity
            );
            newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);
            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
        }
    }
}
