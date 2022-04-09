using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplay : MonoBehaviour
{
    [SerializeField] Image lifeImage = default;
    [SerializeField] float offset = 30f;

    Image[] lifeSlots;
    int lifes;

    // Start is called before the first frame update
    void Start()
    {
        lifes = FindObjectOfType<GameSession>().GetLifes();
        lifeSlots = new Image[lifes];

        SetUpLifes();
    }

    private void SetUpLifes()
    {
        for (int slot = 0; slot < lifeSlots.Length; slot++)
        {
            lifeSlots[slot] = (Image)Instantiate(lifeImage, transform);
            Vector2 position = transform.position;
            position += new Vector2(-offset * slot, 0f);
            lifeSlots[slot].transform.position = position;
        }
    }

    public void Decrease1Life()
    {
        lifes--;
        lifeSlots[lifes].enabled = false;
    }
}
