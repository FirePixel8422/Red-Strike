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
    [SerializeField] private Image succesBar;
    [SerializeField] private TextMeshProUGUI qteKeyText;

    private Animator anim;
    private float qteDuration;
    private float qteWindow01;

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
        succesBar.fillAmount = qteWindow01;

        this.qteDuration = qteDuration;
        this.qteWindow01 = qteWindow01;

        this.Invoke(qteUIStartAheadTime, () =>
        {
            UpdateScheduler.RegisterUpdate(DepleteTimer);
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
        UpdateScheduler.UnRegisterUpdate(DepleteTimer);
    }
    public void FailQTE(bool isFailedBecauseExpired)
    {
        if (isFailedBecauseExpired)
        {
            anim.SetBool(EXPIRE_ANIM_HASH, true);

            timerBar.fillAmount = 0;
            succesBar.fillAmount = 0;
        }
        else
        {
            anim.SetBool(FAIL_ANIM_HASH, true);
        }
        UpdateScheduler.UnRegisterUpdate(DepleteTimer);
    }

    /// <summary>
    /// Called every frame while timer is depleting. Depletes the timer bar and succes bar based on the time left in the QTE and the succes window.
    /// </summary>
    private void DepleteTimer()
    {
        float barPercentageLeft = math.clamp(timerBar.fillAmount - Time.deltaTime / qteDuration, 0, float.MaxValue);
        // Timer bar
        timerBar.fillAmount = barPercentageLeft;

        // Succes bar
        if (qteWindow01 > barPercentageLeft)
        {
            succesBar.fillAmount = barPercentageLeft;
        }
    }
}