using UnityEngine;



public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject[] weaponObjs;
    private int cWeaponId = -1;


    private void Awake()
    {
        int childCount = transform.childCount;
        weaponObjs = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            weaponObjs[i] = transform.GetChild(i).gameObject;
        }
    }

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