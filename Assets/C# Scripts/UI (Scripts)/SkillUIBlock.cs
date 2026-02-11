using Fire_Pixel.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillUIBlock : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    [SerializeField] private ResourceUI[] resourceCostUIs;
    private int currentSkillId = -1;
    private int currentResourceCostId = -1;
    private bool canAfford;


    private const float DISABLED_ALPHA = 0.05f;


    private void Awake()
    {
        button.onClick.AddListener(AttackWithTargetSkill);
    }
    private void AttackWithTargetSkill()
    {
        CombatManager.Instance.Attack_ServerRPC(currentSkillId);
    }

    /// <summary>
    /// Update UISkillBlock title, description and costs UI based on new skill data.
    /// </summary>
    public void UpdateUI(SkillBase skill)
    {
        currentSkillId = skill.Id;

        title.text = skill.Info.Name;
        description.text = skill.Info.Description;

        if (skill.Costs.Amount <= 0)
        {
            // Disable potential previous selected resourceUIBlock
            if (currentResourceCostId != -1)
            {
                resourceCostUIs[currentResourceCostId].Disable();
            }
            currentResourceCostId = -1;

            canAfford = true;
            return;
        }

        int playerResourceId = (int)skill.Costs.Type;
        resourceCostUIs[playerResourceId].Enable(skill.Costs.Amount);

        canAfford = PlayerStats.Local.Resources[playerResourceId] >= skill.Costs.Amount;

        // Disable potential previous selected resourceUIBlock
        if (currentResourceCostId != -1)
        {
            resourceCostUIs[currentResourceCostId].Disable();
        }
        currentResourceCostId = playerResourceId;
    }

    /// <summary>
    /// Update SkillUIBlock ActiveState based on <paramref name="isActive"/> and if the skill is useable according to <see cref="SkillCosts"/> and <see cref="PlayerStats.Resources"/>.
    /// </summary>
    public void UpdateSkillActiveState(bool isActive)
    {
        button.interactable = canAfford && isActive;

        SetAlpha(title, isActive ? 1f : DISABLED_ALPHA);
        SetAlpha(description, isActive ? 1f : DISABLED_ALPHA);
    }
    private void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }


    [System.Serializable]
    public class ResourceUI
    {
        public void Enable(int resourceCost)
        {
            text.text = resourceCost.ToString();
            GameObject.SetActiveSmart(true);
        }
        public void Disable()
        {
            GameObject.SetActiveSmart(false);
        }

        [SerializeField] private GameObject GameObject;
        [SerializeField] private TextMeshProUGUI text;
    }
}