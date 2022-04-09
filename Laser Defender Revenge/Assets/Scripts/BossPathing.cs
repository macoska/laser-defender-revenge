using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPathing : MonoBehaviour
{
    [SerializeField] List<Path> paths = default;
    [SerializeField] [Range(0.1f, 15f)] float pathTransitionMoveSpeed = 3f;
    [SerializeField] [Range(20f, 360f)] float pathTransitionRotateSpeed = 180f;

    // state variables
    int path = 0;
    Waypoint waypoint;
    float moveSpeed, rotateSpeed;
    float oldTimestamp, delay = -1f;

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentWaypoint();
        transform.position = waypoint.transform.position;
        transform.rotation = waypoint.transform.rotation;
    }

    private void SetUpDelay()
    {
        delay = paths[path].GetCurrentWaitingTime();
        oldTimestamp = Time.time;
    }

    private void SetCurrentWaypoint()
    {
        waypoint = paths[path].GetCurrentWaypoint();
        if (!waypoint)
        {
            // At last waypoint...
            if (paths[path].DieAtEndpoint()) // ...Die!
                Die();
            return; // ...Standby
        }
        moveSpeed = paths[path].GetCurrentTransitionMoveSpeed();
        rotateSpeed = paths[path].GetCurrentTransitionRotateSpeed();
    }

    private void SetNextWaypoint()
    {
        paths[path].SetNextWaypoint();
        SetCurrentWaypoint();
    }

    private void Die()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update() => Move();

    private void Move()
    {
        if (!waypoint)
            return;
        if (ReachedTarget())
        {
            if (delay == -1f)
                SetUpDelay();
            if (DelayTimeOver())
                SetNextWaypoint();
        }
        
        if (!waypoint)
            return;
        Steer2Waypoint();
    }

    private void Steer2Waypoint()
    {
        var targetPosition = waypoint.transform.position;
        var targetRotation = waypoint.transform.rotation;
        var movementThisFrame = moveSpeed * Time.deltaTime;
        var rotationThisFrame = rotateSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationThisFrame);
        // Debug.Log("Enter Steer2Waypoint: moveSpeed=" + moveSpeed + ", rotateSpeed=" + rotateSpeed);
        // Debug.Log("Enter Steer2Waypoint: movementThisFrame=" + movementThisFrame + ", rotationThisFrame=" + rotationThisFrame);
    }

    private bool DelayTimeOver()
    {
        float waitedTime = Time.time - oldTimestamp;
        bool delayOver = waitedTime > delay;
        if (delayOver)
            delay = -1f;
        return delayOver;
    }

    private bool ReachedTarget() => transform.position == waypoint.transform.position && transform.rotation == waypoint.transform.rotation;

    public void SetNextPath()
    {
        path++;
        if (path >= paths.Count)
        {
            Debug.LogError("There is no next path! Check your paths and path-continue-conditions!");
            Die();
            return;
        }
    
        SetCurrentWaypoint();
        moveSpeed = pathTransitionMoveSpeed;
        rotateSpeed = pathTransitionRotateSpeed;
        delay = -1f;
    }

    public int GetPathCount() => paths.Count;
}
