using UnityEngine;



[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Combat/WeaponSO", order = -1005)]
public class WeaponSO : ScriptableObject
{
    [SerializeField] private SkillBaseSO[] skills = new SkillBaseSO[3];
    public SkillSet GetAsSkillSet() => new SkillSet(skills);


    private void OnValidate()
    {
        if (skills.Length < 3)
        {
            DebugLogger.LogWarning("Weapons must have AT LEAST 3 skills");
            System.Array.Resize(ref skills, 3);
        }
    }
}