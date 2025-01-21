using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [Header("Projectile Settings: ")]
    [SerializeField] [Range(0, 50)] private float projectileSpeed;
    [SerializeField] private Rigidbody2D rb;

    [Header("Enemy Hit Settings: ")]
    [SerializeField] private AudioSource enemyHit;
    [SerializeField] private AudioClip[] enemySound;
    [SerializeField] private GameObject[] bloodSplatter;
    [SerializeField] [Range(0, 5)] private int damage;

    private Vector2 _dir;
    private PlayerUI _playerUI;

    public void SetDir(Vector2 direction) => _dir = direction.normalized;

    private void Start()
    {
        _playerUI = FindAnyObjectByType<PlayerUI>();   
        rb.velocity = _dir * projectileSpeed;
        Destroy(gameObject, 2.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<EnemyMovement>(out var enemy))
            {
                var audio = Instantiate(enemyHit, collision.transform.position, Quaternion.identity);
                if (enemy.CurrentLives > 0)
                {
                    enemyHit.clip = enemySound[0];
                    enemyHit.Play();
                    Destroy(audio.gameObject, 1);
                }
                else
                {
                    enemyHit.clip = enemySound[1];
                    enemyHit.Play();
                    Destroy(audio.gameObject, 1);
                }

                var particleSplatter = Instantiate(bloodSplatter[0], collision.transform.position, Quaternion.identity);
                Destroy(particleSplatter, 0.5f);
                enemy.CurrentLives -= damage;

                var spawner = FindObjectOfType<EnemySpawner>();
                spawner.RemoveEnemyFromList(collision.gameObject);

                if (enemy.CurrentLives <= 0)
                {
                    Instantiate(bloodSplatter[Random.Range(1,4)], collision.transform.position, Quaternion.identity);
                    var GetCoinAmount = collision.gameObject.GetComponent<EnemyMovement>().giveCoins;
                    _playerUI.GiveCoinAmount(GetCoinAmount);
                    Destroy(collision.gameObject);
                } 
            }

            Destroy(gameObject);
        }
    }
}
