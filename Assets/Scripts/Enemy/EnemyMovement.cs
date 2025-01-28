using System;
using System.Collections;
using System.Collections.Generic;
using Towers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace Enemy
{
    public class EnemyMovement : MonoBehaviour
    {

        [Header("Enemy Settings: ")]
        [SerializeField] private Animator enemAnim = null;
        [SerializeField][Range(0, 15)] private float movementSpeed;
        [SerializeField][Range(0, 500)] private float lookSpeed;
        [SerializeField] private bool shootsBack;
        public int giveCoins;

        [Header("Enemy Health Settings: ")]
        [SerializeField][Range(0, 15)] private int lives;
        [SerializeField] private Slider enemyLives;

        [Header("Shooting Settings: ")]
        [SerializeField][Range(0, 50)] private float shootingRange;
        [SerializeField][Range(0, 10)] private float shootCooldown;
        [SerializeField] private GameObject bulletPref;

        public Action OnReachEndAction;
        public int currentLives;
        private List<Vector2> _wayPoints;
        private int _currentWayPointIndex = 0;
        private PlayerUI _playerUI;

        private Transform _targetTower;
        private readonly List<Transform> _towersInRange = new();
        private bool _isShooting;
        private bool _shootOnce;
        private bool isFlashing;

        private void Start()
        {
            _playerUI = FindAnyObjectByType<PlayerUI>();
            currentLives = lives;
            enemyLives.value = currentLives;

            if (shootsBack)
            {
                enemAnim = gameObject.GetComponent<Animator>();
                if (enemAnim == null)
                    Destroy(gameObject);
                StartCoroutine(TargetTower());
            }
                
        }

        public void SetWaypoints(List<Vector2> path)
        {
            _wayPoints = path;
            enemyLives.value = currentLives;

            if (_wayPoints == null || _wayPoints.Count == 0)
            {
                Debug.LogError("No waypoints assigned to enemy");
                return;
            }

            transform.position = _wayPoints[0];
            _currentWayPointIndex = 1;
        }

        private void Update()
        {
            if (_wayPoints == null || _currentWayPointIndex >= _wayPoints.Count) return;

            if (shootsBack && _targetTower != null)
                StopAndShoot();
            else
                MoveTowardsWayPoint();
        
            enemyLives.value = currentLives;
        }

        private void MoveTowardsWayPoint()
        {
            if (enemAnim != null)
                enemAnim.enabled = false;

            var target = _wayPoints[_currentWayPointIndex];
            var dir = (target - (Vector2)transform.position).normalized;

            var lookDir = target - (Vector2)transform.position;
            var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            var lookTarget = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookTarget, lookSpeed * Time.deltaTime);

            transform.position += (Vector3)(movementSpeed * Time.deltaTime * dir);
            if (Vector2.Distance(transform.position, target) < 0.1f)
            {
                _currentWayPointIndex++;
                if (_currentWayPointIndex >= _wayPoints.Count)
                    OnReachEnd();
            }
        }

        private IEnumerator TargetTower()
        {
            while (true)
            {
                if (_targetTower == null || !_towersInRange.Contains(_targetTower))
                    _targetTower = GetNextTarget();

                if (_targetTower != null && !_isShooting)
                    StartCoroutine(Shoot());

                if (_targetTower == null)
                    _isShooting = false;

                yield return new WaitForSeconds(0.1f);
            }
        }

        private Transform GetNextTarget()
        {
            if (_towersInRange.Count == 0) return null;

            Transform closestTower = null;
            var closestDistance = float.MaxValue;

            foreach (var tower in _towersInRange)
            {
                if (tower == null) continue;

                var distance = Vector3.Distance(transform.position, tower.position);
                if (distance < closestDistance)
                {
                    closestTower = tower;
                    closestDistance = distance;
                }
            }

            return closestTower;
        }

        private void StopAndShoot()
        {
            if (!_targetTower) return;

            var dir = _targetTower.position - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var targetRot = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, lookSpeed * Time.deltaTime);

            if (!_isShooting)
                StartCoroutine(Shoot());
        }

        private IEnumerator Shoot()
        {
            _isShooting = true;

            while (_targetTower != null && _towersInRange.Contains(_targetTower))
            {
                _shootOnce = false;
                yield return new WaitForSeconds(shootCooldown);

                if (!_shootOnce)
                {
                    _shootOnce = true;
                    if (enemAnim != null)
                        enemAnim.Play(0, 0, 0f);

                    if (_targetTower != null)
                    {
                        var direction = (_targetTower.position - transform.position).normalized;
                        var spawnPos = transform.position + (direction * 0.5f);
                        var projectile = Instantiate(bulletPref, spawnPos, transform.rotation);

                        if (projectile.TryGetComponent<EnemyProjectile>(out var projectileScript))
                            projectileScript.SetDir(direction);
                        else
                            Debug.LogError("Projectile script is missing on the projectile prefab!");
                    }
                }
            }

            _isShooting = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Towering"))
                _towersInRange.Add(collision.transform);

            if (collision.CompareTag("Projectile"))
            {
                if (collision.TryGetComponent<Projectile>(out var enemy))
                {
                    currentLives -= enemy.damage;

                    var source = Instantiate(enemy.enemyHit, transform.position, Quaternion.identity);
                    enemy.enemyHit.clip = currentLives > 0 ? enemy.enemySound[0] : enemy.enemySound[1];
                    enemy.enemyHit.Play();
                    Destroy(source.gameObject, 1);

                    var particleSplatter = Instantiate(enemy.bloodSplatter[0], transform.position, Quaternion.identity);
                    Destroy(particleSplatter, 0.5f);

                    StartCoroutine(GiveDamage(gameObject));

                    if (currentLives <= 0)
                    {
                        Instantiate(enemy.bloodSplatter[UnityEngine.Random.Range(1, 4)], collision.transform.position, Quaternion.identity);
                        _playerUI.GiveCoinAmount(giveCoins);
                        OnDeath();
                        Destroy(gameObject);
                    }
                }

                Destroy(collision.gameObject);
            }
        }

        private IEnumerator GiveDamage(GameObject currentObject)
        {
            if (isFlashing)
                yield break;

            isFlashing = true;
            var flashDuration = 0.1f;
            var flashCount = 3;

            if (!currentObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                isFlashing = false;
                yield break;
            }

            var originalColor = spriteRenderer.color;

            for (var i = 0; i < flashCount; i++)
            {
                if (currentObject == null)
                    yield break;

                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(flashDuration);

                if (currentObject == null)
                    yield break;

                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(flashDuration);
            }

            if (currentObject != null)
                spriteRenderer.color = originalColor;

            isFlashing = false;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Towering"))
            {
                _towersInRange.Remove(collision.transform);
                if (collision.transform == _targetTower)
                    _targetTower = GetNextTarget();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnReachEnd()
        {
            _playerUI.DepleteHealth();
            OnReachEndAction?.Invoke();
            Destroy(gameObject);
        }

        public void OnDeath()
        {
            OnReachEndAction?.Invoke();
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, shootingRange);
        }
    }
}
