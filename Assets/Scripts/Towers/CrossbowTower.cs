using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowTower : MonoBehaviour
{

    [Header("Crossbow Settings: ")]
    [SerializeField] private Animator crossbowAnim;
    [SerializeField][Range(0, 50)] private float detectionRange;
    [SerializeField][Range(0, 250)] private float rotSpeed;

    [Header("Projectile Settings: ")]
    [SerializeField] private GameObject projectilePref;
    [SerializeField][Range(0, 10)] private float delayDur;

    private Transform _target = null;
    private List<Transform> _enemiesInRange = new();
    private bool _isShooting;

    // Start is called before the first frame update
    private void Start() => StartCoroutine(TargetEnemy());

    // Update is called once per frame
    private void Update() => RotateTowardsTarget();

    private void RotateTowardsTarget()
    {
        if (_target == null) return;

        var dir = _target.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        var targetRot = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
    }


    private IEnumerator TargetEnemy()
    {
        while(true)
        { 
            if (_target == null || !_enemiesInRange.Contains(_target))
                _target = GetNextTarget();

            if (_target != null && !_isShooting)
                StartCoroutine(Shoot());

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Shoot()
    {
        _isShooting = true;
        while(_target != null && _enemiesInRange.Contains(_target))
        {
            var projectile = Instantiate(projectilePref, transform.position, transform.rotation);
            var projectScript = projectile.GetComponent<Projectile>();

            if (projectScript != null)
                projectScript.SetTarget(_target);

            yield return new WaitForSeconds(delayDur);
        }
    }

    private Transform GetNextTarget()
    {
        if (_enemiesInRange.Count == 0)
        {
            crossbowAnim.enabled = false;
            return null;
        }

        Transform closestEnemy = null;
        var closestDistance = float.MaxValue;

        foreach (var enemy in _enemiesInRange)
        {
            if (enemy == null) continue;

            var distance = Vector3.Distance(transform.position, enemy.position);
            if (distance < closestDistance)
            {
                crossbowAnim.enabled = true;
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }
        Debug.Log(closestEnemy);
        return closestEnemy;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _enemiesInRange.Add(collision.transform);
            Debug.Log(_enemiesInRange);
        }
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _enemiesInRange.Remove(collision.transform);
            if (collision.transform == _target)
                _target = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
