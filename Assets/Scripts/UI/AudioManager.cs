using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Manager Settings: ")]
    [SerializeField] private bool isOnMenu;
    [SerializeField] private bool isInBattle;
    private static AudioManager _instance;

    private void Awake()
    {
        if (isOnMenu)
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
                _instance = null;
            }
            return;
        }
        else if (isInBattle && _instance != null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void PlayMusic()
    {
        if (GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Play();
    }

    public void StopMusic()
    {
        if(GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Stop();
    }
}
