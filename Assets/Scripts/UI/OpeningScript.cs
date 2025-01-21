using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningScript : MonoBehaviour
{

    [Header("Wait Delay Settings: ")]
    [SerializeField] [Range(0, 50)] private float waitTime;
    [SerializeField] private string sceneName;

    // Start is called before the first frame update
    private void Start() => StartCoroutine(Wait(waitTime));

    private IEnumerator Wait(float waitTiming)
    {
        yield return new WaitForSeconds(waitTiming);
        SceneManager.LoadScene(sceneName);
    }
}
