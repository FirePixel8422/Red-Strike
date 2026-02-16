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
        UpdateSkillActiveState(false);
    }
    /// <summary>
    /// Enable Button of the skillUIBlock and wire it to the attack system
    /// </summary>
    public void Init()
    {
        button.enabled = true;
        button.onClick.AddListener(UseSkill);
    }

    private void UseSkill()
    {
        SkillUIManager.Instance.UpdateSkillUIActiveState(false);

        SkillCosts skillCosts = SkillManager.GlobalSkillList[currentSkillId].Costs;
        if (skillCosts.Amount > 0)
        {
            switch (skillCosts.Type)
            {
                case PlayerResourceType.Health:
                    PlayerStats.Local.TakeDamage(skillCosts.Amount);
                    break;

                case PlayerResourceType.Energy:
                    PlayerStats.Local.SpendEnergy(skillCosts.Amount);
                    break;

                default:
                    break;
            }
        }
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
            UpdateSkillActiveState(canAfford);
            return;
        }

        int playerResourceId = (int)skill.Costs.Type;
        canAfford = PlayerStats.Local.Resources[playerResourceId] >= skill.Costs.Amount;

        UpdateSkillActiveState(canAfford);

        resourceCostUIs[playerResourceId].Enable(skill.Costs.Amount, canAfford);

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
        bool canUseSkill = canAfford && isActive;
        button.interactable = canUseSkill;

        SetAlpha(title, canUseSkill ? 1f : DISABLED_ALPHA);
        SetAlpha(description, canUseSkill ? 1f : DISABLED_ALPHA);
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
        [SerializeField] private GameObject gameObject;
        [SerializeField] private GameObject darkOverlayObj;
        [SerializeField] private TextMeshProUGUI text;

        public void Enable(int resourceCost, bool canAfford)
        {
            gameObject.SetActiveSmart(true);
            darkOverlayObj.SetActiveSmart(!canAfford);

            text.text = resourceCost.ToString();
        }
        public void Disable()
        {
            gameObject.SetActiveSmart(false);
        }
    }
}