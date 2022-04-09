using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RocketDisplay : MonoBehaviour
{
    [SerializeField] Image rocketImage = default;
    [SerializeField] float offset = 10f;

    Image[] rocketSlots;
    int rockets;

    // Start is called before the first frame update
    void Start()
    {
        rockets = FindObjectOfType<Player>().GetRockets();
        int maxRockets = FindObjectOfType<Player>().GetMaxRockets();
        rocketSlots = new Image[maxRockets];

        SetUpRockets();
    }

    private void SetUpRockets()
    {
        for (int slot = 0; slot < rocketSlots.Length; slot++)
        {
            rocketSlots[slot] = (Image)Instantiate(rocketImage, transform);
            Vector2 position = transform.position;
            position += new Vector2(-offset * slot, 0f);
            rocketSlots[slot].transform.position = position;
            if (slot >= rockets)
                rocketSlots[slot].enabled = false;
        }
    }

    public void Decrease1Rocket()
    {
        rockets--;
        rocketSlots[rockets].enabled = false;
    }

    public void Add1Rocket()
    {
        rocketSlots[rockets].enabled = true;
        rockets++;
    }
}
