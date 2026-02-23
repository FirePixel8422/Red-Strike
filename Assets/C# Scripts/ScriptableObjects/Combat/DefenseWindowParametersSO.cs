using UnityEngine;



[CreateAssetMenu(fileName = "New DefenseWindowParameters", menuName = "ScriptableObjects/Combat/DefenseWindowParametersSO", order = -1005)]
public class DefenseWindowParametersSO : ScriptableObject
{
    public DefenseWindowParameters Value = DefenseWindowParameters.Default;
}