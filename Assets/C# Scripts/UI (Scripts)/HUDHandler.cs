using Fire_Pixel.Utility;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CanvasGroup))]
public class HUDHandler : NetworkBehaviour
{
    public static HUDHandler Instance { get; private set; }


    [SerializeField] private ResourceBarUI localHealthBar, opponentHealthBar;
    [SerializeField] private ResourceBarUI localEnergyBar;
    public ResourceBarUI LocalHealthBar => localHealthBar;
    public ResourceBarUI OpponentHealthBar => opponentHealthBar;
    public ResourceBarUI LocalEnergyBar => localEnergyBar;

    [Space(15)]

    [SerializeField] private float fadeOutTime;
    [SerializeField] private float fadeInTime;
    [SerializeField] private Image screenBlock;

    private CanvasGroup canvasGroup;


    private void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }


    #region Fade In/Out system

    public void FadeIn()
    {
        Instance.screenBlock.enabled = false;
        UpdateScheduler.RegisterUpdate(FadeInSequence);
    }
    public void FadeOut()
    {
        Instance.screenBlock.enabled = true;
        UpdateScheduler.RegisterUpdate(FadeOutSequence);
    }
    private void FadeInSequence()
    {
        float alpha = canvasGroup.alpha;
        canvasGroup.alpha = Mathf.MoveTowards(alpha, 1, Time.deltaTime / fadeInTime);

        if (alpha == 1)
        {
            UpdateScheduler.UnRegisterUpdate(FadeInSequence);
        }
    }
    private void FadeOutSequence()
    {
        float alpha = canvasGroup.alpha;
        canvasGroup.alpha = Mathf.MoveTowards(alpha, 0, Time.deltaTime / fadeOutTime);

        if (alpha == 0)
        {
            UpdateScheduler.UnRegisterUpdate(FadeOutSequence);
        }
    }

    #endregion


    public override void OnDestroy()
    {
        base.OnDestroy();

        UpdateScheduler.UnRegisterUpdate(FadeInSequence);
        UpdateScheduler.UnRegisterUpdate(FadeOutSequence);
    }


    private float value01;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            value01 += 0.1f;
            localEnergyBar.UpdateBar(value01);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            value01 -= 0.1f;
            localEnergyBar.UpdateBar(value01);
        }
    }
}