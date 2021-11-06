using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    GameObject changeCharacterMenu, settingsMenu, inventoryMenu, partyMenu, audioSettingsMenu, graphicSettingsMenu;
  void Start()
  {
        changeCharacterMenu = GameObject.Find("changeCharacterMenu");
        changeCharacterMenu.SetActive(false);
        settingsMenu = GameObject.Find("settingsMenu");
        settingsMenu.SetActive(false);
        inventoryMenu = GameObject.Find("inventoryMenu");
        inventoryMenu.SetActive(false);
        partyMenu = GameObject.Find("partyMenu");
        partyMenu.SetActive(false);
        audioSettingsMenu = GameObject.Find("audioSettingsMenu");
        audioSettingsMenu.SetActive(false);
        graphicSettingsMenu = GameObject.Find("graphicSettingsMenu");
        graphicSettingsMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // call the Resume function for the Tavern scene here
            SceneManager.LoadScene(1);
        }
    }

    public void PlayGame()
    {
        /* the first start of the game */

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
