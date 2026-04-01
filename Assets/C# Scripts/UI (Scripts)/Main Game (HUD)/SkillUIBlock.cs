using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillUIBlock : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    [SerializeField] private ResourceUI[] resourceCostUIs;
    [SerializeField] private float disabledAlpha = 0.4f;


    private int currentSkillId = -1;
    private int currentResourceCostId = -1;
    private bool canAfford;

    private CanvasGroup canvasGroup;



    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
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
        if (button.interactable == false || canAfford == false) return;

        SkillUIManager.Instance.UpdateSkillUIActiveState(false);

        CombatManager.Instance.UseSkill_OnNetwork(currentSkillId);
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

            resourceCostUIs[playerResourceId].Enable(skill.Costs.Amount);
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

        canvasGroup.alpha = canUseSkill ? 1 : disabledAlpha;
    }


    [System.Serializable]
    public class ResourceUI
    {
        [SerializeField] private GameObject gameObject;
        [SerializeField] private TextMeshProUGUI text;

        public void Enable(int resourceCost)
        {
            gameObject.SetActiveSmart(true);

            text.text = resourceCost.ToString();
        }
        public void Disable()
        {
            gameObject.SetActiveSmart(false);
        }
    }
}