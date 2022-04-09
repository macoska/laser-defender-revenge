using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField] [Range(0f, 3f)] float gameOverDelay = 1f;
    [SerializeField] [Range(0f, 3f)] float sceneTransitionDelay = 1f;

    Coroutine loadingGameOver;

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

    public void LoadStartMenu()
    {
        RestartScene();
        SceneManager.LoadScene(0);
    }

    public void LoadGame()
    {
        GameSession gameSession = FindObjectOfType<GameSession>();
        if (gameSession)
            gameSession.ResetGame();
        RestartScene();
        SceneManager.LoadScene("Level 1");
    }

    public void LoadNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LevelTransition(currentLevel+1));
    }

    private IEnumerator LevelTransition(int level)
    {
        WarpfieldSpawner warpfieldSpawner = FindObjectOfType<WarpfieldSpawner>();
        FindObjectOfType<InfoBox>().ShowInfo("Enter Warpfield", 2f);
        yield return warpfieldSpawner.EnterWarpfield();
        yield return new WaitForSeconds(sceneTransitionDelay/2);
        SceneManager.LoadScene(level);
        yield return new WaitForSeconds(sceneTransitionDelay/2);
        yield return warpfieldSpawner.ExitWarpfield();
    }

    public void LoadGameOver()
    {
        if (loadingGameOver == null)
        {
            GameSession gameSession = FindObjectOfType<GameSession>();
            gameSession.Decrease1Life();
            if (gameSession.GetLifes() > 0)
                loadingGameOver = StartCoroutine(DelayRestartLevel());
            else
                loadingGameOver = StartCoroutine(DelayGameOver());
        }
    }

    IEnumerator DelayGameOver()
    {
        FindObjectOfType<WarpfieldSpawner>().KillWarpfield();
        yield return new WaitForSeconds(gameOverDelay);
        RestartScene();
        SceneManager.LoadScene("Game Over");
        loadingGameOver = null;
    }

    private IEnumerator DelayRestartLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        yield return new WaitForSeconds(gameOverDelay);
        RestartScene();
        SceneManager.LoadScene(currentLevel);
        loadingGameOver = null;
    }

    private void RestartScene()
    {
        // Destroy player
        Player player = FindObjectOfType<Player>();
        if (player) Destroy(player.gameObject);

        // Destroy helperships
        HelperShip[] helperShips = FindObjectsOfType<HelperShip>();
        foreach (var helperShip in helperShips) Destroy(helperShip.gameObject);
    }

    public void QuitGame() => Application.Quit();
}
