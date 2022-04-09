using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Enemy Wave Config")]
public class WaveConfig : ScriptableObject
{
    // configuration parameters
    [Header("Enemy Parameters")]
    [SerializeField] GameObject enemyPrefab = default;
    [SerializeField] float timeBetweenSpawns = 0.5f, spawnRandomFactor = 0.3f;
    [SerializeField] int numberOfEnemies = 5;
    [SerializeField] float moveSpeed = 2f, moveSpeedRandomFactor = 1f;

    [Header("Wave-type")]
    [SerializeField] GameObject pathPrefab = default;
    [SerializeField] [Range(0f, 30f)] float[] delayAtWaypointInSeconds;
    [SerializeField] bool isLoop = false, nextWaveUntilClear = false;

    void OnValidate() // Resize inspector Array lengths
    {
        if (pathPrefab)
        {
            int childCount = pathPrefab.transform.childCount;
            Array.Resize(ref delayAtWaypointInSeconds, childCount);
        }
    }

    // Get()
    public GameObject GetEnemyPrefab() => enemyPrefab;
    public List<Transform> GetWaypoints()
    {
        var waveWaypoints = new List<Transform>();
        foreach (Transform child in pathPrefab.transform)
        {
            waveWaypoints.Add(child);
        }
        return waveWaypoints;
    }
    public float[] GetDelays() => delayAtWaypointInSeconds;
    public float GetTimeBetweenSpawns() => timeBetweenSpawns;
    public float GetSpawnRandomFactor() => spawnRandomFactor;
    public int GetNumberOfEnemies() => numberOfEnemies;
    public float GetMoveSpeed() => moveSpeed;
    public float GetMoveSpeedRandomFactor() => moveSpeedRandomFactor;
    public bool IsLoop() => isLoop;
    public bool WaitUntilClear() => nextWaveUntilClear;
}