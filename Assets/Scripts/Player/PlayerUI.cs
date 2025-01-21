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

    private int originalHealth;
    private int _currentCoinAmount;

    private void Start()
    {
        originalHealth = playerHealth;
        _currentCoinAmount = startCoinAmount;
    }

    private void Update()
    {
        playerHealthText.text = playerHealth + "/" + originalHealth;
        healthSlider.value = playerHealth;

        coinText.text = "Coins: " + _currentCoinAmount;
    }

    public void DepleteHealth() => playerHealth -= 5;
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
