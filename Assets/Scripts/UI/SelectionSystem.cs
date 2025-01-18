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

    // Update is called once per frame
    private void Update()
    {
        OrbitAroundBall();
        RotateArrowTowardsMouse();
    }

    private void OrbitAroundBall()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var dir = mousePos - (Vector2)middleBall.position;
        var angle = Mathf.Atan2(dir.y, dir.x);

        var newArrowPos = (Vector2)middleBall.position + new Vector2(Mathf.Cos(angle) * orbitRad, Mathf.Sin(angle) * orbitRad);
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
