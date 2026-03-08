using Fire_Pixel.Networking;
using UnityEngine;



public class PlayerVisualsManager : SmartNetworkBehaviour
{
    public static PlayerVisualsManager Instance { get; private set; }

    [SerializeField] private Player[] players;

    private Camera mainCam;



    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
    }
    protected override void OnNetworkSystemsSetup()
    {
        mainCam.transform.SetParent(players[LocalClientGameId].CamTransform, false, false);
    }

    public void DoAttackerAnimation_Local(float delayBeforeImpact)
    {
        SkillUIManager.FadeOut();
        players[CombatTurnContext.AttackerGameId].Anim.StartWeaponAttack(delayBeforeImpact);
    }


    [System.Serializable]
    public class Player
    {
        public PlayerAnimator Anim;
        public Transform CamTransform;
    }
}