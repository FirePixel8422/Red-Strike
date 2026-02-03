using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [SerializeField] private BaseSkillSO[] globalSkillSOList;
    public SkillBase[] GlobalSkillList { get; private set; }



    private void Awake()
    {
        Instance = this;

        int skillCount = globalSkillSOList.Length;
        GlobalSkillList = new SkillBase[skillCount];

        for (int i = 0; i < skillCount; i++)
        {
            SkillBase skill = globalSkillSOList[i].Skill;
            skill.SetId(i);

            GlobalSkillList[i] = skill;
        }
    }
}
