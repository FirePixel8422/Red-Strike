using FirePixel.Networking;
using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private PlayerStats defPlayerStats;

    private CombatContext battleCtx;


    private void Awake()
    {
        Instance = this;

        PlayerStats[] playerStats = new PlayerStats[GlobalGameData.MaxPlayers];
        Array.Fill(playerStats, defPlayerStats);

        battleCtx = new CombatContext(playerStats);

        TurnManager.OnTurnStarted += StartAttackingPhase;
    }

    public void StartAttackingPhase()
    {

    }


    [ServerRpc(InvokePermission = RpcInvokePermission.Everyone, Delivery = RpcDelivery.Reliable)]
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
        TurnManager.OnTurnStarted -= StartAttackingPhase;
    }
}
