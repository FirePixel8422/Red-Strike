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
    /// Enable Button of the skillUIDodge and wire it to the attack system
    /// </summary>
    public void Init()
    {
        button.enabled = true;
        button.onClick.AddListener(TryUseSkill);
    }

    public void TryUseSkill()
    {
        if (canAfford == false) return;

        SkillUIManager.Instance.UpdateSkillUIActiveState(false);
        CombatManager.Instance.ResolveSkillUseCosts_Attacker(currentSkillId);
        CombatManager.Instance.Attack_ServerRPC(currentSkillId);
    }

    /// <summary>
    /// Update UISkillDodge title, description and costs UI based on new skill data.
    /// </summary>
    public void UpdateUI(SkillBase skill)
    {
        currentSkillId = skill.Id;

        title.text = skill.Info.Name;
        description.text = skill.Info.Description;

        // Disable potential previous selected resourceUIDodge
        if (currentResourceCostId != -1)
        {
            resourceCostUIs[currentResourceCostId].Disable();
        }
        RecalculateCanAffordSkill();
    }

    /// <summary>
    /// Check skill costs and update UI based on if its affordable or not
    /// </summary>
    public void RecalculateCanAffordSkill()
    {
        if (currentSkillId == -1) return;

        SkillBase skill =  SkillManager.GlobalSkillList[currentSkillId];
        if (skill.Costs.Amount > 0)
        {
            int playerResourceId = (int)skill.Costs.Type;
            canAfford = PlayerStats.Local.Resources[playerResourceId] >= skill.Costs.Amount;

            resourceCostUIs[playerResourceId].Enable(skill.Costs.Amount, canAfford);
            currentResourceCostId = playerResourceId;
        }
        else
        {
            canAfford = true;
            currentResourceCostId = -1;
        }
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