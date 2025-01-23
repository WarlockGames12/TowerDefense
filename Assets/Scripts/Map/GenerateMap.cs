using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{

    [Header("Generate Map Settings: ")]
    [SerializeField] private GameObject[] grassTilePref;
    [SerializeField] private GameObject pathTilePref;
    [SerializeField] private Transform[] parentTile;

    [Header("Create Map Float Settings: ")]
    [SerializeField] [Range(0, 50)] private float tileSize;
    [SerializeField] [Range(0, 50)] private int gridWidth;
    [SerializeField] [Range(0, 50)] private int gridHeight;

    private readonly List<Vector2> pathWaypoints = new(); 
    
    public List<Vector2> GetPathWaypoints() => pathWaypoints;

    private void Awake() => GenerateLevel();
    private void GenerateLevel()
    {
        var pathGrid = new bool[gridWidth, gridHeight];
        GeneratePath(pathGrid);

        if (gridWidth * gridHeight > 10000)
        {
            UnityEngine.Debug.LogError("Grid size too large; reduce grid dimensions.");
            return;
        }

        for (var x = 0; x < gridWidth; x++)
        {
            for (var y = 0; y < gridHeight; y++)
            {
                var spawnPos = new Vector3(x * tileSize, y * tileSize, 0);
                if (pathGrid[x, y])
                {
                    var tile = DeterminePathTile(pathGrid, x, y);
                    Instantiate(tile, spawnPos, Quaternion.identity, parentTile[1]);
                }
                else
                    Instantiate(grassTilePref[Random.Range(0,grassTilePref.Length)], spawnPos, Quaternion.identity, parentTile[0]);
            }
        }
    }

    private void GeneratePath(bool[,] pathGrid)
    {
        var currentX = 0;
        var currentY = 0; 
        pathGrid[currentX, currentY] = true;
        
        var straightCounter = 0;
        var lastMove = new Vector2Int(1, 0); 
        var mustGoStraight = false; 

        while (currentX < gridWidth - 1)
        {
            var validMoves = new List<Vector2Int>();

            Vector2Int[] moves = {
                new Vector2Int(1, 0),  
                new Vector2Int(0, 1),  
                new Vector2Int(0, -1)  
            };

            foreach (var move in moves)
            {
                var nextX = currentX + move.x;
                var nextY = currentY + move.y;

                if (nextX < gridWidth && nextY >= 0 && nextY < gridHeight)
                {
                    if (!pathGrid[nextX, nextY] && move != -lastMove)
                        validMoves.Add(move);
                }
            }

            if (mustGoStraight && validMoves.Contains(lastMove))
            {
                validMoves.Clear();
                validMoves.Add(lastMove);
            }

            if (validMoves.Count == 0)
            {
                UnityEngine.Debug.LogError("Path generation failed: No valid moves available.");
                break;
            }

            var chosenMove = validMoves[Random.Range(0, validMoves.Count)];

            currentX += chosenMove.x;
            currentY += chosenMove.y;
            pathGrid[currentX, currentY] = true;

            if (chosenMove == lastMove)
            {
                straightCounter++;
                mustGoStraight = false;
            }
            else
            {
                straightCounter = 0;
                mustGoStraight = true; 
            }

            if (currentX >= gridWidth - 1)
            {
                UnityEngine.Debug.Log("Path successfully generated.");
                break;
            }
            else if (validMoves.Count == 0)
            {
                UnityEngine.Debug.LogError("Path generation stopped unexpectedly.");
                break;
            }

            pathWaypoints.Add(new Vector2(currentX * tileSize, currentY * tileSize));

            lastMove = chosenMove;
        }
    }

    private GameObject DeterminePathTile(bool[,] pathgrid, int x, int y)
    {
        var left = (x > 0 && pathgrid[x - 1, y]);
        var right = (x < gridWidth - 1 && pathgrid[x + 1, y]);
        var up = (y < gridHeight - 1 && pathgrid[x, y + 1]);
        var down = (y > 0 && pathgrid[x, y-1]);

        if ((left && right && !up && !down) || (!left && !right && up && down)) 
            return pathTilePref; 
        if (up && right)
            return pathTilePref;
        if (down && right)
            return pathTilePref;

        return pathTilePref;
    }
}
