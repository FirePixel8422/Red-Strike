using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillUIBlock : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    private const float DISABLED_ALPHA = 0.1f;


    public void UpdateUI(SkillInfo skill)
    {
        title.text = skill.Name;
        description.text = skill.Description;
    }

    public void UpdateSkillActiveState(bool isActive)
    {
        button.enabled = isActive;

        Color col = title.color;
        col.a = isActive ? 1 : DISABLED_ALPHA;
        title.color = col;

        col = description.color;
        col.a = isActive ? 1 : DISABLED_ALPHA;
        description.color = col;
    }
}