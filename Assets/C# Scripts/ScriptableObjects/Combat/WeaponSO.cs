using UnityEngine;



[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Combat/WeaponSO", order = -1005)]
public class WeaponSO : ScriptableObject
{
    [SerializeField] private string weaponName;
    public WeaponSkillEntry[] Skills = new WeaponSkillEntry[3];

    public WeaponSkillSetData GetAsDataCopy(int assignedWeaponId) => new WeaponSkillSetData(Skills, weaponName, assignedWeaponId);


#if UNITY_EDITOR
    [Header(">>Warning<<: changes will save back to the skill data.")]
    [SerializeReference] private SkillBase[] Debug_Skills;

    private void OnValidate()
    {
        weaponName = name;
        if (Skills.Length < 3)
        {
            DebugLogger.LogWarning("Weapons must have AT LEAST 3 Skills");
            System.Array.Resize(ref Skills, 3);
        }

        int skillCount = Skills.Length;
        Debug_Skills = new SkillBase[skillCount];
        for (int i = 0; i < skillCount; i++)
        {
            if (Skills[i].SkillSO == null) continue;

            Debug_Skills[i] = Skills[i].SkillSO.Skill;
        }
    }
#endif
}

[System.Serializable]
public struct WeaponSkillEntry
{
    public SkillBaseSO SkillSO;
    public string AnimationName;
    public float AttackStartupTime;
}

/// <summary>
/// A datatype acting as a Weapon, holding X Skills in an optimized and quick accesable layout.
/// </summary>
[System.Serializable]
public readonly struct WeaponSkillSetData
{
    public readonly int WeaponId;
    public readonly string WeaponName;

    public readonly SkillBase[] SkillData;
    public readonly int[] AnimHashes;


    public WeaponSkillSetData(WeaponSkillEntry[] weaponSkills, string weaponName, int assignedWeaponId)
    {
        WeaponId = assignedWeaponId;
        WeaponName = weaponName;

        int skillCount = weaponSkills.Length;
        SkillData = new SkillBase[skillCount];
        AnimHashes = new int[skillCount];

        for (int i = 0; i < skillCount; i++)
        {
            SkillData[i] = weaponSkills[i].SkillSO.Skill;
            AnimHashes[i] = Animator.StringToHash(weaponSkills[i].AnimationName);
        }
    }
    public readonly int Length => SkillData.Length;

    public void RandomizeSkillOrder()
    {
        int length = SkillData.Length;

        for (int i = length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            (SkillData[j], SkillData[i]) = (SkillData[i], SkillData[j]);
            (AnimHashes[j], AnimHashes[i]) = (AnimHashes[i], AnimHashes[j]);
        }
    }
}