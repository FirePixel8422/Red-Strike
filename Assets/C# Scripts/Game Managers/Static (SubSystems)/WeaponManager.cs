


public static class WeaponManager
{
    public static WeaponSkillSetData[] WeaponSkillSetData { get; private set; }
    public static WeaponSkillSetData ActiveWeapon_Local { get; private set; }


    public static void Init(GlobalWeaponListSO globalWeaponListSO)
    {
        int weaponCount = globalWeaponListSO.WeaponList.Length;
        WeaponSkillSetData = new WeaponSkillSetData[weaponCount];

        for (int weaponId = 0; weaponId < weaponCount; weaponId++)
        {
            WeaponSkillSetData[weaponId] = globalWeaponListSO.WeaponList[weaponId].GetAsDataCopy(weaponId);
        }
    }

    public static int GetRandomWeaponId()
    {
        return EzRandom.Range(0, WeaponSkillSetData.Length);
    }
    public static void SetLocalWeapon(int weaponId)
    {
        ActiveWeapon_Local = WeaponSkillSetData[weaponId];

        SkillUIManager.UpdateSkillUI(ActiveWeapon_Local);
    }
}