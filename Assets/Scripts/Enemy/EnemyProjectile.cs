using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemyProjectile : MonoBehaviour
    {
        
        [Header("Projectile Settings: ")]
        [SerializeField][Range(0, 50)] private float projectileSpeed;
        [SerializeField] private Rigidbody2D rb;

        [Header("Enemy Hit Settings: ")]
        public AudioSource enemyHit;
        public AudioClip[] enemySound;
        public GameObject[] bloodSplatter;
        [Range(0, 5)] public int damage;
        public bool isLightning;

        private Vector2 _dir;
        // private bool isAbleToDamage = true;

        public void SetDir(Vector2 direction) => _dir = direction.normalized;

        private void Start()
        {
            rb.velocity = _dir * projectileSpeed;
            Destroy(gameObject, 2.5f);

            var crossbow = FindObjectOfType<CrossbowTower>(); 
            if (crossbow != null)
            {
                var childCollider = crossbow.GetComponentInChildren<CircleCollider2D>();
                if (childCollider != null)
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), childCollider);
            }
        }


        private void OnCollisionEnter2D(Collision2D collision) => Destroy(gameObject);

    }
}

