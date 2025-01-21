using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloodsplatter : MonoBehaviour
{

    [Header("Blood Splatter Settings: ")]
    [SerializeField] private Animator bloodSplatAnim;
    [SerializeField] [Range(0, 50)] private float waitDur;

    // Start is called before the first frame update
    private void Start() => Destroy(bloodSplatAnim.gameObject, waitDur);
}
