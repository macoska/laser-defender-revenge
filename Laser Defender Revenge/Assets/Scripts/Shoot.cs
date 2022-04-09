using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] float shootWidth = 0f;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab = default;
    [SerializeField] float minTimeBetweenShots = 0.2f, maxTimeBetweenShots = 2f;
    [SerializeField] float speed = 5f;

    [Header("SFX")]
    [SerializeField] AudioClip shootSFX = default;
    [SerializeField] [Range(0,1)] float shootSFXVolume = 0.2f;

    // state variables
    float shotCounter;

    public void SetProjectile(GameObject projectile) => projectilePrefab = projectile;
    public void SetProjectileSpeed(float speed) => this.speed = speed;
    public void SetProjectileShotTiming(float minTimeBetweenShots, float maxTimeBetweenShots)
    {
        this.minTimeBetweenShots = minTimeBetweenShots;
        this.maxTimeBetweenShots = maxTimeBetweenShots;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetShotCounter();
    }

    private void GetShotCounter()
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
            GetShotCounter();
        }
    }

    private void Fire()
    {
        if (!projectilePrefab) return;

        // Obtain position & rotation
        float width = UnityEngine.Random.Range(-shootWidth/2, shootWidth/2);
        float angle = transform.eulerAngles.z;
        Vector2 position = new Vector2(transform.position.x + width, transform.position.y);
        Quaternion rotation = Quaternion.AngleAxis(180f+angle, Vector3.forward);

        // Instantiate
        GameObject projectile = Instantiate(projectilePrefab, position, rotation) as GameObject;

        // assign velocity to object
        Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
        if (rigidbody)
            rigidbody.velocity = rotation * new Vector2(0f, speed);
        
        // SFX
        AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSFXVolume);
    }
}
