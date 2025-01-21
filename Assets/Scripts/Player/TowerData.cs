using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "TowerData", order = 1)]
public class TowerData : ScriptableObject
{
    public int towerID;
    public int cost;
    public GameObject prefab;
}
