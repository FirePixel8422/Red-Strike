using UnityEngine;


public class HUDManager : MonoBehaviour
{
#pragma warning disable UDR0001
    public static HUDManager Instance;
#pragma warning restore UDR0001


    [SerializeField] private ResourceBarUI localHealthBar, opponentHealthBar;
    [SerializeField] private ResourceBarUI localEnergyBar;

    [SerializeField] private StatusEffectBar localStatusBar, opponentStatusBar;

    public ResourceBarUI LocalHealthBar => localHealthBar;
    public ResourceBarUI OpponentHealthBar => opponentHealthBar;
    public ResourceBarUI LocalEnergyBar => localEnergyBar;
    public StatusEffectBar LocalStatusBar => localStatusBar;
    public StatusEffectBar OpponentStatusBar => opponentStatusBar;


    private void Awake()
    {
        Instance = this;
    }


    private void OnDestroy()
    {
        localHealthBar.Destroy();
        opponentHealthBar.Destroy();
        localEnergyBar.Destroy();
    }
}