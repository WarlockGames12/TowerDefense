using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorBehaviour : MonoBehaviour
{

    [Header("Cursor Settings: ")]
    [SerializeField] private Texture2D[] cursorSprite;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;

    // Start is called before the first frame update
    private void Start() => SetCursor(0);

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            SetCursor(1);
        else if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            SetCursor(0);
    }

    private void SetCursor(int index) => Cursor.SetCursor(cursorSprite[index], hotSpot, cursorMode);
}
