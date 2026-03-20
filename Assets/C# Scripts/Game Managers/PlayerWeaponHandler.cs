using UnityEngine;



public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private Transform weaponHolderL, weaponHolderR;
    [SerializeField] private GameObject[] weaponObjsL, weaponObjsR;
    private int cWeaponId = -1;


    private void Awake()
    {
        int childCount = weaponHolderL.childCount;
        weaponObjsL = new GameObject[childCount];
        weaponObjsR = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            GameObject obj = weaponHolderL.GetChild(i).gameObject;
            obj.SetActive(false);

            weaponObjsL[i] = obj;
        }
        for (int i = 0; i < childCount; i++)
        {
            GameObject obj = weaponHolderR.GetChild(i).gameObject;
            obj.SetActive(false);

            weaponObjsR[i] = obj;
        }
    }

    public void SwapToWeapon(int newWeaponId)
    {
        if (cWeaponId != -1)
        {
            weaponObjsL[cWeaponId].SetActive(false);
            weaponObjsR[cWeaponId].SetActive(false);
        }
        cWeaponId = newWeaponId;
        weaponObjsL[cWeaponId].SetActive(true);
        weaponObjsR[cWeaponId].SetActive(true);
    }
}