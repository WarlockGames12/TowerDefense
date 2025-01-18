using UnityEngine;

public class HoverGrass : MonoBehaviour
{

    [Header("Hover Over Grass Settings: ")]
    [SerializeField] private AudioSource select;
    [SerializeField] private Color hoverColor = Color.green;
    [SerializeField] private Color originalColor;
    [SerializeField] private SpriteRenderer grass;

    private bool _playOnce;

    private void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hitCollider = Physics2D.OverlapPoint(mousePos);

        if (hitCollider != null && hitCollider.gameObject == gameObject)
        {
            if (!_playOnce)
            {
                _playOnce = true;
                select.Play();
            }
            
            grass.color = hoverColor;
        }
        else
        {
            _playOnce = false;
            grass.color = originalColor;
        } 
    }
}
