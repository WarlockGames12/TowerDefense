using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [Header("Projectile Settings: ")]
    [SerializeField] [Range(0, 50)] private float projectileSpeed;
    private Transform _target;

    public void SetTarget(Transform target) => _target = target;

    // Update is called once per frame
    private void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        var dir = (_target.position - transform.position).normalized;
        transform.position += projectileSpeed * Time.deltaTime * dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyMovement>().CurrentLives -= 1;
            Destroy(gameObject);
        }
    }
}
