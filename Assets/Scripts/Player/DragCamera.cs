using System;
using UnityEngine;

public class DragCamera : MonoBehaviour
{

    [Header("Dragging Camera Settings: ")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;
    [SerializeField] [Range(0, 25)] private float dragSpeed;

    [Header("Zoom Camera Settings: ")]
    [SerializeField] [Range(0, 25)] private float zoomSpeed;
    [SerializeField] [Range(0, 25)] private float minZoom;
    [SerializeField] [Range(0, 25)] private float maxZoom;

    [Header("Paused Menu Settings: ")]
    [SerializeField] private GameObject pausedMenu;

    private Vector3 _dragOg;
    private Camera _cam;

    private void Start() => _cam = Camera.main;

    // Update is called once per frame
    private void Update()
    {
        DraggingCamera();
        HandleZoom();

        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale >= 1)
            PauseMenu(0);
    }

    public void PauseMenu(int timeScale)
    {
        if (timeScale >= 1)
        {
            Time.timeScale = timeScale;
            pausedMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = timeScale;
            pausedMenu.SetActive(true);
        }
    }

    private void DraggingCamera()
    {
        if (Input.GetMouseButtonDown(1) && Time.timeScale >= 1)
            _dragOg = Input.mousePosition;

        if (Input.GetMouseButton(1) && Time.timeScale >= 1)
        {
            var difference = Camera.main.ScreenToWorldPoint(_dragOg) - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (difference != Vector3.zero)
            {
                var newPosition = Camera.main.transform.position + difference;
                Camera.main.transform.position = newPosition;
                _dragOg = Input.mousePosition;
            }

            ClampCamPos();
        }
    }

    private void HandleZoom()
    {
        var scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            _cam.orthographicSize -= scrollInput * zoomSpeed;
            _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minZoom, maxZoom);
        }
    }

    private void ClampCamPos()
    {
        var clampX = Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x);
        var clampY = Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y);

        transform.position = new Vector3(clampX, clampY, transform.position.z);
    }
}
