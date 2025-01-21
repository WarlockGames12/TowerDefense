using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSystem : MonoBehaviour
{

    [Header("Selector Settings: ")]
    [SerializeField] private Transform arrowPointer;
    [SerializeField] private Transform middleBall;
    [SerializeField] [Range(0, 10)] private float orbitRad;
    [SerializeField] [Range(0, 150)] private float rotSpeed;

    private Camera _mainCamera;

    private void Start() => _mainCamera = Camera.main;

    // Update is called once per frame
    private void Update()
    {
        OrbitAroundBall();
        RotateArrowTowardsMouse();
    }

    private void OrbitAroundBall()
    {
        var zoomFactor = _mainCamera.orthographicSize / 5;
        var adjustOrbitRad = orbitRad * zoomFactor; 

        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        var dir = mousePos - (Vector2)middleBall.position;
        var angle = Mathf.Atan2(dir.y, dir.x);

        var newArrowPos = (Vector2)middleBall.position + new Vector2(Mathf.Cos(angle) * adjustOrbitRad, Mathf.Sin(angle) * adjustOrbitRad);
        arrowPointer.position = newArrowPos;
    }

    private void RotateArrowTowardsMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var dir = mousePos - (Vector2)middleBall.position;

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowPointer.rotation = Quaternion.Euler(0, 0, angle);
    }
}
