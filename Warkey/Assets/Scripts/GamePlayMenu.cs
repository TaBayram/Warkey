using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayMenu : MonoBehaviour
{
    GameObject settingsMenu;
    void Start()
    {
        settingsMenu = GameObject.Find("settingsMenu");
        settingsMenu.SetActive(false);
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
        SceneManager.LoadScene(0);
    }
}

