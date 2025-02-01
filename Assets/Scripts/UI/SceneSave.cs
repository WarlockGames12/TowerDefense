using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSave : MonoBehaviour
{

    [HideInInspector] public string sceneSaveKey = "SavedScene";

    public void SaveGame()
    {
        var currentSceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(sceneSaveKey, currentSceneName);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey(sceneSaveKey))
        {
            var savedScene = PlayerPrefs.GetString(sceneSaveKey);
            SceneManager.LoadScene(savedScene);
        }
    }

    public void NewGame()
    {
        if (PlayerPrefs.HasKey(sceneSaveKey))
            PlayerPrefs.DeleteKey(sceneSaveKey);
    }
}
