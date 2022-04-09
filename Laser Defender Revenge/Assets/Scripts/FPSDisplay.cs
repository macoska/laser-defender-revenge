using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    // cached references
    Text fpsText;

    // Start is called before the first frame update
    void Start()
    {
        fpsText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int FPS = Mathf.RoundToInt(1f / Time.deltaTime);
        fpsText.text = "FPS: " + FPS.ToString();
    }
}
