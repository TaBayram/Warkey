using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    GameObject changeCharacterMenu, settingsMenu, inventoryMenu, partyMenu;
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
    }

    void Update()
    {
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene(1);
        }
    }

    public void PlayGame()
    {
        /* Scene number will be modifies according to the order of scenes */

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
