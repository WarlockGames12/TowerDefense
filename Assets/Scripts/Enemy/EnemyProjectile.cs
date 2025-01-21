using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings: ")]
    [SerializeField][Range(0, 50)] private float projectileSpeed;
    [SerializeField] private Rigidbody2D rb;

    [Header("Enemy Hit Settings: ")]
    [SerializeField] private AudioSource enemyHit;
    [SerializeField] private AudioClip[] enemySound;
    [SerializeField] private GameObject[] bloodSplatter;
    [SerializeField][Range(0, 5)] private int damage;

    private Vector2 _dir;
    private bool _hasHit; 

    public void SetDir(Vector2 direction) => _dir = direction.normalized;

    private void Start()
    {
        rb.velocity = _dir * projectileSpeed;
        Destroy(gameObject, 2.5f); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasHit) return;
        _hasHit = true;

        if (collision.CompareTag("Tower"))     
        {
            if (collision.TryGetComponent<CrossbowTower>(out var tower))
            {
                tower.CurrentLives -= damage;

                if (tower.CurrentLives <= 0)
                {
                    Instantiate(bloodSplatter[Random.Range(1, 4)], collision.transform.position, Quaternion.identity);
                    Destroy(collision.gameObject);
                }
                else
                {
                    var particleSplatter = Instantiate(bloodSplatter[0], collision.transform.position, Quaternion.identity);
                    Destroy(particleSplatter, 0.5f);
                }

                if (enemyHit != null)
                {
                    var audio = Instantiate(enemyHit, collision.transform.position, Quaternion.identity);
                    enemyHit.clip = tower.CurrentLives > 0 ? enemySound[0] : enemySound[1];
                    enemyHit.Play();
                    Destroy(audio.gameObject, 1);
                    Destroy(gameObject);
                }
            }
        }
    }
}
