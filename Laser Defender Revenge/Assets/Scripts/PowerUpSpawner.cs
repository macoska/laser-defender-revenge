using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] float globalSpawnProbability = 0.05f;

    [Header("Power Ups")]
    [SerializeField] PowerUp[] powerUps = default;
    [SerializeField] [Range(0f, 1f)] float[] spawnProbabilities = default;

    Vector2[] spawnRange;

    void OnValidate() // Resize inspector Array lengths
    {
        // Resize arrays
        int powerUpCount = powerUps.Length;
        if (powerUpCount > 0)
            Array.Resize(ref spawnProbabilities, powerUpCount);

        // Verify: SUM(probabilites) = 1
        if (powerUpCount > 0)
        {
            float[] spawnProbabilitiesCache = new float[powerUpCount];
            float sum = 0f;

            for (int slot = 0; slot < powerUpCount-1; slot++)
            {
                float probability = spawnProbabilities[slot];
                sum += probability;
                if (sum <= 1f)
                    spawnProbabilitiesCache[slot] = probability;
                else
                {
                    float remainder = Mathf.Clamp(1f-(sum-probability), 0f, 1f);
                    spawnProbabilitiesCache[slot] = remainder;
                    sum = 1f;
                }
            }
            if (powerUpCount > 1)
                spawnProbabilitiesCache[powerUpCount-1] = 1f-sum;
            else if (powerUpCount == 1)
                spawnProbabilitiesCache[0] = 1f;
            
            for (int slot = 0; slot < powerUpCount; slot++)
                spawnProbabilities[slot] = spawnProbabilitiesCache[slot];
        }
    }

    private void Start()
    {
        if (powerUps.Length == 0) { return; }
        CalculateSpawnRange();
    }

    private void CalculateSpawnRange()
    {
        spawnRange = new Vector2[powerUps.Length];
        float a = 0f, b = 0f;
        for (int slot = 0; slot < powerUps.Length; slot++)
        {
            float probability = spawnProbabilities[slot];
            b += probability;
            spawnRange[slot].x = a;
            spawnRange[slot].y = slot == powerUps.Length-1 ? 1f : b;
            a += probability;
        }
    }

    public void NotifyEnemyKilled(Vector2 position)
    {
        if (powerUps.Length == 0) { return; }
        bool shouldPOWSpawn = CheckSpawnPOW(globalSpawnProbability);
        if (shouldPOWSpawn)
        {
            PowerUp powerUp = ChoosePOW();
            SpawnPOW(powerUp,position);
        }
    }

    public void NotifyAsteroidKilled(Vector2 position) // Asteroids have *2 probability to spawn a POW
    {
        if (powerUps.Length == 0) { return; }
        bool shouldPOWSpawn = CheckSpawnPOW(globalSpawnProbability*2);
        if (shouldPOWSpawn)
        {
            PowerUp powerUp = ChoosePOW();
            SpawnPOW(powerUp,position);
        }
    }

    public PowerUp NotifyBoss(Vector2 position) // Boss has 100% droprate
    {
        if (powerUps.Length == 0) { return null; }
        PowerUp powerUp = ChoosePOW();
        return NotifyBoss(position,powerUp);
    }

    public PowerUp NotifyBoss(Vector2 position, PowerUp powerUp) // Boss has 100% droprate
    {
        return SpawnPOW(powerUp,position);
    }

    private PowerUp ChoosePOW()
    {
        float factor = UnityEngine.Random.value;
        for (int slot = 0; slot < powerUps.Length; slot++)
        {
            Vector2 range = spawnRange[slot];
            if (isWithinRange(range, factor))
                return powerUps[slot];
        }

        return powerUps[0]; // default, but should never happen
    }

    private bool isWithinRange(Vector2 range, float factor)
    {
        return range.x <= factor && factor <= range.y;
    }

    private bool CheckSpawnPOW(float probability)
    {
        float factor = UnityEngine.Random.value;
        bool shouldPOWSpawn = probability >= factor;
        //Debug.Log("Factor: " + factor + ", should Spawn: " + shouldPOWSpawn + " (" + probability + ")");
        return shouldPOWSpawn;
    }

    private PowerUp SpawnPOW(PowerUp powerUp, Vector2 position)
    {
        return Instantiate(powerUp, position, Quaternion.identity);
    }
}
