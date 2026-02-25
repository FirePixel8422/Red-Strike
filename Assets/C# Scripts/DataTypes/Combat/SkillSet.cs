using UnityEngine;


/// <summary>
/// A datatype acting as a Weapon, holding X skills in an optimized and quick accesable layout.
/// </summary>
[System.Serializable]
public struct SkillSet
{
    [SerializeReference] private SkillBase[] Skills;

    public SkillSet(SkillBaseSO[] skillsSOs)
    {
        int skillCount = skillsSOs.Length;
        Skills = new SkillBase[skillCount];

        for (int i = 0; i < skillCount; i++)
        {
            Skills[i] = skillsSOs[i].Skill;
        }
    }

    public void RandomizeSkillOrder()
    {
        Skills.Shuffle();
    }
    public SkillBase this[int i] => Skills[i];
    public int Length => Skills.Length;
}