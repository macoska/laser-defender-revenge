using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] int startPath = 0;

    [Header("Boss Stats")]
    [SerializeField] int health = 300000;
    [SerializeField] int score  = 50000;
    [SerializeField] int[] healthForNextPath;

    [Header("Shooting")]
    [SerializeField] GameObject laser = default;
    [SerializeField] GameObject rocket = default;
    [SerializeField] float laserMinTimeBetweenShots = 0.2f, laserMaxTimeBetweenShots = 2f;
    [SerializeField] float rocketMinTimeBetweenShots = 1.5f, rocketMaxTimeBetweenShots = 3f;
    [SerializeField] float laserSpeed = 5f, rocketSpeed = 3f;

    [Header("SFVX")]
    [SerializeField] GameObject deathVFX = default;
    [SerializeField] AudioClip deathSFX = default;
    [SerializeField] [Range(0,1)] float deathSFXVolume = 0.75f;
    [SerializeField] float durationOfExplosion = 3f;

    [Header("PowerUps")]
    [SerializeField] PowerUp POW_Health = default;
    [SerializeField] PowerUp POW_UltraBeam = default;
    [SerializeField] [Range(0f, 180f)] float POW_HealthRate = 20f, POW_UltraBeamRate = 30f, POW_OtherRate = 60f;
    [SerializeField] float POW_offsetVariation = 5f;

    // cached variables
    BossPathing bossPathing;
    Player player;
    MusicPlayer musicPlayer;

    // state variables
    GameObject[] rocketCannons;
    GameObject[] laserCannons;
    PowerUpSpawner powerUpSpawner;
    int maxHealth;
    int state = 0;
    bool[] stateClear;
    

    void OnValidate() // Resize inspector Array lengths
    {
        int pathCount = GetComponent<BossPathing>().GetPathCount();
        Array.Resize(ref healthForNextPath, pathCount);
        if (pathCount > 0)
            healthForNextPath[0] = Mathf.Clamp(healthForNextPath[0], 0, health);
        for (int slot = 1; slot < pathCount; slot++)
        {
            healthForNextPath[slot] = Mathf.Clamp(healthForNextPath[slot], 0, healthForNextPath[slot-1]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set cached references
        powerUpSpawner = FindObjectOfType<PowerUpSpawner>();
        bossPathing = GetComponent<BossPathing>();
        player = FindObjectOfType<Player>();
        musicPlayer = FindObjectOfType<MusicPlayer>();

        // Set cannons
        rocketCannons = new GameObject[2];
        rocketCannons[0] = transform.Find("RocketCannonL").gameObject;
        rocketCannons[1] = transform.Find("RocketCannonR").gameObject;
        // SetCannons(rocketCannons, rocket, rocketSpeed, rocketMinTimeBetweenShots, rocketMaxTimeBetweenShots);
        SetCannons(rocketCannons, laser, laserSpeed, laserMinTimeBetweenShots, laserMaxTimeBetweenShots);
        laserCannons = new GameObject[3];
        laserCannons[0] = transform.Find("LaserCannonL").gameObject;
        laserCannons[1] = transform.Find("LaserCannonM").gameObject;
        laserCannons[2] = transform.Find("LaserCannonR").gameObject;
        SetCannons(laserCannons, laser, laserSpeed, laserMinTimeBetweenShots, laserMaxTimeBetweenShots);

        // Set variables
        maxHealth = health;
        state = 0;
        stateClear = new bool[bossPathing.GetPathCount()];

        // Advance to next states
        for (int i = 0; i < startPath; i++)
        {
            SetNextPath();
            health = healthForNextPath[state-1];
        }

        // Start powerUp spawn coroutines
        StartCoroutine(SpawnPOWCoroutine(POW_Health, POW_HealthRate));
        StartCoroutine(SpawnPOWCoroutine(POW_UltraBeam, POW_UltraBeamRate));
        StartCoroutine(SpawnPOWCoroutine(null, POW_OtherRate));

        // Start epic boss music
        if (musicPlayer) musicPlayer.SetBossVolumeOnlyHard();
    }

    private void SetCannons(GameObject[] cannons, GameObject projectile, float speed, float minTimeBetweenShots, float maxTimeBetweenShots)
    {
        foreach (var cannon in cannons)
        {
            cannon.GetComponent<Shoot>().SetProjectile(projectile);
            cannon.GetComponent<Shoot>().SetProjectileSpeed(speed);
            cannon.GetComponent<Shoot>().SetProjectileShotTiming(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private IEnumerator SpawnPOWCoroutine(PowerUp powerUp, float rate)
    {
        while (true)
        {
            yield return new WaitForSeconds(rate);
            SpawnPOW(powerUp);
        }
    }

    private void SpawnPOW(PowerUp powerUp)
    {
        Vector2 position = transform.position;
        position.x += UnityEngine.Random.Range(-POW_offsetVariation/2, POW_offsetVariation/2);
        PowerUp POW;
        if (powerUp)
            POW = powerUpSpawner.NotifyBoss(position, powerUp);
        else
            POW = powerUpSpawner.NotifyBoss(transform.position);

        if (45f <= transform.eulerAngles.z && transform.eulerAngles.z <= 135f)
            POW.SetMove(true, false);
        else if (135f <= transform.eulerAngles.z && transform.eulerAngles.z <= 225f)
        {
            POW.SetMove(false, true);
            POW.InvertY();
        }
        else
            POW.SetMove(false, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (StateClear())
            SetNextPath();
    }

    private void SetNextPath()
    {
        if (state == bossPathing.GetPathCount() - 1)
            return;
        state++;
        bossPathing.SetNextPath();
        switch (state)
        { // Define special states
            case 1:
                SetCannons(rocketCannons, laser, laserSpeed*2, laserMinTimeBetweenShots, laserMaxTimeBetweenShots);
                SetCannons(laserCannons, laser, laserSpeed, laserMinTimeBetweenShots, laserMaxTimeBetweenShots);
                break;
            case 2:
                SetCannons(rocketCannons, rocket, rocketSpeed, rocketMinTimeBetweenShots, rocketMaxTimeBetweenShots);
                SetCannons(laserCannons, laser, laserSpeed, laserMinTimeBetweenShots, laserMaxTimeBetweenShots);
                break;
            case 3:
                if(player) player.SetTiltAngle(90f);
                SetCannons(rocketCannons, rocket, rocketSpeed, rocketMinTimeBetweenShots, rocketMaxTimeBetweenShots);
                SetCannons(laserCannons, laser, laserSpeed, laserMinTimeBetweenShots*3, laserMaxTimeBetweenShots*1.5f);
                break;
            case 4:
                SetCannons(rocketCannons, rocket, rocketSpeed, rocketMinTimeBetweenShots, rocketMaxTimeBetweenShots);
                SetCannons(laserCannons, laser, laserSpeed, laserMinTimeBetweenShots*3, laserMaxTimeBetweenShots*1.5f);
                break;
            case 5:
                if(player) player.SetTiltAngle(180f);
                SetCannons(rocketCannons, rocket, rocketSpeed, rocketMinTimeBetweenShots, rocketMaxTimeBetweenShots);
                SetCannons(laserCannons, laser, laserSpeed, laserMinTimeBetweenShots*2, laserMaxTimeBetweenShots*1.5f);
                break;
            case 6:
                if(player) player.SetTiltAngle(0f);
                SetCannons(rocketCannons, rocket, rocketSpeed, rocketMinTimeBetweenShots, rocketMaxTimeBetweenShots);
                SetCannons(laserCannons, laser, laserSpeed, laserMinTimeBetweenShots, laserMaxTimeBetweenShots);
                break;
            default:
                SetCannons(rocketCannons, rocket, rocketSpeed, rocketMinTimeBetweenShots, rocketMaxTimeBetweenShots);
                SetCannons(laserCannons, laser, laserSpeed, laserMinTimeBetweenShots, laserMaxTimeBetweenShots);
                break;
        }
    }

    private bool StateClear()
    {
        bool clear = health <= healthForNextPath[state] && !stateClear[state];
        stateClear[state] = clear;
        return clear;
    }

    public int GetHealth() => health;
    public int GetMaxHealth() => maxHealth;

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

        // Play SVFX
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        explosion.transform.localScale = explosion.transform.localScale * 2f;
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
        Destroy(explosion, durationOfExplosion);

        // Sink Audio
        if (musicPlayer) musicPlayer.SetAmbientVolumeOnly();

        // Destroy Enemy
        gameObject.SetActive(false);
        Destroy(gameObject);

        // Load game over
        FindObjectOfType<Level>().LoadNextLevel();
    }
}
