using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyPathing : MonoBehaviour
{
    // state variables
    WaveConfig waveConfig;
    List<Transform> waypoints;
    int waypointIndex = 0;
    bool isInverted = false;
    float[] waypointDelays;
    float oldTimestamp, currentDelay;

    // Start is called before the first frame update
    void Start()
    {
        waypoints = waveConfig.GetWaypoints();
        waypointDelays = waveConfig.GetDelays();
        waypointIndex = 0;
        oldTimestamp = Time.time;
        currentDelay = waypointDelays[waypointIndex];
        transform.position = waypoints[waypointIndex].transform.position;
    }

    // Update is called once per frame
    void Update() => Move();

    public void SetWaveConfig(WaveConfig waveConfig) => this.waveConfig = waveConfig;

    private void Move()
    {
        if (IsWaypointAhead())
            Steer2Waypoint();
        else
            if (waveConfig.IsLoop())
                InvertPath();
            else
                DestroyEnemy();
    }

    private bool IsWaypointAhead()
    {
        bool isWaypointAhead = false;
        if (isInverted) // exclude "out-of-playspace"-waypoint
            isWaypointAhead = waypointIndex <= waypoints.Count - 2;
        else
            isWaypointAhead = waypointIndex <= waypoints.Count - 1;
        return isWaypointAhead;
    }

    private void InvertPath()
    {
        waypoints.Reverse();
        Array.Reverse(waypointDelays);
        isInverted = !isInverted;
        waypointIndex = 1; // We are currently at index 0, therefore must be 1
    }

    private void Steer2Waypoint()
    {
        float waitedTime = Time.time - oldTimestamp;
        if (waitedTime > currentDelay)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards
                (transform.position, targetPosition, movementThisFrame);

            if (transform.position == targetPosition)
            {
                currentDelay = waypointDelays[waypointIndex];
                oldTimestamp = Time.time;
                waypointIndex++;
            }
        }
    }

    private void DestroyEnemy()
    {
        // Destroy Enemy
        gameObject.SetActive(false);
        Destroy(gameObject);

        // Sink Audio
        BossMusicPlayer bossMusicPlayer = GetComponent<BossMusicPlayer>();
        if (bossMusicPlayer) bossMusicPlayer.TurnOffBossMusic();

        // Notify if enemies clear
        if (FindObjectsOfType<Enemy>().Length == 0)
            FindObjectOfType<EnemySpawner>().NotifyEnemiesClear();
    }
}
