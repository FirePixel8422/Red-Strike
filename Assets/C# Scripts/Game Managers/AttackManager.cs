using Unity.Netcode;
using UnityEngine;


public static class AttackManager
{
#pragma warning disable UDR0001
    public static DefenseWindowParameters defenseWindow;
    public static float attackImpactGlobalTime;
    
    public static DefenseResult defenseResult;
    public static int skillId;
#pragma warning restore UDR0001


    public static void StartAttackSequence(int incomingSkillId)
    {
        SkillBase skill = SkillManager.GlobalSkillList[incomingSkillId];

        defenseWindow = skill.DefenseWindows;
        attackImpactGlobalTime = Time.time + skill.AttackStartupTime;

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
        float timeBeforeAttackImpact = attackImpactGlobalTime - Time.time;

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