using Unity.Netcode;
using UnityEngine;


public static class AttackManager
{
#pragma warning disable UDR0001
    private static DefenseWindowParameters defenseWindow;
    private static float attackImpactGlobalTime;

    private static DefenseResult defenseResult;
    private static int skillId;

    public static bool CanDefend => attackImpactGlobalTime > Time.unscaledTime;
#pragma warning restore UDR0001


    public static void StartAttackSequence(int incomingSkillId)
    {
        SkillAttack skill = SkillManager.GlobalSkillList[incomingSkillId].AsAttack();

        defenseWindow = skill.DefenseWindows;
        attackImpactGlobalTime = Time.unscaledTime + skill.AttackStartupTime;

        defenseResult = DefenseResult.None;
        skillId = incomingSkillId;

        ExtensionMethods.Invoke(NetworkManager.Singleton, skill.AttackStartupTime, () =>
        {
            if (CombatManager.Instance != null)
            {
                CombatManager.Instance.ResolveAttack_OnDefender(skillId, defenseResult);
            }
        });
    }

    /// <returns>Whether the defense action succesfully defends the attack</returns>
    public static DefenseResult DoDefendAction(DefenseType defenseType)
    {
        if (CanDefend == false) return DefenseResult.None;

        float timeBeforeAttackImpact = attackImpactGlobalTime - Time.unscaledTime;

        defenseResult = defenseType switch
        {
            DefenseType.Dodge => ResolveDodge(timeBeforeAttackImpact),
            DefenseType.Parry => ResolveParry(timeBeforeAttackImpact),
            _ => DefenseResult.None
        };
        return defenseResult;
    }
    private static DefenseResult ResolveDodge(float timeBeforeAttackImpact)
    {
        if (defenseWindow.Dodge > timeBeforeAttackImpact)
        {
            return DefenseResult.Dodged;
        }
        return DefenseResult.None;
    }
    private static DefenseResult ResolveParry(float timeBeforeAttackImpact)
    {
        if (defenseWindow.PerfectParry > timeBeforeAttackImpact)
        {
            return DefenseResult.PerfectParried;
        }
        if (defenseWindow.Parry > timeBeforeAttackImpact)
        {
            return DefenseResult.Parried;
        }
        return DefenseResult.None;
    }
}