using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [Header("Projectile Settings: ")]
    [SerializeField] [Range(0, 50)] private float projectileSpeed;
    [SerializeField] private Rigidbody2D rb;

    private Vector2 _dir;

    public void SetDir(Vector2 direction) => _dir = direction.normalized;

    private void Start()
    {
        rb.velocity = _dir * projectileSpeed;
        Destroy(gameObject, 10);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<EnemyMovement>(out var enemy))
            {
                enemy.CurrentLives -= 1;

                if (enemy.CurrentLives <= 0)
                    Destroy(collision.gameObject);
            }

            Destroy(gameObject);
        }
    }
}
