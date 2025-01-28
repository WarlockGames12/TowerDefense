using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    [Header("Player Health Settings: ")]
    [SerializeField] private int playerHealth;
    [SerializeField] private Text playerHealthText;
    [SerializeField] private Slider healthSlider;

    [Header("Player Coins System: ")]
    [SerializeField] private Text coinText;
    [SerializeField] [Range(0, 500)] private int startCoinAmount;

    [Header("Store Transform Settings: ")]
    [SerializeField] private Transform storedTileTransform;
    [SerializeField] private SelectionSystem selectionSystem;

    [Header("Game Over Settings: ")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private Animator lateAnim;

    [Header("Damage Sprite's Settings: ")]
    [SerializeField] private AudioSource damage;
    [SerializeField] private SpriteRenderer[] allCastlesRenders;

    private int originalHealth;
    private int _currentCoinAmount;
    private bool isFlashing;

    private void Start()
    {
        originalHealth = playerHealth;
        _currentCoinAmount = startCoinAmount;
        lateAnim.enabled = false;
    }

    private void Update()
    {
        playerHealthText.text = playerHealth + "/" + originalHealth;
        healthSlider.value = playerHealth;

        coinText.text = "Coins: " + _currentCoinAmount;

        if (healthSlider.value <= 0)
            StartCoroutine(GameOverSequence(2));
    }

    private IEnumerator GameOverSequence(float delay)
    {
        Time.timeScale = 0;
        gameOverScreen.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        lateAnim.gameObject.SetActive(true);
        lateAnim.enabled = true;
    }

    public void DepleteHealth() => StartCoroutine(GiveDamage());

    private IEnumerator GiveDamage()
    {
        playerHealth -= 5;
        if (isFlashing)
            yield break;

        isFlashing = true;
        var flashDur = 0.1f;
        var flashCount = 3;
        var ogColor = new Color[allCastlesRenders.Length];

        damage.Play();
        for (int i = 0; i < allCastlesRenders.Length; i++)
            ogColor[i] = allCastlesRenders[i].color;
        for (var flash = 0; flash < flashCount; flash++)
        {
            for (var i = 0; i < allCastlesRenders.Length; i++)
                allCastlesRenders[i].color = Color.red;
            yield return new WaitForSeconds(flashDur);
            for (var i = 0; i < allCastlesRenders.Length; i++)
                allCastlesRenders[i].color = ogColor[i];
            yield return new WaitForSeconds(flashDur);
        }

        isFlashing = false;
    }

    public void GiveCoinAmount(int amount) => _currentCoinAmount += amount;
    public bool DepleteCoinAmount(int amount)
    {
        if (_currentCoinAmount < amount)
            return false;

        _currentCoinAmount -= amount;
        return true;
    }

    public void SpawnTower(TowerData towerData)
    {
        if (DepleteCoinAmount(towerData.cost))
        {
            Time.timeScale = 1;
            Instantiate(towerData.prefab, storedTileTransform.position, storedTileTransform.rotation, storedTileTransform);
            storedTileTransform.GetComponent<HoverGrass>()._isBuildUIActive = false;
            storedTileTransform = null;
            selectionSystem.gameObject.SetActive(false);
        }
        else
            Debug.Log("Not enough coins to spawn this tower.");
    }
    public Transform StoreTileTransform(Transform tileTransform)
    {
        storedTileTransform = tileTransform;
        return storedTileTransform;
    }
}
