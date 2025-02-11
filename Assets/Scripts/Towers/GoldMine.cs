using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldMine : MonoBehaviour
{

    [Header("Gold Mine Settings: ")]
    [SerializeField] private AudioSource goldGrab;
    [SerializeField] private int amountCoinsDug;
    [SerializeField][Range(0, 15)] private float waitTime;
    [SerializeField] private Sprite[] showSprite;
    [SerializeField] private SpriteRenderer showRender;
    [SerializeField] private LayerMask mineMask;

    [Header("Gold Mine Health Settings: ")]
    [SerializeField] private int goldMineLives;
    [SerializeField] private Slider goldMineHealth;

    public int CurrentLives;
    private PlayerUI _playerUI;
    private int _currentAmount;
    private bool _isCollecting;

    private Coroutine _goldMineCoroutine;

    // Start is called before the first frame update
    private void Start()
    {
        CurrentLives = goldMineLives;
        goldMineHealth.value = CurrentLives;

        _playerUI = FindObjectOfType<PlayerUI>();
        showRender.sprite = showSprite[0];
        _goldMineCoroutine = StartCoroutine(GoldMineBehaviour(waitTime));
        _isCollecting = false;
    }

    private void Update()
    {
        goldMineHealth.value = CurrentLives;

        if (Input.GetMouseButtonDown(0) && !_isCollecting)
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.up, Mathf.Infinity, mineMask);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
                CollectCoins();
        }
    }

    private IEnumerator GoldMineBehaviour(float delay)
    {
        while (_currentAmount < 20)
        {
            switch (_currentAmount)
            {
                case 0:
                    showRender.sprite = showSprite[0];
                    yield return new WaitForSeconds(delay);
                    showRender.sprite = showSprite[1];
                    _currentAmount += 5;
                    break;
                case 5:
                    showRender.sprite = showSprite[1];
                    yield return new WaitForSeconds(delay);
                    showRender.sprite = showSprite[2];
                    _currentAmount += 5;
                    break;
                case 10:
                    showRender.sprite = showSprite[2];
                    yield return new WaitForSeconds(delay);
                    showRender.sprite = showSprite[3];
                    _currentAmount += 5;
                    break;
                case 15:
                    showRender.sprite = showSprite[3];
                    yield return new WaitForSeconds(delay);
                    showRender.sprite = showSprite[4];
                    _currentAmount += 5;
                    break;
            }
        }

        showRender.sprite = showSprite[4]; 
    }

    private void CollectCoins()
    {
        _isCollecting = true;

        if (_currentAmount > 0)
        {
            goldGrab.Play();
            _playerUI.GiveCoinAmount(_currentAmount);
            _currentAmount = 0;

            if (_goldMineCoroutine != null)
                StopCoroutine(_goldMineCoroutine);
            _goldMineCoroutine = StartCoroutine(GoldMineBehaviour(waitTime));
        }

        _isCollecting = false;
    }
}
