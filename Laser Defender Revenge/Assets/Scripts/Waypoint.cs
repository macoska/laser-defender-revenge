using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] float waitDuration = 0f;

    public float GetWaitDuration() => waitDuration;
}
