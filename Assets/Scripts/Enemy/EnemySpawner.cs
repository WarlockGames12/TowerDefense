using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [Header("Spawner Settings: ")]
    [SerializeField] private GameObject enemyPref;
    [SerializeField] private GenerateMap map;
    [SerializeField] private float spawnDelay = 2f;
    [SerializeField] private Transform enemyParent;

    private int _spawnIndex;

    // Start is called before the first frame update
    private void Start() => StartCoroutine(SpawnEnemies());

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(5);
        while(true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnEnemy()
    {
        var wayPoints = map.GetPathWaypoints();
        if (wayPoints == null || wayPoints.Count == 0) return;

        var enemy = Instantiate(enemyPref, wayPoints[0], Quaternion.identity, enemyParent);
        var enemyMovement = enemy.GetComponent<EnemyMovement>();
        enemyMovement.SetWaypoints(wayPoints);
    }
}
