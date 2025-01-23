using Enemy;
using UnityEngine;

namespace Towers
{
    public class Projectile : MonoBehaviour
    {

        [Header("Projectile Settings: ")]
        [SerializeField] [Range(0, 50)] private float projectileSpeed;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private bool isAbleToDamage;

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
                if (collision.TryGetComponent<EnemyMovement>(out var enemy) && isAbleToDamage)
                {
                    var source = Instantiate(enemyHit, collision.transform.position, Quaternion.identity);
                    enemyHit.clip = enemy.currentLives > 0 ? enemySound[0] : enemySound[1];
                    enemyHit.Play();
                    Destroy(source.gameObject, 1);

                    var particleSplatter = Instantiate(bloodSplatter[0], collision.transform.position, Quaternion.identity);
                    Destroy(particleSplatter, 0.5f);
                    enemy.currentLives -= damage;
                    var spawner = FindObjectOfType<EnemySpawner>();
                
                    if (enemy.currentLives <= 0)
                    {
                        Instantiate(bloodSplatter[Random.Range(1,4)], collision.transform.position, Quaternion.identity);
                        var getCoinAmount = collision.gameObject.GetComponent<EnemyMovement>().giveCoins;
                        _playerUI.GiveCoinAmount(getCoinAmount);
                        enemy.OnDeath();
                        Destroy(collision.gameObject);
                    } 
                }
                else if (collision.TryGetComponent<EnemyMovement>(out var enemy1) && !isAbleToDamage)
                {
                
                }

                Destroy(gameObject);
            }
        }
    }
}
