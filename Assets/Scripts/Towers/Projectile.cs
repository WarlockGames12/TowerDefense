using Enemy;
using System.Collections;
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
        [SerializeField] public AudioSource enemyHit;
        [SerializeField] public AudioClip[] enemySound;
        [SerializeField] public GameObject[] bloodSplatter;
        [SerializeField] [Range(0, 5)] public int damage;

        private Vector2 _dir;
        private PlayerUI _playerUI;
        private bool isFlashing;

        public void SetDir(Vector2 direction) => _dir = direction.normalized;

        private void Start()
        {
            _playerUI = FindAnyObjectByType<PlayerUI>();   
            rb.velocity = _dir * projectileSpeed;
            Destroy(gameObject, 2.5f);
        }
        
    }
}
