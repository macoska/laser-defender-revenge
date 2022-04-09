using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingProjectile : MonoBehaviour
{
    public enum aimObjectType{Enemy, Player};

    [SerializeField] aimObjectType targetType = aimObjectType.Player;
    [SerializeField] [Range(0f, 15f)] float speed = 2f, rotationSpeed = 3f;
    [SerializeField] [Range(2f, 15f)] float selfExplosionTimer = 10f;
    [SerializeField] int health = 200;
    [SerializeField] int score = 200;

    [Header("PowerUps")]
    [SerializeField] PowerUp POW = default;
    [SerializeField] [Range(0f, 1f)] float POW_droprate = 0.1f;

    [Header("SFVX")]
    [SerializeField] GameObject deathVFX = default;
    [SerializeField] AudioClip deathSFX = default;
    [SerializeField] [Range(0,1)] float deathSFXVolume = 0.75f;
    [SerializeField] float durationOfExplosion = 1f;

    // state variables
    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        SetTarget();
        StartCoroutine(StartSelfExplosionTimer(selfExplosionTimer));
    }

    // Update is called once per frame
    void Update()
    {
        if (!target)
            SetTarget();

        if (target)
            MoveToTarget();
        else
            Die();
    }

    private void MoveToTarget()
    {
        // Rotate
        Vector2 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // magic number
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        // Move
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    private void SetTarget()
    {
        if (targetType == aimObjectType.Enemy)
        {
            var targetBoss = (Boss)FindObjectOfType(typeof(Boss));
            var targetEnemy = (Enemy)FindObjectOfType(typeof(Enemy));
            if (targetBoss)
                target = targetBoss.gameObject;
            else if (targetEnemy)
                target = targetEnemy.gameObject;
        }
        else
        {
            var targetPlayer = (Player)FindObjectOfType(typeof(Player));
            if (targetPlayer)
                target = targetPlayer.gameObject;
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
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            FindObjectOfType<GameSession>().AddToScore(score);
            Die();
        }
    }

    private IEnumerator StartSelfExplosionTimer(float timer)
    {
        yield return new WaitForSeconds(timer);
        Die();
    }

    private void Die()
    {
        // Notify PowerUp-Spawner
        if (targetType == aimObjectType.Player)
        {
            if (gameObject.tag == "BossRocket" && UnityEngine.Random.value <= POW_droprate)
                    FindObjectOfType<PowerUpSpawner>().NotifyBoss(transform.position, POW);
            else
                FindObjectOfType<PowerUpSpawner>().NotifyEnemyKilled(transform.position);
        }

        // Play SVFX
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
        Destroy(explosion, durationOfExplosion);

        // Destroy Enemy
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
