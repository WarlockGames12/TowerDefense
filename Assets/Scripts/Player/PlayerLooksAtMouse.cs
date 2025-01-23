using UnityEngine;

namespace Player
{
    public class PlayerLooksAtMouse : MonoBehaviour
    {

        [Header("Players Looks at Mouse Settings: ")]
        [SerializeField] [Range(0, 500)] private float lookSpeed;

        private Vector2 _mousePos;
        private Camera _cam;

        // Update is called once per frame
        private void Start() =>  _cam = Camera.main;
        private void Update()
        {
            _mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            var dir = _mousePos - (Vector2)transform.position;

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var targetRot = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, lookSpeed * Time.deltaTime); 
        }
    }
}
