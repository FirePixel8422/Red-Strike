using UnityEngine;



[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/WeaponSO", order = -1000)]
public class WeaponSO : ScriptableObject
{
    public Weapon Weapon;
}