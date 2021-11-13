using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject inGameMenuUI;
    GameObject settingsMenu, audioSettingsMenu, graphicSettingsMenu;
    void Start()
    {
        settingsMenu = GameObject.Find("settingsMenu");
        settingsMenu.SetActive(false);
        audioSettingsMenu = GameObject.Find("audioSettingsMenu");
        audioSettingsMenu.SetActive(false);
        graphicSettingsMenu = GameObject.Find("graphicSettingsMenu");
        graphicSettingsMenu.SetActive(false);
        inGameMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //SceneManager.LoadScene(1);
            if (GameIsPaused)
            {
                Resume();
                GameState.CurrentState = GameState.State.ingame;
            }
            else
            {
                Pause();
                GameState.CurrentState = GameState.State.settings;
            }

        }
    }

    public void Resume()
    {
        inGameMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void Pause()
    {
        inGameMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        SceneManager.LoadScene(0);
    }
}

