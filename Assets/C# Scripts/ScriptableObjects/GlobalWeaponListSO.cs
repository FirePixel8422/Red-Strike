using UnityEngine;



[CreateAssetMenu(fileName = "New GlobalWeaponList", menuName = "ScriptableObjects/GlobalDataLists/WeaponListSO", order = -1000)]
public class GlobalWeaponListSO : ScriptableObject
{
    public WeaponSO[] WeaponList;
}