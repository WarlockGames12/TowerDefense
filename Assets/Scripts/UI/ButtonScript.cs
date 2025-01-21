using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void Scene(string sceneName) => SceneManager.LoadScene(sceneName);
    public void ControlTime(int timeScale) => Time.timeScale = timeScale;
    public void ExitGame() => Application.Quit();
}
