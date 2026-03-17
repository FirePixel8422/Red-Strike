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
            GameObject obj = transform.GetChild(i).gameObject;
            obj.SetActive(false);

            weaponObjs[i] = obj;
        }
    }

    public void SwapToWeapon(int newWeaponId)
    {
        if (cWeaponId != -1)
        {
            weaponObjs[cWeaponId].SetActive(false);
        }
        cWeaponId = newWeaponId;
        weaponObjs[cWeaponId].SetActive(true);
    }
}