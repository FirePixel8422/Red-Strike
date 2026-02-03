using UnityEngine;



[CreateAssetMenu(fileName = "New StatusEffectSettings", menuName = "ScriptableObjects/StatusEffectSettingsSO", order = -1000)]
public class StatusEffectSettingsSO : ScriptableObject
{
    public StatusEffectRules StatusRules;
}