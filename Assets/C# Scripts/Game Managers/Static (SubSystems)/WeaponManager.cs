


public static class WeaponManager
{
    public static WeaponSkillSetData[] SkillSets { get; private set; }
    public static WeaponSkillSetData ActiveWeapon_Local { get; private set; }


    public static void Init(GlobalWeaponListSO globalWeaponListSO)
    {
        int weaponCount = globalWeaponListSO.WeaponList.Length;
        SkillSets = new WeaponSkillSetData[weaponCount];

        for (int i = 0; i < weaponCount; i++)
        {
            SkillSets[i] = globalWeaponListSO.WeaponList[i].GetAsDataCopy();
        }
    }

    public static int GetRandomWeaponId()
    {
        return EzRandom.Range(0, SkillSets.Length);
    }
    public static void SetLocalWeapon(int weaponId)
    {
        ActiveWeapon_Local = SkillSets[weaponId];

        SkillUIManager.UpdateSkillUI(ActiveWeapon_Local);
    }
}