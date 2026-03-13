using Fire_Pixel.Networking;
using UnityEngine;



public class PlayerVisualsManager : SmartNetworkBehaviour
{
    public static PlayerVisualsManager Instance { get; private set; }

    [SerializeField] private Player[] players;
    [SerializeField] private float attackPrepareTime = 0.5f;
    [SerializeField] private float attackResetDelay = 1f;

    public static float AttackPrepareTime => Instance.attackPrepareTime;

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

    public void DoAttackAnimation_Local(int animationNameHash, float delayBeforeImpact)
    {
        SkillUIManager.FadeOut();
        players[CombatTurnContext.AttackerGameId].Anim.StartWeaponAttack(animationNameHash, delayBeforeImpact, attackPrepareTime, attackResetDelay);
    }
    public void DoSupportAnimation_Local(int animationNameHash)
    {
        SkillUIManager.FadeOut();
        players[CombatTurnContext.AttackerGameId].Anim.StartWeaponSupport(animationNameHash);
    }


    [System.Serializable]
    public class Player
    {
        public PlayerAnimator Anim;
        public Transform CamTransform;
    }
}