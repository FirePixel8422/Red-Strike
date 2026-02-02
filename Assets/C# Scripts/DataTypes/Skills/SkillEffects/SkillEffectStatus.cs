



[System.Serializable]
public class SkillEffectStatus : SkillEffectBase
{
    public StatusEffect StatusEffect;

    protected override void Resolve(CombatContext ctx)
    {
        ctx.Defender.AddStatusEffect(StatusEffect);
    }
}