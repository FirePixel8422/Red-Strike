using UnityEngine;



[CreateAssetMenu(fileName = "New ToolTips list", menuName = "ScriptableObjects/ToolTipsSO", order = -1000)]
public class ToolTipsSO : ScriptableObject
{
    public ToolTipWord[] ToolTips;
}