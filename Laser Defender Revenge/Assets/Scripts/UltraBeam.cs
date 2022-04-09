using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltraBeam : MonoBehaviour
{
    [SerializeField] float offset = 3f;
    [SerializeField] [Range(1f, 15f)] float scaleX = 2f, scaleY = 16f;
    [SerializeField] [Range(1f, 15f)] float duration = 5f;
    [SerializeField] GameObject tilePrefab = default; // includes damagedealer
    [SerializeField] float tileSpawnPeriod = 0.1f;
    [SerializeField] float tileSpeed = 40f, tileAngularSpeed = 180f;

    [Header("SFVX")]
    [SerializeField] AudioClip shootSFX = default;
    [SerializeField] [Range(0,1)] float shootSFXVolume = 0.1f;

    // state variables
    Vector2 padding;

    // cached references
    Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
        padding = new Vector2(0f, offset + scaleY/2f);
        playerTransform = FindObjectOfType<Player>().gameObject.transform;
        Move();

        // Play SFX
        // AudioSource audioSource = new AudioSource();
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.transform.parent = transform;
        audioSource.clip = shootSFX;
        audioSource.volume = shootSFXVolume;
        audioSource.loop = true;
        audioSource.Play();
        // AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSFXVolume);

        // Start Beam
        StartCoroutine(Fire());
        //Destroy(this.gameObject, duration);
    }

    private IEnumerator Fire()
    {
        AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSFXVolume);

        while(true)
        {
            GameObject tile = Instantiate(tilePrefab, playerTransform.position, Quaternion.identity);
            tile.transform.localScale = new Vector3(scaleX*1.25f, scaleX*1.25f, 1f);

            Rigidbody2D rigidbody = tile.GetComponent<Rigidbody2D>();
            rigidbody.velocity = transform.rotation * new Vector2(0f, tileSpeed);
            rigidbody.angularVelocity = tileAngularSpeed;

            yield return new WaitForSeconds(tileSpawnPeriod);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public float GetDuration() => duration;

    private void Move()
    {
        Vector2 position = playerTransform.position + playerTransform.rotation * padding;
        transform.position = position;
        transform.rotation = playerTransform.rotation;
    }
}
