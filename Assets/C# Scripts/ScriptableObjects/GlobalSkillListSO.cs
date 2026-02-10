using UnityEngine;



[CreateAssetMenu(fileName = "New GlobalSkillList", menuName = "ScriptableObjects/GlobalDataLists/SkillListSO", order = -1000)]
public class GlobalSkillListSO : ScriptableObject
{
    public SkillBaseSO[] SkillList;
}