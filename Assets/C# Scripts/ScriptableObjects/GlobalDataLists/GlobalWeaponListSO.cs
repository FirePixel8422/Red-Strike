using UnityEngine;



[CreateAssetMenu(fileName = "New GlobalWeaponList", menuName = "ScriptableObjects/GlobalDataLists/WeaponListSO", order = -1004)]
public class GlobalWeaponListSO : ScriptableObject
{
    [SerializeField] private WeaponSO[] weaponList;
    public WeaponSO[] WeaponList => weaponList;
}