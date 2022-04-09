using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] int health = 200;
    [SerializeField] int score = 10;

    [Header("Child Asteroid Settings")]
    [SerializeField] Asteroid[] childAsteroidPrefabs = default;
    [SerializeField] [Range(0f, 1f)] float childSpawnProbability = 0.3f;
    [SerializeField] [Range(0, 4)] int numberChildsMin = 1, numberChildsMax = 4; 
    [SerializeField] [Range(0f,1f)] float childOffset = 0.3f;
    [SerializeField] [Range(0f,0.9f)] float speedRandomMultiplier = 0.5f;

    [Header("SFVX")]
    [SerializeField] GameObject deathVFX = default;
    [SerializeField] AudioClip deathSFX = default;
    [SerializeField] [Range(0,1)] float deathSFXVolume = 0.75f;
    [SerializeField] float durationOfExplosion = 1f;

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
            Die();
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(score);
        SpawnChildsOnChance();

        // Notify PowerUp-Spawner
        FindObjectOfType<PowerUpSpawner>().NotifyAsteroidKilled(transform.position);

        // Play SVFX
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
        Destroy(explosion, durationOfExplosion);
        
        // Destroy
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void SpawnChildsOnChance()
    {
        if (childAsteroidPrefabs.Length == 0)
            return;

        // Randomly decide if childs should be spawned
        float spawnFactor = UnityEngine.Random.value;
        if (spawnFactor > childSpawnProbability)
            return;
        
        // Randomly choose between asteroids to spawn
        int childsToSpawn = UnityEngine.Random.Range(numberChildsMin, numberChildsMax);

        // Spawn childs
        for (int child = 0; child < childsToSpawn; child++)
        {
            // Spawn Position based on #childs
            float spawnAngle = 360f * child / childsToSpawn;
            Quaternion spawnOffsetDirection = Quaternion.Euler(0f, 0f, spawnAngle);
            Vector3 spawnOffset = spawnOffsetDirection * (new Vector3(childOffset, 0f, 0f));
            Vector3 spawnPosition = transform.position + spawnOffset;

            // Create asteroid
            var childPrefab = childAsteroidPrefabs[UnityEngine.Random.Range(0,childAsteroidPrefabs.Length)];
            var childAsteroid = Instantiate(
                childPrefab,
                spawnPosition,
                Quaternion.identity);

            // Set velocity
            float velocityMultiplier = 1 + UnityEngine.Random.Range(-speedRandomMultiplier, speedRandomMultiplier);
            Vector2 velocity = GetComponent<Rigidbody2D>().velocity * velocityMultiplier;
            if (90f < spawnAngle && spawnAngle < 270f)
                velocity += Vector2.left * Mathf.Abs(velocity.y)*0.5f; // magic number
            else
                velocity += Vector2.right * Mathf.Abs(velocity.y)*0.5f; // magic number
            childAsteroid.GetComponent<Rigidbody2D>().velocity += velocity;
        }
    }
}
