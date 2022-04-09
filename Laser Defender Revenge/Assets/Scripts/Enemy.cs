using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] int health = 100;
    [SerializeField] int score = 50;

    [Header("Shooting")]
    [SerializeField] GameObject projectile = default;
    [SerializeField] float minTimeBetweenShots = 0.2f, maxTimeBetweenShots = 3f;
    [SerializeField] float projectileSpeed = 5f;

    [Header("SFVX")]
    [SerializeField] GameObject deathVFX = default;
    [SerializeField] AudioClip deathSFX = default;
    [SerializeField] [Range(0,1)] float deathSFXVolume = 0.75f;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip shootSFX = default;
    [SerializeField] [Range(0,1)] float shootSFXVolume = 0.2f;

    // State variables
    float shotCounter;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -projectileSpeed);
        AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSFXVolume);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(score);

        // Notify PowerUp-Spawner
        FindObjectOfType<PowerUpSpawner>().NotifyEnemyKilled(transform.position);

        // Play SVFX
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
        Destroy(explosion, durationOfExplosion);

        // Destroy Enemy
        gameObject.SetActive(false);
        Destroy(gameObject);

        // Notify if enemies clear
        if (FindObjectsOfType<Enemy>().Length == 0)
            FindObjectOfType<EnemySpawner>().NotifyEnemiesClear();
    }

    
}
