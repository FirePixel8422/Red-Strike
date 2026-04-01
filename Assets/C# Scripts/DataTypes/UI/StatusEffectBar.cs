using TMPro;
using UnityEngine;



[System.Serializable]
public class StatusEffectBar
{
    [SerializeField] private TextMeshProUGUI[] statusEffectText;


    public void UpdateStatusUI(StatusEffectType type, int stackCount)
    {
        int id = (int)type;

        statusEffectText[id].text = stackCount.ToString() + "X";
        statusEffectText[id].transform.parent.gameObject.SetActive(stackCount != 0);
    }
}