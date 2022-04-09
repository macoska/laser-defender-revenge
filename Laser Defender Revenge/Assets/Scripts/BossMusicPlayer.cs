using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMusicPlayer : MonoBehaviour
{
    // cached variables
    MusicPlayer musicPlayer;

    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = FindObjectOfType<MusicPlayer>();

        // Start epic boss music
        if (musicPlayer) musicPlayer.SetBossVolumeOnlyHard();
    }

    public void TurnOffBossMusic()
    {
        if (musicPlayer)
            musicPlayer.SetAmbientVolumeOnly();
    }
}
