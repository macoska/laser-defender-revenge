using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthDisplay : MonoBehaviour
{
    // Cached references
    Boss boss;
    RectTransform rt;

    // state variables
    float maxPixel;

    // Start is called before the first frame update
    void Start()
    {
        boss = FindObjectOfType<Boss>();
        rt = GetComponent<RectTransform>();
        maxPixel = rt.sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (boss)
            UpdateHealthBar();
        else
        {
            enabled = false;
            Destroy(transform.parent.gameObject);
        }
    }

    private void UpdateHealthBar()
    {
        enabled = true;
        float height = boss.GetHealth() / (float)boss.GetMaxHealth() * maxPixel;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
    }
}
