using UnityEngine;



public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] weaponObjs;
    private int cWeaponId = -1;


    public void SwapToWeapon(int newWeaponId)
    {
        if (cWeaponId != -1)
        {
            weaponObjs[cWeaponId].SetActive(false);
        }
        cWeaponId = newWeaponId;
        weaponObjs[cWeaponId].SetActive(false);
    }
}