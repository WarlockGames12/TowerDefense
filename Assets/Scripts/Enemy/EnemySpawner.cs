using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Enemy Spawner Settings: ")]
        [SerializeField, Range(0, 15)] private int maxWaves;
        [SerializeField, Range(0, 15)] private float spawnDelay;
        [SerializeField, Range(0, 15)] private float nextWaveDelay;
        [SerializeField] private Text[] showCurrentWaveAndNextWaveDelay;
        [SerializeField] private GameObject[] enemyPref;
        [SerializeField] private GameObject bossPref;
        [SerializeField] private bool hasBoss;
        [SerializeField] private Transform enemyParent;

        [Header("You Win Method: ")]
        [SerializeField] private GameObject youWinScreen;

        private float _currentWave = 0;
        private GenerateMap _map;
        private int _enemiesCanBeSpawned = 5;
        private readonly List<GameObject> _activeEnemies = new();
        private bool _waitOnce;

        private void Start()
        {
            _map = GetComponent<GenerateMap>();
            StartCoroutine(SpawnWaves());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator SpawnWaves()
        {
            if (!_waitOnce)
            {
                _waitOnce = true;
                for (var timer = nextWaveDelay; timer > 0; timer -= 1f)
                {
                    showCurrentWaveAndNextWaveDelay[1].text = $"Next Wave In: {timer} seconds";
                    yield return new WaitForSeconds(1f);
                }

                showCurrentWaveAndNextWaveDelay[1].text = "";
            }

            while (_currentWave < maxWaves)
            {
                _currentWave++;
                UpdateUI();

                if (_currentWave == 7 && hasBoss)
                {
                    SpawnBoss();

                    yield return new WaitUntil(() =>
                    {
                        _activeEnemies.RemoveAll(enemy => enemy == null);
                        return _activeEnemies.Count == 0;
                    });
                }

                for (var i = 0; i < _enemiesCanBeSpawned; i++)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(spawnDelay);
                }
                
                yield return new WaitUntil(() =>
                {
                    _activeEnemies.RemoveAll(enemy => enemy == null); 
                    return _activeEnemies.Count == 0; 
                });
                
                for (var timer = nextWaveDelay; timer > 0; timer -= 1f)
                {
                    if (_currentWave == 7)
                        showCurrentWaveAndNextWaveDelay[1].text = $"Finishing Level In: {timer} seconds!";
                    else if (_currentWave == 6 && hasBoss)
                        showCurrentWaveAndNextWaveDelay[1].text = $"Boss Incoming In: {timer} seconds!";
                    else
                        showCurrentWaveAndNextWaveDelay[1].text = $"Next Wave In: {timer} seconds";
                    yield return new WaitForSeconds(1f);
                }
                
                showCurrentWaveAndNextWaveDelay[1].text = "";
                if (_enemiesCanBeSpawned < 10) 
                    _enemiesCanBeSpawned += 1;
            }
            if (_currentWave == 7)
            {
                youWinScreen.SetActive(true);
                Time.timeScale = 0; 
            }
                
        }

        private void SpawnEnemy()
        {
            var wayPoints = _map.GetPathWaypoints();
            if (wayPoints == null || wayPoints.Count == 0)
                return;
                
            var enemy = Instantiate(enemyPref[Random.Range(0, enemyPref.Length)], wayPoints[0], Quaternion.identity, enemyParent);
            var enemyMovement = enemy.GetComponent<EnemyMovement>();
            enemyMovement.SetWaypoints(wayPoints);
            
            _activeEnemies.Add(enemy);
            enemyMovement.OnReachEndAction += () =>
            {
                _activeEnemies.Remove(enemy);
            };
        }

        private void SpawnBoss()
        {
            var wayPoints = _map.GetPathWaypoints();
            if (wayPoints == null || wayPoints.Count == 0)
                return;

            var boss = Instantiate(bossPref, wayPoints[0], Quaternion.identity, enemyParent);
            var bossMovement = boss.GetComponent<EnemyMovement>();
            bossMovement.SetWaypoints(wayPoints);

            _activeEnemies.Add(boss);
            bossMovement.OnReachEndAction += () =>
            {
                _activeEnemies.Remove(boss);
            };
        }

        private void UpdateUI()
        {
            showCurrentWaveAndNextWaveDelay[0].text = showCurrentWaveAndNextWaveDelay.Length switch
            {
                >= 2 => $"Wave: {_currentWave}/{maxWaves}",
                _ => showCurrentWaveAndNextWaveDelay[0].text
            };
        }
    }
}
