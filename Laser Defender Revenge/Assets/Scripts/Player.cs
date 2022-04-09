using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] bool godMode = false;

    // configuration parameters
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 1f;
    [SerializeField] int health = 200, maxHealth = 600;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab = default;
    [SerializeField] GameObject laserBigPrefab = default;
    [SerializeField] GameObject rocketPrefab = default;
    [SerializeField] GameObject helperShipPrefab = default;
    [SerializeField] GameObject ultraBeamPrefab = default;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.15f;
    [SerializeField] [Range(0,5)] int maxRockets = 5;
    [SerializeField] bool ultraBeamReady = false;

    [Header("SFVX")]
    [SerializeField] GameObject deathVFX = default;
    [SerializeField] AudioClip deathSFX = default;
    [SerializeField] [Range(0,1)] float deathSFXVolume = 1f;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip shootSFX = default;
    [SerializeField] [Range(0,1)] float shootSFXVolume = 0.05f;

    Coroutine firingCoroutine, ultraBeamCoroutine;

    // state variables
    float xMin, xMax, yMin, yMax; // Play space boundaries
    int rockets = 1;
    float normalProjectileSpeed;
    float normalProjectileFiringPeriod;
    GameObject normalLaserPrefab;
    float tiltAngle = 0f;


    // Awake is called before Start()
    void Awake() => SetUpSingleton();

    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
            DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();

        normalProjectileSpeed = projectileSpeed;
        normalProjectileFiringPeriod = projectileFiringPeriod;
        normalLaserPrefab = laserPrefab;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    public int GetHealth() => health;

    public int GetMaxRockets() => maxRockets;

    public int GetRockets() => rockets;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if PowerUp
        PowerUp powerUp = other.gameObject.GetComponent<PowerUp>();
        if (powerUp)
        {
            ObtainPowerUp(powerUp);
            Destroy(other.gameObject);
            return;
        }

        // Must be a projectile
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ObtainPowerUp(PowerUp powerUp)
    {
        var POW_ID = powerUp.GetPowerUp();
        switch (POW_ID)
        {
            case PowerUpID.FasterFiring:
                StartCoroutine(POW_FasterFiring());
                break;
            case PowerUpID.StrongLaser:
                StartCoroutine(POW_StrongLaser());
                break;
            case PowerUpID.Rocket:
                if (rockets < maxRockets)
                {
                    rockets++;
                    FindObjectOfType<RocketDisplay>().Add1Rocket();
                    FindObjectOfType<InfoBox>().ShowRocketInfo();
                }
                break;
            case PowerUpID.UltraBeam:
                ultraBeamReady = true;
                FindObjectOfType<UltraBeamDisplay>().Show(true);
                FindObjectOfType<InfoBox>().ShowUltraBeamInfo();
                break;
            case PowerUpID.HelperShip:
                Instantiate(helperShipPrefab, transform.position, Quaternion.identity);
                break;
            case PowerUpID.EMP:
                POW_EMP();
                break;
            case PowerUpID.Health:
                health = Mathf.Clamp(health + 200, 0, maxHealth);
                break;
            default:
                Debug.Log("Cannot recognize Power Up");
                break;
        }
    }

    private void POW_EMP()
    {
        var enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            Rigidbody2D rigidbody = enemy.GetComponent<Rigidbody2D>();
            rigidbody.bodyType = RigidbodyType2D.Dynamic;

            // Push enemy slightly away from Player
            float pushVel = 2f; // magic number
            float angleVel = -30f; // magic number
            if (enemy.transform.position.x < transform.position.x)
            {
                pushVel *= -1;
                angleVel *= -1;
            }
            rigidbody.velocity = new Vector2(pushVel,0f);
            rigidbody.angularVelocity = angleVel;

            // disable enemy pathing
            enemy.GetComponent<EnemyPathing>().enabled = false;
        }
    }

    private IEnumerator POW_FasterFiring()
    {
        // PowerUp Settings
        float POW_projectileFiringPeriod = Mathf.Clamp(projectileFiringPeriod / 2, normalProjectileFiringPeriod / 2, normalProjectileFiringPeriod);
        float POW_projectileSpeed = Mathf.Clamp(projectileSpeed * 2, normalProjectileSpeed, normalProjectileSpeed * 2);
        float POW_duration = 10f; // magic number

        // Apply Power Up
        projectileFiringPeriod = POW_projectileFiringPeriod;
        projectileSpeed = POW_projectileSpeed;

        // Revert to old settings
        yield return new WaitForSeconds(POW_duration);
        projectileFiringPeriod = normalProjectileFiringPeriod;
        projectileSpeed = normalProjectileSpeed;
    }

    private IEnumerator POW_StrongLaser()
    {
        // POW Settings
        float POW_projectileFiringPeriod = Mathf.Clamp(projectileFiringPeriod * 2, normalProjectileFiringPeriod, normalProjectileFiringPeriod * 2);
        float POW_projectileSpeed = projectileSpeed;
        float POW_duration = 10f; // magic number

        // Apply Power Up
        projectileFiringPeriod = POW_projectileFiringPeriod;
        projectileSpeed = POW_projectileSpeed;
        laserPrefab = laserBigPrefab;

        // Revert to old settings
        yield return new WaitForSeconds(POW_duration);
        projectileFiringPeriod = normalProjectileFiringPeriod;
        projectileSpeed = normalProjectileSpeed;
        laserPrefab = normalLaserPrefab;
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        int damage = Mathf.Clamp(damageDealer.GetDamage(), 0, health);
        if (godMode) damage = 0;
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
        FindObjectOfType<Level>().GetComponent<Level>().LoadGameOver();
    }

    public bool isUltraBeamReady() => ultraBeamReady;

    private void Fire()
    {
        if (Input.GetButtonDown("FireLaser") && firingCoroutine == null && ultraBeamCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("FireLaser") && firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }

        if (Input.GetButtonDown("FireRocket") && rockets > 0 && ultraBeamCoroutine == null)
            FireRocket();
        
        if (Input.GetButtonDown("FireUltraBeam") && ultraBeamReady && ultraBeamCoroutine == null)
        {
            if (firingCoroutine != null)
            {
                StopCoroutine(firingCoroutine);
                firingCoroutine = null;
            }
            ultraBeamCoroutine = StartCoroutine(FireUltraBeam());
            ultraBeamReady = false;
        }
    }

    private IEnumerator FireUltraBeam()
    {
        FindObjectOfType<UltraBeamDisplay>().Show(false);
        FindObjectOfType<InfoBox>().ClearUltraBeamInfo();
        GameObject ultraBeam = Instantiate(ultraBeamPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(ultraBeam.GetComponent<UltraBeam>().GetDuration());
        Destroy(ultraBeam);
        ultraBeamCoroutine = null;
    }

    private void FireRocket()
    {
        if (FindObjectOfType<Enemy>() || FindObjectOfType<Boss>()) // only shoot rocket when enemies are present
        {
            FindObjectOfType<RocketDisplay>().Decrease1Rocket();
            FindObjectOfType<InfoBox>().ClearRocketInfo();
            Instantiate(rocketPrefab,transform.position,transform.rotation);
            AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSFXVolume);
            rockets--;
        }
    }

    IEnumerator FireContinuously()
    {
        while(true)
        {
            GameObject laser = Instantiate(
                            laserPrefab,
                            transform.position,
                            transform.rotation) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = transform.rotation * new Vector2(0, projectileSpeed);
            AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSFXVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void Move()
    {
        // Obtain force
        var fx = Input.GetAxis("Horizontal");
        var fy = Input.GetAxis("Vertical");

        // Normalize force to ||F||<=1
        var norm = Mathf.Sqrt(fx*fx + fy*fy);
        if (norm > 1f) // Reminder: Input.GetAxis smooths the input
        {
            fx = fx/norm;
            fy = fy/norm;
        }

        // Obtain force
        var deltaX = fx * Time.deltaTime * moveSpeed;
        var deltaY = fy * Time.deltaTime * moveSpeed;
        
        // Update position by force
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
        //transform.Translate(deltaPos);

        // Set rotation
        Quaternion targetRotation = Quaternion.AngleAxis(tiltAngle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 60f*Time.deltaTime);
    }

    public void SetTiltAngle(float tiltAngle) => this.tiltAngle = tiltAngle;
    public float GetTiltAngle() => tiltAngle;

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0,0,0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1,0,0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0,0,0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0,1,0)).y - padding;
    }
}
