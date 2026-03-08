using UnityEngine;


public class HUDHandler : MonoBehaviour
{
#pragma warning disable UDR0001
    private static HUDHandler instance;
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