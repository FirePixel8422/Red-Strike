using Fire_Pixel.Networking;
using Unity.Netcode;
using UnityEngine;


public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private PlayerStats defPlayerStats;
    [SerializeField] private CombatContext combatContext;

    private static SyncedAction QuitGame = new SyncedAction();


    private void Awake()
    {
        Instance = this;

        PlayerStats[] playerStats = new PlayerStats[GlobalGameData.MaxPlayers];
        for (int i = 0; i < GlobalGameData.MaxPlayers; i++)
        {
            playerStats[i] = new PlayerStats(defPlayerStats);
        }

        combatContext = new CombatContext(playerStats);

        TurnManager.TurnStarted += StartAttackingPhase;

        QuitGame.Create();
        QuitGame += () =>
        {
            Application.Quit();
        };
    }


    public void StartAttackingPhase()
    {

    }


    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    public void AttackTargetPlayer_ServerRPC(int targetClientGameId)
    {
        StartDefensePhase_ClientRPC(GameIdRPCTargets.SendToTargetClient(targetClientGameId));
    }

    [ClientRpc(InvokePermission = RpcInvokePermission.Server, Delivery = RpcDelivery.Reliable)]
    public void StartDefensePhase_ClientRPC(GameIdRPCTargets rpcTargets)
    {
        if (rpcTargets.IsTarget == false) return;
    }

    private void OnDestroy()
    {
        TurnManager.TurnStarted -= StartAttackingPhase;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SkillManager.Instance.GlobalSkillList.SelectRandom().Resolve(combatContext, DefenseResult.None);

            QuitGame.Schedule_ServerRPC(3);
        }
    }
}
