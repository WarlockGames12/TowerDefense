using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Button Hover Settings: ")]
    [SerializeField] private AudioSource hoverSound;
    [SerializeField] private Sprite[] buttonSprite;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverSound.Play();
        GetComponent<Image>().sprite = buttonSprite[1];
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = buttonSprite[0];
    }
}
