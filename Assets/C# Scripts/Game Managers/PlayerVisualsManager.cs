using Fire_Pixel.Networking;
using UnityEngine;



public class PlayerVisualsManager : SmartNetworkBehaviour
{
    [SerializeField] private Player[] players;
    private Camera mainCam;

#pragma warning disable UDR0001
    public static Player[] Players{ get; private set; }
#pragma warning restore UDR0001



    private void Awake()
    {
        Players = players;
        mainCam = Camera.main;
    }
    protected override void OnNetworkSystemsSetup()
    {
        mainCam.transform.SetParent(players[LocalClientGameId].CamTransform, false, false);
    }

    public static void DoAttackAnimation(float delayBeforeImpact)
    {
        Players[CombatTurnContext.AttackerGameId].Anim.StartWeaponAttack(delayBeforeImpact);
    }


    [System.Serializable]
    public class Player
    {
        public PlayerAnimator Anim;
        public Transform CamTransform;
    }
}