using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Header("Ambient")]
    [SerializeField] AudioClip ambientMusic = default;
    [SerializeField] [Range(0,1)] float ambientVolumeMin = 0f, ambientVolumeMax = 1f;
    [SerializeField] [Range(0.01f,1f)] float ambientRiseVolume = 0.3f;

    [Header("Boss")]
    [SerializeField] AudioClip bossMusic = default;
    [SerializeField] [Range(0,1)] float bossVolumeMin = 0f, bossVolumeMax = 1f;
    [SerializeField] [Range(0.01f,1f)] float bossRiseVolume = 0.3f;


    // cached references
    AudioSource sourceAmbient;
    AudioSource sourceBoss;

    // state variables
    float ambientVolumeTarget, bossVolumeTarget;


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

    private void Start()
    {
        // Set cached references
        sourceAmbient = transform.Find("Ambient Music").GetComponent<AudioSource>();
        sourceBoss    = transform.Find("Boss Music").GetComponent<AudioSource>();

        // Set clips
        sourceAmbient.clip = ambientMusic;
        sourceBoss.clip = bossMusic;

        // Set loops
        sourceAmbient.loop = true;
        sourceBoss.loop = true;

        // Set Volume
        ambientVolumeTarget = ambientVolumeMax;
        bossVolumeTarget    = bossVolumeMin;
        SetVolumeHard();

        // Play Music
        RestartAudioSource(sourceAmbient);
    }

    private void RestartAudioSource(AudioSource source)
    {
        source.Stop();
        source.Play();
    }

    private void CheckRestartAudio()
    {
        // Current volume
        float ambientVolume = sourceAmbient.volume;
        float bossVolume = sourceBoss.volume;

        // Restart Audio if needed
        if (ambientVolume == 0 && ambientVolumeTarget > 0)
            RestartAudioSource(sourceAmbient);
        if (bossVolume == 0 && bossVolumeTarget > 0)
            RestartAudioSource(sourceBoss);
    }

    private void Update() => SetVolumeSmooth();

    private void SetVolumeSmooth()
    {
        // Current volume
        float ambientVolume = sourceAmbient.volume;
        float bossVolume = sourceBoss.volume;

        // Update Volume
        ambientVolume = TransitionTowards(ambientVolume, ambientVolumeTarget, ambientRiseVolume, ambientVolumeMin, ambientVolumeMax);
        bossVolume = TransitionTowards(bossVolume, bossVolumeTarget, bossRiseVolume, bossVolumeMin, bossVolumeMax);
        SetVolume(ambientVolume, bossVolume);
    }

    private void SetVolumeHard() => SetVolume(ambientVolumeTarget, bossVolumeTarget);

    private void SetVolume(float ambientVolume, float bossVolume)
    {
        CheckRestartAudio(); // Should be given the target volumes as parameter to indicate if a change is happening from Volume: 0 -> TARGET

        SetAmbientVolume(ambientVolume);
        SetBossVolume(bossVolume);
    }

    private void SetAmbientVolume(float volume) => sourceAmbient.volume = volume;
    private void SetBossVolume(float volume) => sourceBoss.volume = volume;

    private float TransitionTowards(float current, float target, float rate, float min, float max)
    {
        float dist = target-current;
        current += dist != 0f ? Mathf.Sign(dist) * rate * Time.deltaTime : 0f;
        current = Mathf.Clamp(current, min, max);
        return current;
    }

    public void SetBossVolumeOnly()
    {
        ambientVolumeTarget = ambientVolumeMin;
        bossVolumeTarget = bossVolumeMax;
    }

    public void SetAmbientVolumeOnly()
    {
        ambientVolumeTarget = ambientVolumeMax;
        bossVolumeTarget = bossVolumeMin;
    }

    public void SetBossVolumeOnlyHard()
    {
        SetBossVolumeOnly();
        SetVolumeHard();
    }

    public void SetAmbientVolumeOnlyHard()
    {
        SetAmbientVolumeOnly();
        SetVolumeHard();
    }
}
