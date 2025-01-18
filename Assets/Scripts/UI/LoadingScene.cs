using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{

    [Header("Loading Settings: ")]
    [SerializeField] private Text loadingText;
    [SerializeField] private string sceneToLoad;
    [SerializeField] [Range(0, 30)] private float minLoadingTime; 
    [SerializeField] [Range(0, 30)] private float maxLoadingTime;

    // Start is called before the first frame update
    private void Start() => StartCoroutine(Loading());

    private IEnumerator Loading()
    {
        var randomLoadTime = Random.Range(minLoadingTime, maxLoadingTime);
        var elapsedTime = 0f;

        while(elapsedTime < randomLoadTime)
        {
            Debug.Log(elapsedTime);
            loadingText.text = "Loading"; yield return new WaitForSeconds(0.5f);
            loadingText.text = "Loading."; yield return new WaitForSeconds(0.5f);
            loadingText.text = "Loading.."; yield return new WaitForSeconds(0.5f);
            loadingText.text = "Loading..."; yield return new WaitForSeconds(0.5f);

            elapsedTime += 2f;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
