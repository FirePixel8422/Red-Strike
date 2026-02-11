


public static class WeaponManager
{
    public static SkillSet[] SkillSets { get; private set; }
    public static SkillSet ActiveWeapon_Local { get; private set; }


    public static void Init(GlobalWeaponListSO globalWeaponListSO)
    {
        int weaponCount = globalWeaponListSO.WeaponList.Length;
        SkillSets = new SkillSet[weaponCount];

        for (int i = 0; i < weaponCount; i++)
        {
            SkillSets[i] = globalWeaponListSO.WeaponList[i].GetAsSkillSet();
        }
    }
    public static int SwapToRandomWeapon()
    {
        int r = EzRandom.Range(0, SkillSets.Length);
        ActiveWeapon_Local = SkillSets[r];

        SkillUIHandler.UpdateSkillUI(ActiveWeapon_Local);
        return r;
    }
}