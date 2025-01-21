using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [Header("Spawner Settings: ")]
    [SerializeField] private GameObject[] enemyPref;
    [SerializeField] private GenerateMap map;
    [SerializeField] private float spawnDelay = 2f;
    [SerializeField] private Transform enemyParent;

    private int totalEnemyCount = 0;
    private const int maxEnemyCount = 50;
    private List<GameObject> activeEnemies = new(); 

    private void Start() => StartCoroutine(SpawnEnemies());

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(5);
        while (totalEnemyCount < maxEnemyCount)
        {
            RemoveMissingEnemies();

            if (activeEnemies.Count < 7)
                SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
        Debug.Log("Reached maximum enemy count.");
    }

    private void SpawnEnemy()
    {
        var wayPoints = map.GetPathWaypoints();
        if (wayPoints == null || wayPoints.Count == 0) return;

        var enemy = Instantiate(enemyPref[Random.Range(0, enemyPref.Length)], wayPoints[0], Quaternion.identity, enemyParent);
        var enemyMovement = enemy.GetComponent<EnemyMovement>();
        enemyMovement.SetWaypoints(wayPoints);

        totalEnemyCount++;
        activeEnemies.Add(enemy);

        enemy.GetComponent<EnemyMovement>().OnReachEndAction += () => RemoveEnemyFromList(enemy);
    }

    private void RemoveMissingEnemies() => activeEnemies.RemoveAll(enemy => enemy == null);

    public void RemoveEnemyFromList(GameObject enemy) => activeEnemies.Remove(enemy);
}
