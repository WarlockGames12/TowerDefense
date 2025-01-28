using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSave : MonoBehaviour
{
    [Header("Check Save Settings: ")]
    [SerializeField] private SceneSave savingScene;
    [SerializeField] private GameObject[] menu;

    // Start is called before the first frame update
    private void Awake()
    {
        if (PlayerPrefs.HasKey(savingScene.sceneSaveKey))
        {
            menu[0].SetActive(false);
            menu[1].SetActive(true);
        }
        else
        {
            menu[0].SetActive(true);
            menu[1].SetActive(false);
        }
    }
}
