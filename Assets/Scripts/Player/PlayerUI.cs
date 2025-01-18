using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    [Header("Player Health Settings: ")]
    [SerializeField] private int playerHealth;
    [SerializeField] private Text playerHealthText;
    [SerializeField] private Slider healthSlider;

    private int originalHealth;

    private void Start() => originalHealth = playerHealth;

    private void Update()
    {
        playerHealthText.text = playerHealth + "/" + originalHealth;
        healthSlider.value = playerHealth;
    }

    public void DepleteHealth() => playerHealth -= 5;
}
