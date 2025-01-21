using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler
{

    [Header("Button Hover Settings: ")]
    [SerializeField] private AudioSource hoverSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverSound.Play();
    }
}
