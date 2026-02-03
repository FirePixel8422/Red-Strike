using Unity.Mathematics;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    [SerializeField] private StatusEffectSettingsSO statusEffectsRulesSO;


    private void Awake()
    {
        GameRules.StatusEffectRules = statusEffectsRulesSO.StatusRules;
        DontDestroyOnLoad(gameObject);
    }
}