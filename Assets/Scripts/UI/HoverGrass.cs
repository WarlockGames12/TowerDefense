using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    [SerializeField][Range(0, 1000)] private int grassTilesInt;
    [SerializeField] private LayerMask grassLayer;

    private bool _playOnce;
    private HoverGrass[] _grassList;
    private PlayerUI _playerUI;

    public bool _isBuildUIActive;

    private void Start()
    {
        buildUI = FindObjectOfType<SelectionSystem>();
        _grassList = FindObjectsOfType<HoverGrass>();
        _playerUI = FindObjectOfType<PlayerUI>();

        grassTilesInt = _grassList.Length;
        bool allHaveSelectionSystem = _grassList.All(grass => grass.buildUI != null);

        if (allHaveSelectionSystem)
            buildUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isBuildUIActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _isBuildUIActive)
                CloseBuildUI();
            return;
        }
                       
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hitCollider = Physics2D.OverlapPoint(mousePos, grassLayer);

        if (hitCollider != null && hitCollider.gameObject == gameObject && transform.childCount == 0 && Time.timeScale >= 1)
        {
            HandleHover();
            if (Input.GetMouseButtonDown(0))
                OnTileSelected();
        }
        else if(!_isBuildUIActive)
            ResetHover();
    }

    private void HandleHover()
    {
        if (!_playOnce)
        {
            _playOnce = true;
            select.Play();
        }
        grass.color = hoverColor;
    }

    private void ResetHover()
    {
        _playOnce = false;
        grass.color = originalColor;
    }

    private void OnTileSelected()
    {
        ResetHover();
        _playerUI.StoreTileTransform(transform);
        ActivateBuildUI();
    }

    private void ActivateBuildUI()
    {
        buildUI.gameObject.SetActive(true);
        _isBuildUIActive = true; 
        Time.timeScale = 0;
    }

    private void CloseBuildUI()
    {
        Time.timeScale = 1;

        foreach (var grassTile in _grassList)
            grassTile.ResetHover();

        buildUI.gameObject.SetActive(false);
        _isBuildUIActive = false;
    }
}
