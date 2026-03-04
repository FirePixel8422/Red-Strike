using Fire_Pixel.Utility;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


/// <summary>
/// Quick Time Event UI Block. Handles displaying a single UI popup, which is used for support skills.
/// </summary>
public class QTEUIBlock : MonoBehaviour
{
    [SerializeField] private Image timerBar;
    [SerializeField] private Image timerBarCoverA, timerBarCoverB;
    [SerializeField] private Image succesBar;
    [SerializeField] private float timerBarSize;

    [SerializeField] private TextMeshProUGUI qteKeyText;

    private Animator anim;
    private float qteDuration;

    private static readonly int ACTIVATE_ANIM_HASH = Animator.StringToHash("Enabled");
    private static readonly int SUCCEED_ANIM_HASH = Animator.StringToHash("Succeed");
    private static readonly int FAIL_ANIM_HASH = Animator.StringToHash("Fail");
    private static readonly int EXPIRE_ANIM_HASH = Animator.StringToHash("Expire");


    public void Init(InputActionReference qteInput)
    {
        anim = GetComponent<Animator>();
        qteKeyText.text = qteInput.action.GetBindingDisplayString(0);
    }

    public void Activate(float qteDuration, float qteWindow01, float qteUIStartAheadTime)
    {
        anim.SetBool(ACTIVATE_ANIM_HASH, true);

        timerBar.fillAmount = 1;
        timerBarCoverA.fillAmount = 1 - timerBarSize;

        succesBar.fillAmount = qteWindow01;
        timerBarCoverB.fillAmount = qteWindow01;

        this.qteDuration = qteDuration;

        this.Invoke(qteUIStartAheadTime + timerBarSize * qteDuration, () =>
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
            timerBarCoverA.fillAmount = 0;
            timerBarCoverB.fillAmount = 0;
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
        timerBarCoverA.fillAmount = barPercentageLeft - timerBarSize;
        // After timer bar reaches the succes window, follow the timer bar with the copySuccesBarOverlay until the end of the timer.
        timerBarCoverB.fillAmount = math.clamp(barPercentageLeft - timerBarSize, 0, succesBar.fillAmount);
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

        float succesBarSize = succesBar.fillAmount;

        timerBar.fillAmount = DEBUG_OverrideFill;

        // Follow the timer bar until it reached the succes window, then stay at the succes window until the end of the timer.
        timerBarCoverA.fillAmount = DEBUG_OverrideFill - timerBarSize;
        // After timer bar reaches the succes window, follow the timer bar with the copySuccesBarOverlay until the end of the timer.
        timerBarCoverB.fillAmount = math.clamp(DEBUG_OverrideFill - timerBarSize, 0, succesBarSize);
    }
#endif
}