using UnityEngine;


public static class SupportQTEManager
{
#pragma warning disable UDR0001
    private static QTEWindowParameters qteParameters;
    private static float nextQTEGlobalTime;

    private static QTEResult qteResult;
    private static int skillId;

    public static bool CanDoQTE { get; private set; }
#pragma warning restore UDR0001


    public static void StartSupportSequence(int supportSkillId)
    {
        CanDoQTE = true;

        SkillSupport skill = SkillManager.GlobalSkillList[supportSkillId].AsSupport();

        qteParameters = skill.QTEWindowParameters;
        nextQTEGlobalTime = Time.time + qteParameters.StartDelay;

        qteResult = QTEResult.None;
        skillId = supportSkillId;

        //ExtensionMethods.Invoke(NetworkManager.Singleton, , () =>
        //{
        //    if (CombatManager.Instance != null)
        //    {
        //        CombatManager.Instance.ResolveAttack_OnDefender(skillId, defenseResult);
        //    }
        //});
    }

    /// <returns>Whether the defense action succesfully defends the attack</returns>
    public static QTEResult DoQuickTimeEvent(DefenseType defenseType)
    {
        float timeBeforeAttackImpact = nextQTEGlobalTime - Time.time;

        return qteResult;
    }
}