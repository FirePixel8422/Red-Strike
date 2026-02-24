using Unity.Netcode;
using UnityEngine;


public static class QuickTimeEventManager
{
#pragma warning disable UDR0001
    private static QTEWindowParameters qteWindow;
    private static float nextQTEGlobalTime;

    private static QTEResult qteResult;
    private static int skillId;

    public static bool CanDoQTE => nextQTEGlobalTime > Time.unscaledTime;
#pragma warning restore UDR0001


    public static void StartSupportSequence(int supportSkillId)
    {
        SkillSupport skill = SkillManager.GlobalSkillList[supportSkillId].AsSupport();

        qteWindow = skill.QTEWindowParameters;
        nextQTEGlobalTime = Time.time + qteWindow.StartDelay;

        qteResult = QTEResult.None;
        skillId = supportSkillId;

        float totalQTEDuration = qteWindow.StartDelay + qteWindow.Delay * qteWindow.Count;
        ExtensionMethods.Invoke(NetworkManager.Singleton, totalQTEDuration, () =>
        {
            if (CombatManager.Instance != null)
            {
                CombatManager.Instance.ResolveQTE_OnAttacker(skillId, qteResult);
            }
        });
    }

    /// <returns>Whether the defense action succesfully defends the attack</returns>
    public static QTEResult DoQuickTimeEvent()
    {
        float timeBeforeQTEWindowEnd = nextQTEGlobalTime - Time.time;

        qteResult = ResolveQTEWindow(timeBeforeQTEWindowEnd);

        return qteResult;
    }

    private static QTEResult ResolveQTEWindow(float timeBeforeQTEWindowEnd)
    {
        if (qteWindow.PerfectWindow > timeBeforeQTEWindowEnd)
        {
            return QTEResult.Perfect;
        }
        if (qteWindow.SuccesWindow > timeBeforeQTEWindowEnd)
        {
            return QTEResult.Success;
        }
        return QTEResult.None;
    }
}