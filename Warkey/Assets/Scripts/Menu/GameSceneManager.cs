using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    public GameObject LoadingScreen;
    public Slider slider;

    private void Start() {
        LoadGame((int)LoadScene.SceneIndex);
    }

    private void LoadGame(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex,LoadSceneMode.Single);
        LoadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progress = (operation.progress * 100f / 0.9f);
            slider.value = progress;
            yield return null;
        }
    }
}
