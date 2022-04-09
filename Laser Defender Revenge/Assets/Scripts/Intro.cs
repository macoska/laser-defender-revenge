using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Intro : MonoBehaviour
{
    [SerializeField] float destroyDelay = 4f;

    // Start is called before the first frame update
    void Start()
    {
        ShowLevelHeader();
        Destroy(gameObject, destroyDelay);
    }

    private void ShowLevelHeader()
    {
        int totalLevel = SceneManager.sceneCountInBuildSettings - 2;
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        TextMeshProUGUI levelText = GetComponent<TextMeshProUGUI>();
        levelText.text = "wave " + currentLevel.ToString() + " / " + totalLevel.ToString();
    }
}
