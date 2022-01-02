using Photon.Pun;
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

    public bool quitGame = false;
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
                
            }
            else
            {
                Pause();
                
            }

        }
    }

    public void Resume()
    {
        inGameMenuUI.SetActive(false);
        //Time.timeScale = 1f;
        GameIsPaused = false;
        GameState.CurrentState = GameState.State.ingame;
    }

    private void Pause()
    {
        inGameMenuUI.SetActive(true);
        //Time.timeScale = 0f;
        GameIsPaused = true;
        GameState.CurrentState = GameState.State.settings;
    }

    public void QuitGame()
    {
        if(quitGame) {
            Application.Quit();
        }
        else {
            if (PhotonNetwork.IsMasterClient) {
                GameState.CurrentState = GameState.State.ingame;
                PhotonNetwork.LoadLevel((int)LoadScene.Scenes.Camp);
            }
            else {
                PhotonNetwork.LeaveRoom();
                GameState.CurrentState = GameState.State.ingame;
                SceneManager.LoadScene((int)LoadScene.Scenes.Lobby);
            }
        }
            
    }
}

