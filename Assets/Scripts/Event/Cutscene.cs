using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{

    [Header("Cutscene Settings: ")]
    [SerializeField] private Animator[] startAnimation;

    private void Awake()
    {
        GetComponent<StartEvent>().enabled = false;
        StartCoroutine(BeginningScene());
    }

    private IEnumerator BeginningScene()
    {
        startAnimation[0].enabled = true;
        yield return new WaitForSeconds(1f);
        startAnimation[1].enabled = true;
        yield return new WaitForSeconds(2.5f);
        GetComponent<StartEvent>().enabled = true;
    }
}
