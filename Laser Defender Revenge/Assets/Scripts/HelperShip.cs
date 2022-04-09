using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperShip : MonoBehaviour
{
    public enum Orientation{Left, Right, Top, Bottom, Null};

    [SerializeField] int health = 1;
    [SerializeField] float playerOffset = 1f;

    [Header("Projectile")]
    [SerializeField] GameObject rocketPrefab = default;
    [SerializeField] float rocketFiringPeriod = 3f;

    [Header("SFVX")]
    [SerializeField] GameObject deathVFX = default;
    [SerializeField] AudioClip deathSFX = default;
    [SerializeField] [Range(0,1)] float deathSFXVolume = 1f;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip shootSFX = default;
    [SerializeField] [Range(0,1)] float shootSFXVolume = 0.05f;

    // state variables
    Vector2 padding;
    Orientation orientation = Orientation.Null;

    // cached references
    Transform playerPos;

    // Awake is called before Start()
    void Awake() => DontDestroyOnLoad(gameObject);

    // Start is called before the first frame update
    void Start()
    {
        SetUpPlayerDocking();
        playerPos = FindObjectOfType<Player>().gameObject.transform;

        StartCoroutine(FireContinuously());
    }

    public Orientation GetOrientation() => orientation;

    private IEnumerator FireContinuously()
    {
        while(true)
        {
            yield return new WaitForSeconds(rocketFiringPeriod);
            FireRocket();
        }
    }

    private void FireRocket()
    {
        if (FindObjectOfType<Enemy>() || FindObjectOfType<Boss>()) // only shoot rocket when enemies are present
        {
            Instantiate(rocketPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSFXVolume);
        }
    }

    private void SetUpPlayerDocking()
    {
        int helperShipsCount = FindObjectsOfType<HelperShip>().Length;
        bool[] dockFull = new bool[5];
        foreach (var helperShip in FindObjectsOfType<HelperShip>())
        {
            Orientation shipOrientation = helperShip.GetOrientation();
            dockFull[(int)shipOrientation] = true;
        }
        for (int dock = 0; dock < 5; dock++)
        {
            if (!dockFull[dock])
            {
                orientation = (Orientation)dock;
                SetUpPadding();
                return;
            }
        }

        // default:
        orientation = Orientation.Null;
        SetUpPadding();
    }

    private void SetUpPadding()
    {
        switch (orientation)
        {
            case Orientation.Left:
                padding.x = -playerOffset;
                padding.y = 0f;
                break;
            case Orientation.Right:
                padding.x = playerOffset;
                padding.y = 0f;
                break;
            case Orientation.Top:
                padding.x = 0f;
                padding.y = playerOffset;
                break;
            case Orientation.Bottom:
                padding.x = 0f;
                padding.y = -playerOffset;
                break;
            case Orientation.Null:
                Debug.Log("Too many helper ships. Skipping");
                Destroy(gameObject);
                break;
            default:
                Debug.Log("Something went wrong when assigning helpership orientation");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (playerPos)
        {
            Vector2 position = playerPos.position;
            position += padding;
            transform.position = position;
        }
        else
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        int damage = damageDealer.GetDamage();
        health -= damage;
        damageDealer.Hit();
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(explosion, durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
    }
}
