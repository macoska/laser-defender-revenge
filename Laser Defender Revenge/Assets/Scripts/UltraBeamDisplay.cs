using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UltraBeamDisplay : MonoBehaviour
{
    Image ultraBeam;

    // Start is called before the first frame update
    void Start()
    {
        ultraBeam = GetComponent<Image>();
        Show(FindObjectOfType<Player>().isUltraBeamReady());
    }

    public void Show(bool isReady) => ultraBeam.enabled = isReady;
}
