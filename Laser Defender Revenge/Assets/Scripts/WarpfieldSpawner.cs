using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpfieldSpawner : MonoBehaviour
{
    // configuration parameters
    [SerializeField] GameObject warpfieldPrefab = default;
    [SerializeField] [Range(0.1f,90f)] float speedRotation = 20f;
    [SerializeField] float warpfieldOffAngle = 0f, warpfieldOnAngle = 40f;

    // state variables
    float radius;
    GameObject warpfield;

    // Awake is called before Start()
    void Awake()
    {
        SetUpSingleton();
    }

    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        GetRadius();
        //StartCoroutine(EnterWarpfield()); // debugging
    }

    private void GetRadius()
    {
        Camera gameCamera = Camera.main;
        float yMax = gameCamera.ViewportToWorldPoint(new Vector3(0,1,0)).y;
        radius = yMax + 2;
    }

    public IEnumerator EnterWarpfield()
    {
        // Check if warpfield already turned on
        if (!warpfield)
        {
            // Create warpfield
            warpfield = Instantiate(
                warpfieldPrefab,
                new Vector3(0,0,radius),
                Quaternion.Euler(warpfieldOffAngle,0f,0f));
            DontDestroyOnLoad(warpfield);

            // Start warpfield
            yield return MoveWarpfield(warpfieldOffAngle, warpfieldOnAngle);

            // Highlight particle trail glowing effect
            SetTrailHighlight(1f);
        } 
        else
        {
            Debug.Log("Warpfield already running!");
            yield break; // maybe should be "yield return null;"
        }
    }

    public IEnumerator ExitWarpfield()
    {
        // Check if warpfield already turned on
        if (warpfield)
        {
            // Remove highlights from particles
            SetTrailHighlight(0f);

            // Stop warpfield
            yield return MoveWarpfield(warpfieldOnAngle,warpfieldOffAngle);
            Destroy(warpfield);
        }
        else
        {
            Debug.Log("There is no warpfield to turn off!");
            yield break;
        }
    }

    private IEnumerator MoveWarpfield(float startAngle, float endAngle)
    {
        // Position
        Vector3  startPosition = warpfield.transform.position;
        Vector3  newPosition = startPosition;

        // Time
        float currentTime = 0f;
        float timeUntilFinished = Mathf.Abs(endAngle-startAngle)/speedRotation;

        // Loop
        while(currentTime <= timeUntilFinished)
        {
            // Lerp angle
            float ratio = currentTime / timeUntilFinished;
            float angle = Mathf.SmoothStep(startAngle, endAngle, ratio);
            float angleInRadians = Mathf.Deg2Rad * angle;

            // Rotate transform around pivot point
            newPosition.y = Mathf.Cos(angleInRadians)*startPosition.y + Mathf.Sin(angleInRadians)*startPosition.z;
            newPosition.z = -Mathf.Sin(angleInRadians)*startPosition.y + Mathf.Cos(angleInRadians)*startPosition.z;
            Quaternion newRotation = Quaternion.Euler(angle, 0f, 0f);

            // Set Pose
            warpfield.transform.position = newPosition;
            warpfield.transform.rotation = newRotation;

            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
    }

    private void SetTrailHighlight(float size)
    {
        ParticleSystem.MainModule pMain = warpfield.GetComponent<ParticleSystem>().main;
        pMain.startSize = new ParticleSystem.MinMaxCurve(size, size);
    }

    public void KillWarpfield()
    {
        StopAllCoroutines();
        Destroy(warpfield,0.1f);
        //Destroy(gameObject);
    }
}