using UnityEngine;



[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Combat/WeaponSO", order = -1005)]
public class WeaponSO : ScriptableObject
{
    [SerializeField] private string weaponName;
    [SerializeField] private WeaponSkill[] skills = new WeaponSkill[3];

    public string WeaponName => weaponName;
    public WeaponSkillSetData GetAsDataCopy() => new WeaponSkillSetData(skills, weaponName);


#if UNITY_EDITOR
    [Header(">>Warning<<: changes will save back to the skill data.")]
    [SerializeReference] private SkillBase[] Debug_Skills;

    private void OnValidate()
    {
        weaponName = name;
        if (skills.Length < 3)
        {
            DebugLogger.LogWarning("Weapons must have AT LEAST 3 skills");
            System.Array.Resize(ref skills, 3);
        }

        int skillCount = skills.Length;
        Debug_Skills = new SkillBase[skillCount];
        for (int i = 0; i < skillCount; i++)
        {
            Debug_Skills[i] = skills[i].SkillSO.Skill;
        }
    }
#endif
}

[System.Serializable]
public struct WeaponSkill
{
    public SkillBaseSO SkillSO;
    public string animationName;
}

/// <summary>
/// A datatype acting as a Weapon, holding X skills in an optimized and quick accesable layout.
/// </summary>
[System.Serializable]
public struct WeaponSkillSetData
{
    public string WeaponName;
    public SkillBase[] SkillData;
    public int[] AnimHashes;


    public WeaponSkillSetData(WeaponSkill[] weaponSkills, string weaponName)
    {
        WeaponName = weaponName;

        int skillCount = weaponSkills.Length;
        SkillData = new SkillBase[skillCount];
        AnimHashes = new int[skillCount];

        for (int i = 0; i < skillCount; i++)
        {
            SkillData[i] = weaponSkills[i].SkillSO.Skill;
            AnimHashes[i] = Animator.StringToHash(weaponSkills[i].animationName);
        }
    }

    public readonly void RandomizeSkillOrder()
    {
        SkillData.Shuffle();
    }
    public readonly int Length => SkillData.Length;
}