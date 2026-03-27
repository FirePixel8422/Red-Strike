using Fire_Pixel.Networking;
using UnityEngine;



public class PlayerVisualsManager : SmartNetworkBehaviour
{
    public static PlayerVisualsManager Instance { get; private set; }

    [SerializeField] private Player[] players;
    [SerializeField] private float attackPrepareTime = 0.5f;
    [SerializeField] private float attackResetDelay = 1f;

    public Player AttackerPlayer => players[CombatManager.Instance.CombatCtx.AttackerGameId];
    public float AttackPrepareTime => attackPrepareTime;

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
        SkillUIManager.Instance.FadeOut();
        AttackerPlayer.Anim.StartWeaponAttack(animationNameHash, delayBeforeImpact, attackPrepareTime, attackResetDelay);
    }
    public void DoSupportAnimation_Local(int animationNameHash)
    {
        SkillUIManager.Instance.FadeOut();
        AttackerPlayer.Anim.StartWeaponSupport(animationNameHash);
    }
    public void UpdateWeapon(int playerId, int weaponId)
    {
        players[playerId].WeaponHandler.SwapToWeapon(weaponId);
    }


    [System.Serializable]
    public class Player
    {
        public PlayerAnimator Anim;
        public PlayerWeaponHandler WeaponHandler;
        public Transform CamTransform;
    }
}