using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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

    public int CurrentLives;
    private List<Vector2> _wayPoints;
    private int _currentWayPointIndex = 0;
    private PlayerUI _playerUI;

    public event Action OnReachEndAction;
    private Transform _targetTower;
    private readonly List<Transform> _towersInRange = new();
    private bool _isShooting;

    private void Start()
    {
        _playerUI = FindAnyObjectByType<PlayerUI>();
        CurrentLives = lives;
        enemyLives.value = CurrentLives;

        if (shootsBack)
            StartCoroutine(TargetTower());
    }

    public void SetWaypoints(List<Vector2> path)
    {
        _wayPoints = path;
        enemyLives.value = CurrentLives;

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
        
        enemyLives.value = CurrentLives;
    }

    private void MoveTowardsWayPoint()
    {
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

        var lookDir = (Vector2)_targetTower.position - (Vector2)transform.position;
        var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        var lookTarget = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookTarget, lookSpeed * Time.deltaTime);

        if (!_isShooting)
            StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        _isShooting = true;

        while (_targetTower != null && _towersInRange.Contains(_targetTower))
        {
            if (bulletPref != null)
            {
                enemAnim.enabled = true;
                var direction = (_targetTower.position - transform.position).normalized;
                var projectile = Instantiate(bulletPref, transform.position, transform.rotation);

                if (projectile.TryGetComponent<EnemyProjectile>(out var projectileScript))
                    projectileScript.SetDir(direction);
                else
                    Debug.LogError("Projectile script is missing on the projectile prefab!");

                enemAnim.enabled = false;
            }

            yield return new WaitForSeconds(shootCooldown);
        }

        _isShooting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
            _towersInRange.Add(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            _towersInRange.Remove(collision.transform);
            if (collision.transform == _targetTower)
                _targetTower = GetNextTarget();
        }
    }

    private void OnReachEnd()
    {
        _playerUI.DepleteHealth();
        OnReachEndAction?.Invoke();
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
