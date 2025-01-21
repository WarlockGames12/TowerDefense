using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class StreamVideo : MonoBehaviour
{

    [Header("Stream Settings: ")]
    [SerializeField] private string fileName;

    // Start is called before the first frame update
    private void Start() => PlayVideo(fileName);

    private void PlayVideo(string vidFile)
    {
        var vidPlayer = GetComponent<VideoPlayer>();

        if (vidPlayer)
        {
            var vidPath = System.IO.Path.Combine(Application.streamingAssetsPath, vidFile);
            vidPlayer.url = vidPath;
            vidPlayer.Play();
        }
    }
}
