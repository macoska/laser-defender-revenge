using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    // Configuration parameter
    [SerializeField] Asteroid[] asteroidPrefabs = default;
    [SerializeField] float paddingX = 1f, paddingY = 1f;

    // state variables
    float spawnPositionY, spawnPositionXMin, spawnPositionXMax;
    int numberOfAsteroids;
    float fallingSpeed, speedRandomFactor;
    float timeBetweenSpawns, spawnRandomFactor;

    // // Awake is called before Start()
    // void Awake()
    // {
    //     SetUpSingleton();
    // }

    // private void SetUpSingleton()
    // {
    //     if (FindObjectsOfType<AsteroidSpawner>().Length > 1)
    //         Destroy(gameObject);
    //     else
    //         DontDestroyOnLoad(gameObject);
    // }

    private void Start() {
        SetUpSpawnPosition();
    }

    public IEnumerator EnterAsteroidBelt(WaveConfig wave)
    {
        // Unzip wave information
        numberOfAsteroids = wave.GetNumberOfEnemies();
        fallingSpeed = wave.GetMoveSpeed();
        speedRandomFactor = wave.GetMoveSpeedRandomFactor();
        timeBetweenSpawns = wave.GetTimeBetweenSpawns();
        spawnRandomFactor = wave.GetSpawnRandomFactor();

        for (int i = 0; i < numberOfAsteroids; i++)
        {
            // Choose asteroid
            var asteroidPrefab = asteroidPrefabs[UnityEngine.Random.Range(0,asteroidPrefabs.Length)];

            // Instantiate asteroid
            var asteroid = Instantiate(asteroidPrefab, GetRandomSpawnPoint(), Quaternion.identity);
            float vel = fallingSpeed + UnityEngine.Random.Range(-speedRandomFactor, speedRandomFactor);
            asteroid.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -vel);

            // Wait before next asteroid
            float waitingTime = timeBetweenSpawns + UnityEngine.Random.Range(-spawnRandomFactor, spawnRandomFactor);
            yield return new WaitForSeconds(waitingTime);
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        float spawnPositionX = UnityEngine.Random.Range(spawnPositionXMin, spawnPositionXMax);
        Vector3 spawnPosition = new Vector3(spawnPositionX, spawnPositionY, 0);
        return spawnPosition;
    }

    private void SetUpSpawnPosition()
    {
        Camera gameCamera = Camera.main;
        spawnPositionXMin = gameCamera.ViewportToWorldPoint(new Vector3(0,0,0)).x + paddingX;
        spawnPositionXMax = gameCamera.ViewportToWorldPoint(new Vector3(1,0,0)).x - paddingX;
        spawnPositionY    = gameCamera.ViewportToWorldPoint(new Vector3(0,1,0)).y + paddingY;
    }
}
