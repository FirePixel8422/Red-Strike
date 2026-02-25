using Fire_Pixel.Utility;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class QTEUIBlock : MonoBehaviour
{
    [SerializeField] private Image timerBar;
    [SerializeField] private Image succesBar;
    [SerializeField] private TextMeshProUGUI qteKeyText;

    private Animator anim;
    private float qteDuration;
    private float qteWindow01;

    private static readonly int ACTIVATE_ANIM_HASH = Animator.StringToHash("Enabled");
    private static readonly int SUCCEED_ANIM_HASH = Animator.StringToHash("Succeed");
    private static readonly int FAIL_ANIM_HASH = Animator.StringToHash("Fail");


    public void Init(InputActionReference qteInput)
    {
        anim = GetComponent<Animator>();

        qteKeyText.text = qteInput.action.GetBindingDisplayString(0);
    }

    public void Activate(float _qteDuration, float _qteWindow01)
    {
        anim.SetBool(ACTIVATE_ANIM_HASH, true);

        timerBar.fillAmount = 1;
        succesBar.fillAmount = _qteWindow01;
        UpdateScheduler.RegisterUpdate(DepleteTimer);

        qteDuration = _qteDuration;
        qteWindow01 = _qteWindow01;
    }
    public void Disable()
    {
        anim.SetBool(ACTIVATE_ANIM_HASH, false);
        anim.SetBool(SUCCEED_ANIM_HASH, false);
        anim.SetBool(FAIL_ANIM_HASH, false);
    }
    public void SucceedQTE()
    {
        anim.SetBool(SUCCEED_ANIM_HASH, true);
        UpdateScheduler.UnRegisterUpdate(DepleteTimer);
    }
    public void FailQTE()
    {
        anim.SetBool(FAIL_ANIM_HASH, true);
        UpdateScheduler.UnRegisterUpdate(DepleteTimer);
    }

    /// <summary>
    /// Called every frame while registered to <see cref="UpdateScheduler"/>.
    /// </summary>
    private void DepleteTimer()
    {
        float barPercentageLeft = math.saturate(timerBar.fillAmount - Time.deltaTime / qteDuration);
        timerBar.fillAmount = barPercentageLeft;

        if (qteWindow01 > barPercentageLeft)
        {
            succesBar.fillAmount = barPercentageLeft;
        }
    }
}