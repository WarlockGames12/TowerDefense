using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCulling : MonoBehaviour
{

    [Header("Tile Culling Settings: ")]
    [SerializeField] private GameObject[] tileParent;
    [SerializeField] [Range(0, 5)] private float tileSize;

    private Camera _mainCam;

    // Start is called before the first frame update
    private void Start() => _mainCam = Camera.main;

    // Update is called once per frame
    private void Update() => CullTiles();

    private void CullTiles()
    {
        var camPos = _mainCam.transform.position;
        var camHeight = 2f * _mainCam.orthographicSize;
        var camWidth = camHeight * _mainCam.aspect;

        var minX = camPos.x - camWidth / 2 - tileSize;
        var maxX = camPos.x + camWidth / 2 + tileSize;
        var minY = camPos.y - camHeight / 2 - tileSize;
        var maxY = camPos.y + camHeight / 2 + tileSize;

        for (int i = 0; i < tileParent.Length; i++)
        {
            foreach (Transform tile in tileParent[i].transform)
            {
                var tilePos = tile.position;
                var isOnScreen = tilePos.x > minX && tilePos.x < maxX && tilePos.y > minY && tilePos.y < maxY;

                if (tile.gameObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                    spriteRenderer.enabled = isOnScreen;
            }
        }
    }
}
