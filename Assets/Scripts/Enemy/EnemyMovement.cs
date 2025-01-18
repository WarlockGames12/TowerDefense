using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMovement : MonoBehaviour
{

    [Header("Enemy Settings: ")]
    [SerializeField] [Range(0, 15)] private float movementSpeed;
    [SerializeField] [Range(0, 500)] private float lookSpeed;

    [Header("Enemy Health Settings: ")]
    [SerializeField] [Range(0, 5)] private int lives;
    [SerializeField] private Slider enemyLives;

    public int CurrentLives;
    private List<Vector2> _wayPoints;
    private int _currentWayPointIndex = 0;
    private PlayerUI _playerUI;

    private void Start()
    {
        _playerUI = FindAnyObjectByType<PlayerUI>();
        CurrentLives = lives;
        enemyLives.value = CurrentLives;
    }

    public void SetWaypoints(List<Vector2> path)
    {
        _wayPoints = path;

        if (_wayPoints == null || _wayPoints.Count == 0)
        {
            Debug.LogError("No waypoints assigned to enemy");
            return;
        }

        transform.position = _wayPoints[0];
        _currentWayPointIndex = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_wayPoints == null || _currentWayPointIndex >= _wayPoints.Count) return;
        MoveTowardsWayPoint();
        enemyLives.value = CurrentLives;
    }

    private void MoveTowardsWayPoint()
    {
        var target = _wayPoints[_currentWayPointIndex];
        Debug.Log(_wayPoints.Count);
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

    private void OnReachEnd()
    {
        _playerUI.DepleteHealth();
        Destroy(gameObject);
    }
}
