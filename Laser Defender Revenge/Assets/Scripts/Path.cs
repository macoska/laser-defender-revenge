using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Path : MonoBehaviour
{
    enum Behaviour{Repeat, Reverse, Standby, Die} // behaviour at last waypoint

    [SerializeField] [Range(0.1f, 15f)] float[] transitionMoveSpeed;
    [SerializeField] [Range(20f, 360f)] float[] transitionRotateSpeed;
    [SerializeField] Behaviour behaviour = Behaviour.Standby;

    // state variables
    List<Waypoint> waypoints;
    int currentWaypoint = 1;

    void OnValidate() // Resize inspector Array lengths
    {
        int childCount = transform.childCount;
        if (childCount == 0)
        {
            Array.Resize(ref transitionMoveSpeed, 0);
            Array.Resize(ref transitionRotateSpeed, 0);
            return;
        }
        switch (behaviour)
        {
            case Behaviour.Repeat:
                Array.Resize(ref transitionMoveSpeed, childCount);
                Array.Resize(ref transitionRotateSpeed, childCount);
                break;
            case Behaviour.Reverse:
            case Behaviour.Standby:
            case Behaviour.Die:
                Array.Resize(ref transitionMoveSpeed, childCount-1);
                Array.Resize(ref transitionRotateSpeed, childCount-1);
                break;
            default:
                Debug.LogWarning("Behaviour not set (" + gameObject.name + ")");
                break;
        }
    }

    private void Start()
    {
        waypoints = GetWaypoints();
    }

    private List<Waypoint> GetWaypoints()
    {
        var waypoints = new List<Waypoint>();
        foreach (Transform child in transform)
            waypoints.Add(child.gameObject.GetComponent<Waypoint>());
        return waypoints;
    }

    public void SetNextWaypoint()
    {
        if (waypoints.Count == 0) { return; }

        if (currentWaypoint < waypoints.Count)
            currentWaypoint++;
        else if (currentWaypoint == waypoints.Count)
        {
            switch (behaviour)
            {
                case Behaviour.Repeat:
                    currentWaypoint = 1;
                    break;
                case Behaviour.Reverse:
                    currentWaypoint = 1;
                    ReverseWaypoints();
                    break;
                case Behaviour.Standby:
                case Behaviour.Die:
                    currentWaypoint++;
                    break;
                default:
                    Debug.LogWarning("Behaviour not set (" + gameObject.name + ")");
                    break;
            }
        }
    }

    private void ReverseWaypoints()
    {
        waypoints.Reverse();
        Array.Reverse(transitionMoveSpeed);
        Array.Reverse(transitionRotateSpeed);
    }

    public Waypoint GetCurrentWaypoint()
    {
        Waypoint waypoint = null;
        if (currentWaypoint <= waypoints.Count)
            waypoint = GetWaypoint(currentWaypoint);
        return waypoint;
    }
    private Waypoint GetWaypoint(int index) => waypoints[index-1];
    public float GetCurrentTransitionMoveSpeed() => GetCurrentTransitionSpeed(transitionMoveSpeed);
    public float GetCurrentTransitionRotateSpeed() => GetCurrentTransitionSpeed(transitionRotateSpeed);
    private float GetCurrentTransitionSpeed(float[] transitionSpeed)
    {
        if (waypoints.Count == 0) { return 0f; }

        float speed = 0f;
        if (currentWaypoint > waypoints.Count)
            speed = 0f;
        else
        {
            // Debug.Log("Current waypoint: " + currentWaypoint + " (Max: " + waypoints.Count + ")");
            switch (behaviour)
            {
                case Behaviour.Repeat:
                    if (currentWaypoint == 1)
                        speed = transitionSpeed[waypoints.Count - 1];
                    else
                        speed = transitionSpeed[currentWaypoint - 2];
                    break;
                default:
                    if (currentWaypoint == 1) // may need special treatment for case Behaviour.Reverse
                        speed = 0f;
                    else
                        speed = transitionSpeed[currentWaypoint - 2];
                    break;
            }
        }

        return speed;
    }
    public float GetCurrentWaitingTime() => GetCurrentWaypoint().GetWaitDuration();
    public Transform GetCurrentTransform() => GetCurrentWaypoint().transform;

    public bool DieAtEndpoint() => behaviour == Behaviour.Die;
}
