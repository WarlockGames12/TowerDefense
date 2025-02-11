using Enemy;
using System.Collections;
using System.Collections.Generic;
using Towers;
using UnityEngine;
using UnityEngine.UI;

public class CrossbowTower : MonoBehaviour
{

    [Header("Crossbow Settings: ")]
    [SerializeField] private Animator crossbowAnim;
    [SerializeField][Range(0, 50)] private float detectionRange;
    [SerializeField][Range(0, 250)] private float rotSpeed;

    [Header("Crossbow Lives Settings: ")]
    [SerializeField] [Range(0, 15)] private int lives;
    [SerializeField] private Slider towerLives;

    [Header("Projectile Settings: ")]
    [SerializeField] private GameObject projectilePref;
    [SerializeField][Range(0, 10)] private float delayDur;

    private Transform _target = null;
    private readonly List<Transform> _enemiesInRange = new();
    private bool _isShooting;
    private Rigidbody2D _rb;

    private bool _shootOnce;
    public int CurrentLives;
    private bool isFlashing;
    private EnemyProjectile _enemyProjectile;

    private void Start()
    {
        CurrentLives = lives;
        towerLives.value = CurrentLives;

        _rb = GetComponent<Rigidbody2D>();
        _rb.freezeRotation = true;

        var childCollider = GetComponentInChildren<CircleCollider2D>();
        
        var mainCollider = GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(childCollider, mainCollider);

        StartCoroutine(TargetEnemy());
    }

    private void Update()
    {
        if (_enemyProjectile != null && _enemyProjectile.isLightning)
        {

        }
        else
        {
            towerLives.value = CurrentLives;
            GetComponentInChildren<CircleCollider2D>().radius = detectionRange;
            RotateTowardsTarget();
        }
    }

    private void RotateTowardsTarget()
    {
        if (_target == null) return;

        var dir = _target.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _rb.freezeRotation = false;

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

            if (_target == null)
            {
                _isShooting = false;
                crossbowAnim.Play(0, 0, 0f);
                _rb.freezeRotation = true;
            }
               

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Shoot()
    {
        _isShooting = true;
        while(_target != null && _enemiesInRange.Contains(_target))
        {
            _shootOnce = false;
            yield return new WaitForSeconds(delayDur);
            if (_target != null)
            {
                var direction = (_target.position - transform.position).normalized;
                if (!_shootOnce)
                {
                    _shootOnce = true;
                    var projectile = Instantiate(projectilePref, transform.position, transform.rotation);
                    if (projectile.TryGetComponent<Projectile>(out var projectileScript))
                        projectileScript.SetDir(direction);
                    else
                        Debug.LogError("Projectile script is missing on the projectile prefab!");
                }
                crossbowAnim.Play(0, 0, 0f);
            }
        }
    }

    private Transform GetNextTarget()
    {
        if (_enemiesInRange.Count == 0)
        {
            crossbowAnim.Play(0, 0, 0f);
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
        return closestEnemy;
    }

    public IEnumerator GiveDamage()
    {
        if (isFlashing)
            yield break;

        isFlashing = true;
        var flashDuration = 0.1f;
        var flashCount = 3;

        if (!TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            isFlashing = false;
            yield break;
        }

        var originalColor = spriteRenderer.color;

        for (var i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashDuration);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        isFlashing = false;
        _enemyProjectile = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            var enemyProjectile = collision.gameObject.GetComponent<EnemyProjectile>();
            CurrentLives -= enemyProjectile.damage;
            _enemyProjectile = collision.gameObject.GetComponent<EnemyProjectile>();

            var source = Instantiate(enemyProjectile.enemyHit, transform.position, Quaternion.identity);
            enemyProjectile.enemyHit.clip = CurrentLives > 0 ? enemyProjectile.enemySound[0] : enemyProjectile.enemySound[1];
            enemyProjectile.enemyHit.Play();
            Destroy(source.gameObject, 1);

            var particleSplatter = Instantiate(enemyProjectile.bloodSplatter[0], transform.position, Quaternion.identity);
            Destroy(particleSplatter, 0.5f);

            StartCoroutine(GiveDamage());

            if (CurrentLives <= 0)
            {
                Instantiate(enemyProjectile.bloodSplatter[1], transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            // Destroy(collision.gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            _enemiesInRange.Add(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _enemiesInRange.Remove(collision.transform);
            if (collision.transform == _target)
                  _target = GetNextTarget();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
