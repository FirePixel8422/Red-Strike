using Fire_Pixel.Networking;
using Unity.Netcode;
using UnityEngine;


public class PlayerVisualsManager : SmartNetworkBehaviour
{
#pragma warning disable UDR0001
    public static PlayerAnimator[] PlayerAnimators;
#pragma warning restore UDR0001



    private void Awake()
    {
        PlayerAnimators = GetComponentsInChildren<PlayerAnimator>(true);
    }
}