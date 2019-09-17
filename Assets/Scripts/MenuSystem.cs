using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public static bool paused = false;

    [SerializeField]
    private GameObject pauseMenu;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
    }
	public void PlayAgain()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    public void GoToMenu()
    {        
        SceneManager.LoadScene("MenuScene");
        Resume();
    }
}
