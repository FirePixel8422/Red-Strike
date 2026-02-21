using Unity.Netcode;
using System;


namespace Fire_Pixel.Networking
{
    /// <summary>
    /// MB manager class that tracks player on turn GameId through <see cref="ClientManager"/> GameId System. Also has callback event for OnTurnChanged and OnTurn -Started and -Ended
    /// </summary>
    public class TurnManager : SmartNetworkBehaviour
    {
        public static TurnManager Instance { get; private set; }
        private void Awake() => Instance = this;


        private int clientOnTurnId = -1;
        public static int ClientOnTurnId => Instance.clientOnTurnId;

        public static bool IsMyTurn => Instance.clientOnTurnId == LocalClientGameId;

#pragma warning disable UDR0001
        public static event Action<int> TurnChanged;
        public static event Action TurnStarted;
        public static event Action TurnEnded;
#pragma warning restore UDR0001


        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                MatchManager.StartMatch_OnServer += StartGame_OnServer;
            }
        }
        private void StartGame_OnServer()
        {
            clientOnTurnId = EzRandom.Range(0, GlobalGameData.MAX_PLAYERS);

            SwapToNextTurn_ClientRPC(-1, clientOnTurnId);
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        public void EndTurn_ServerRPC()
        {
            int prevClientOnTurnId = clientOnTurnId;
            clientOnTurnId.IncrementSmart(GlobalGameData.MAX_PLAYERS);

            SwapToNextTurn_ClientRPC(prevClientOnTurnId, clientOnTurnId);
        }
        [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void SwapToNextTurn_ClientRPC(int prevClientOnTurnId, int nextClientOnTurnId)
        {
            clientOnTurnId = nextClientOnTurnId;

            // Invoke OnTurnChanged with new clientId.
            TurnChanged?.Invoke(clientOnTurnId);

            // If it becomes or stays local clients turn, Invoke OnMyTurnStarted.
            if (IsMyTurn)
            {
                TurnStarted?.Invoke();
            }
            // If its not local clients turn, check if they lost the turn and Invoke OnTurnEnded if so.
            else if (prevClientOnTurnId == LocalClientGameId)
            {
                TurnEnded?.Invoke();
            }
        }
    }
}