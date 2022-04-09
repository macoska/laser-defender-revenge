using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public enum axis{x, y, z};

    [SerializeField] [Range(20f, 1000f)] float speedOfSpin = 50f;
    [SerializeField] axis spinAroundAxis = axis.z;

    // Update is called once per frame
    void Update()
    {
        float spin = speedOfSpin * Time.deltaTime;

        if (spinAroundAxis == axis.x)
            transform.Rotate(spin, 0, 0);
        else if (spinAroundAxis == axis.y)
            transform.Rotate(0, spin, 0);
        else
            transform.Rotate(0, 0, spin);
    }
}
