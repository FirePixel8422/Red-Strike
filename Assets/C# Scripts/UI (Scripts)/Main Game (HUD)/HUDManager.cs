using UnityEngine;


public class HUDManager : MonoBehaviour
{
#pragma warning disable UDR0001
    private static HUDManager instance;
#pragma warning restore UDR0001


    [SerializeField] private ResourceBarUI localHealthBar, opponentHealthBar;
    [SerializeField] private ResourceBarUI localEnergyBar;
    public static ResourceBarUI LocalHealthBar => instance.localHealthBar;
    public static ResourceBarUI OpponentHealthBar => instance.opponentHealthBar;
    public static ResourceBarUI LocalEnergyBar => instance.localEnergyBar;


    private void Awake()
    {
        instance = this;
    }
}