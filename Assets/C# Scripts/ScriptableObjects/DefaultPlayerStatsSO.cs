using UnityEngine;



[CreateAssetMenu(fileName = "New Player Stats", menuName = "ScriptableObjects/DefaultPlayerStatsSO", order = -1000)]
public class DefaultPlayerStatsSO : ScriptableObject
{
    [SerializeField] private float health = 100;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private int energy = 4;
    [SerializeField] private int maxEnergy = 10;

    public float MaxHealth => maxHealth;
    public int MaxEnergy => maxEnergy;
    public PlayerStats GetStatsCopy() => new PlayerStats(health, energy);
}