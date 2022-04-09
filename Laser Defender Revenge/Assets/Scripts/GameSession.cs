using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    int score = 0;
    [SerializeField] int lifes = 5;

    // Awake is called before Start()
    void Awake() => SetUpSingleton();

    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            gameObject.SetActive(false); // bug fix: This bug is always nasty when you have multiple scenes... Happens a lot, remember it pls!
            Destroy(gameObject);
        }
        else
            DontDestroyOnLoad(gameObject);
    }

    public void AddToScore(int score) => this.score += score;

    public int GetScore() => score;

    public void ResetGame() => Destroy(gameObject);

    public int GetLifes() => lifes;

    public void Decrease1Life()
    {
        lifes--;
        FindObjectOfType<LifeDisplay>().Decrease1Life();
    }
}
