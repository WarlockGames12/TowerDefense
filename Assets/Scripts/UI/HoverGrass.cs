using System.Collections.Generic;
using UnityEngine;

public class HoverGrass : MonoBehaviour
{

    [Header("Hover Over Grass Settings: ")]
    [SerializeField] private AudioSource select;
    [SerializeField] private Color hoverColor = Color.green;
    [SerializeField] private Color originalColor;
    [SerializeField] private SpriteRenderer grass;

    [Header("Show Build UI: ")]
    [SerializeField] private SelectionSystem buildUI;
    [SerializeField] [Range(0, 1000)] private int grassTilesInt;

    private bool _playOnce;
    private HoverGrass[] _grassList;
    private bool _stoppedTime;

    private void Awake()
    {
        buildUI = FindObjectOfType<SelectionSystem>();
        _grassList = FindObjectsOfType<HoverGrass>();

        if (_grassList.Length == grassTilesInt && buildUI != null)
            buildUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hitCollider = Physics2D.OverlapPoint(mousePos);

        if (hitCollider != null && hitCollider.gameObject == gameObject)
        {
            if (!_playOnce)
            {
                _playOnce = true;
                select.Play();
            }
            if (Input.GetMouseButtonDown(0))
            {
                buildUI.gameObject.SetActive(true);
                _stoppedTime = true;
                Time.timeScale = 0;
            }
            grass.color = hoverColor;
        }
        else
        {
            _playOnce = false;
            grass.color = originalColor;
        } 

        if (Input.GetKeyDown(KeyCode.Escape) && _stoppedTime)
        {
            Time.timeScale = 1;
            buildUI.gameObject.SetActive(false);
            _stoppedTime = false;
        }
    }
}
