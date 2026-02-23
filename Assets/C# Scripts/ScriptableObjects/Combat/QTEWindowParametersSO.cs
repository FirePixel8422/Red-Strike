using UnityEngine;



[CreateAssetMenu(fileName = "New QTEWindowParameters", menuName = "ScriptableObjects/Combat/QTEWindowParametersSO", order = -1005)]
public class QTEWindowParametersSO : ScriptableObject
{
    public QTEWindowParameters Value = QTEWindowParameters.Default;
}