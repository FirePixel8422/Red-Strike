using Fire_Pixel.Utility;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Quick Time Event UI Block. Handles displaying a single UI popup, which is used for Defending against attacks
/// </summary>
public class DefenseUIBlock : MonoBehaviour
{
    [SerializeField] private Image timerBar;
    [SerializeField] private Image timerBarCover;
    [SerializeField] private float timerBarSize;

    [Header("A")]
    [SerializeField] private Image succesBarA;
    [SerializeField] private Image succesBarCoverA;

    [Header("B")]
    [SerializeField] private Image succesBarB;
    [SerializeField] private Image succesBarCoverB;

    [Header("C")]
    [SerializeField] private Image succesBarC;
    [SerializeField] private Image succesBarCoverC;

    private Animator anim;
    private float qteDuration;

    private static readonly int ACTIVATE_ANIM_HASH = Animator.StringToHash("Enabled");
    private static readonly int SUCCEED_ANIM_HASH = Animator.StringToHash("Succeed");
    private static readonly int FAIL_ANIM_HASH = Animator.StringToHash("Fail");
    private static readonly int EXPIRE_ANIM_HASH = Animator.StringToHash("Expire");


    public void Init()
    {
        anim = GetComponent<Animator>();
    }

    public void Activate(float qteDuration, float qteStartDelay, DefenseWindowParameters windowParams)
    {
        anim.SetBool(ACTIVATE_ANIM_HASH, true);

        timerBar.fillAmount = 1;
        timerBarCover.fillAmount = 1 - timerBarSize;

        succesBarA.fillAmount = (windowParams.Dodge / qteDuration);
        succesBarB.fillAmount = windowParams.Parry / qteDuration;
        succesBarC.fillAmount = windowParams.PerfectParry / qteDuration;

        this.qteDuration = qteDuration;

        this.Invoke(qteStartDelay + timerBarSize * qteDuration, () =>
        {
            CallbackScheduler.RegisterUpdate(DepleteTimer);
        });
    }
    public void Disable()
    {
        anim.SetBool(ACTIVATE_ANIM_HASH, false);
        anim.SetBool(SUCCEED_ANIM_HASH, false);
        anim.SetBool(FAIL_ANIM_HASH, false);
        anim.SetBool(EXPIRE_ANIM_HASH, false);
    }
    public void SucceedQTE()
    {
        anim.SetBool(SUCCEED_ANIM_HASH, true);
        CallbackScheduler.UnRegisterUpdate(DepleteTimer);
    }
    public void FailQTE(bool isFailedBecauseExpired)
    {
        if (isFailedBecauseExpired)
        {
            anim.SetBool(EXPIRE_ANIM_HASH, true);

            timerBar.fillAmount = 0;
            timerBarCover.fillAmount = 0;
        }
        else
        {
            anim.SetBool(FAIL_ANIM_HASH, true);
        }
        CallbackScheduler.UnRegisterUpdate(DepleteTimer);
    }

    /// <summary>
    /// Called every frame while timer is depleting. Depletes the timer bar and succes bar based on the time left in the QTE and the succes window.
    /// </summary>
    private void DepleteTimer()
    {
        float barPercentageLeft = math.clamp(timerBar.fillAmount - Time.deltaTime / qteDuration, 0, float.MaxValue);

        timerBar.fillAmount = barPercentageLeft;

        // Follow the timer bar until it reached the succes window, then stay at the succes window until the end of the timer.
        timerBarCover.fillAmount = barPercentageLeft - timerBarSize;

        // After timer bar reaches the succes window, follow the timer bar with the copySuccesBarOverlay until the end of the timer.
        succesBarA.fillAmount = math.clamp(barPercentageLeft - timerBarSize, 0, succesBarCoverA.fillAmount);
        succesBarB.fillAmount = math.clamp(barPercentageLeft - timerBarSize, 0, succesBarCoverB.fillAmount);
        succesBarC.fillAmount = math.clamp(barPercentageLeft - timerBarSize, 0, succesBarCoverC.fillAmount);
    }

    private void OnDestroy()
    {
        CallbackScheduler.UnRegisterUpdate(DepleteTimer);
    }

#if UNITY_EDITOR
    [Range(0, 1)]
    [SerializeField] private float DEBUG_OverrideFill;

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        timerBar.fillAmount = DEBUG_OverrideFill;

        // Follow the timer bar until it reached the succes window, then stay at the succes window until the end of the timer.
        timerBarCover.fillAmount = DEBUG_OverrideFill - timerBarSize;

        // After timer bar reaches the succes window, follow the timer bar with the copySuccesBarOverlay until the end of the timer.
        succesBarA.fillAmount = math.clamp(DEBUG_OverrideFill - timerBarSize, 0, succesBarCoverA.fillAmount);
        succesBarB.fillAmount = math.clamp(DEBUG_OverrideFill - timerBarSize, 0, succesBarCoverB.fillAmount);
        succesBarC.fillAmount = math.clamp(DEBUG_OverrideFill - timerBarSize, 0, succesBarCoverC.fillAmount);
    }
#endif
}