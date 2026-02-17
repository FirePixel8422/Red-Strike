


/// <summary>
/// Base class for modular effects that run when a skill is used.
/// Skill effects operate on <see cref="CombatTurnContext"/> to modify damage,
/// apply status effects, or execute additional combat logic.
/// </summary>
[System.Serializable]
public abstract class SkillBaseEffect
{
    public virtual void Resolve(DefenseAbsorptionParameters absorptionParams) { }
}